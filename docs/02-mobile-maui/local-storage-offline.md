---
title: Local Storage & Offline Data Strategies in .NET MAUI
description: Store, encrypt, sync, and process data locally on Android, iOS, Windows, and Mac Catalyst.
last_reviewed: 2025-10-30
owners:
  - @prodyum/data-guild
---

# Local Storage & Offline Data Strategies in .NET MAUI

MAUI apps often need to store preferences, cache content, or support offline workflows. This guide covers the built-in Essentials APIs and advanced options—SQLite, EF Core, file system, secure storage, and synchronization patterns—so you can choose the right approach for each scenario.

---

## 1. Storage decision matrix

| Use case | Recommended API | Platform notes |
| --- | --- | --- |
| User settings / flags | `Preferences` | Stored in platform-specific settings stores (SharedPreferences, NSUserDefaults, ApplicationData).[^pref-learn] |
| Secrets (tokens, keys) | `SecureStorage` | Wraps platform vaults such as Android Keystore and iOS Keychain; requires device lock on Android.[^secure-storage] |
| Structured data | SQLite (via `SQLite-net` or EF Core) | Supports relational queries, indexes, migrations. |
| Files / media | `FileSystem` (`AppDataDirectory`, `CacheDirectory`) | Private storage by default; use pickers for user-visible folders.[^filesystem] |
| Large objects | Local file + metadata (SQLite) | Keep binary data out of the database where possible. |
| Offline-first sync | EF Core + Datasync/Azure Mobile Apps | Provide conflict resolution and background sync.[^datasync][^azure-mobile] |

---

## 2. Preferences

```csharp
Preferences.Default.Set("onboarding_complete", true);
bool isComplete = Preferences.Default.Get("onboarding_complete", false);
```

- Use small key/value pairs; avoid sensitive data.
- On Windows, data stored in `LocalSettings`; clearing app data removes preferences.
- Provide migration logic when renaming keys.
- Wrap calls behind an interface so you can swap storage providers or inject test doubles in unit tests.[^pref-learn]

---

## 3. Secure storage

```csharp
await SecureStorage.Default.SetAsync("auth_token", token);
var token = await SecureStorage.Default.GetAsync("auth_token");
```

- SecureStorage relies on platform vaults (Android Keystore, iOS/macOS Keychain, Windows Credential Locker); handle `SecureStorageException` for cases where no device credential exists or the secure store is invalid.[^secure-storage]

---

## 4. File system access

```csharp
var path = Path.Combine(FileSystem.AppDataDirectory, "notes.json");
await File.WriteAllTextAsync(path, json);
```

- `AppDataDirectory`: private app storage.
- `CacheDirectory`: temp files (OS may purge).
- For user-visible files, use `FilePicker`/`FolderPicker` and platform storage permissions.
- On Android 13+, request granular media permissions (images/video/audio) when touching shared media libraries.[^filesystem]
- `AppDataDirectory` maps to app-private storage on every platform (Library/Application Support on iOS, LocalFolder on Windows).[^filesystem]

---

## 5. SQLite & EF Core

### 5.1 SQLite-net

```csharp
using SQLite;

var dbPath = Path.Combine(FileSystem.AppDataDirectory, "contoso.db3");
var db = new SQLiteAsyncConnection(dbPath);
await db.CreateTableAsync<TodoItem>();
await db.InsertAsync(new TodoItem { Title = "Sync docs" });
```

- Lightweight, minimal overhead; perfect for mobile caching.
- Use `SQLitePCLRaw.bundle_green` for consistent runtime.

### 5.2 EF Core with SQLite

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Filename={Path.Combine(FileSystem.AppDataDirectory, "contoso.db")}"));
```

Run migrations on startup:

```csharp
using var scope = app.Services.CreateScope();
await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
```

- EF Core 9 preview adds performance optimizations for mobile; ensure trimmed builds preserve EF metadata by adding a `TrimmerRootDescriptor` for DbContext types.[^linking]

### 5.3 Pre-populated databases

Store `.db` file under `Resources/Raw`; copy on first run:

```csharp
using var stream = await FileSystem.OpenAppPackageFileAsync("seed.db3");
using var dest = File.Create(dbPath);
await stream.CopyToAsync(dest);
```

---

## 6. LiteDB & NoSQL options

- LiteDB (embedded document database) works on MAUI; supports LINQ queries and file storage.
- Use for flexible schema; be mindful of file size growth.
- Set `TrimmerRootDescriptor` entries to preserve dynamic types when linking.[^linking]

---

## 7. Offline sync patterns

### 7.1 Azure Mobile Apps / App Service

- Use `Microsoft.Datasync.Client` for offline sync with conflict resolution policies and delta queries.[^datasync]
- Configure `SyncTable<T>` instances and call `PushAsync`/`PullAsync` to synchronize local tables with the backend.[^azure-mobile]

### 7.2 Datasync Community Toolkit

- Wrap EF Core contexts with toolkit helpers to get change tracking, queued push/pull, and customizable conflict handlers.[^datasync]
### 7.3 Custom sync

- Track `UpdatedAt` and `IsDirty` columns; send delta to backend.
- Resolve conflicts using last-write-wins or merge strategies.
- Queue offline operations (e.g., `System.Text.Json` serialized commands) and replay when online using `Connectivity.Current`.

---

## 8. Data encryption

- Use `AES-GCM` via `System.Security.Cryptography` to encrypt files before writing.
- For SQLite, consider SQLCipher (community license) or `Microsoft.Data.Sqlite` with `connectionString += ";Password=..."` for built-in encryption support.[^sqlite-encryption]
- Secure encryption keys via `SecureStorage`; rotate keys periodically.

---

## 9. Large data processing

- Stream large files instead of loading into memory (`File.OpenRead` + `Stream.CopyToAsync`).
- Use background services (`IHostedService`, `BackgroundService`) for batch processing.
- On Android, handle storage permissions (MANAGE_EXTERNAL_STORAGE for legacy, or scoped storage).

---

## 10. Testing & debugging storage

- Add diagnostic page listing stored files and sizes (guard with debug flag).
- Use platform tooling:
  - Android Studio Device Explorer.
  - Xcode Devices and Simulators logs.
  - Windows `%LocalAppData%` path.
- Automate storage tests: ensure migrations apply, data loads offline, and secure storage returns expected values.

---

## Checklist

- [ ] Storage requirements mapped to appropriate API.
- [ ] Sensitive data routed through SecureStorage/encryption.
- [ ] Database migrations tested; seed data copied on first run.
- [ ] Offline sync strategy defined (queue, conflict resolution).
- [ ] Telemetry monitors storage failures and disk usage.

---

## Further reading

- [Store local data with SQLite in a .NET MAUI app (training module)](https://learn.microsoft.com/en-us/training/modules/store-local-data/).
- [Secure storage - .NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/secure-storage?view=net-maui-9.0).
- [Linking a .NET MAUI Android app](https://learn.microsoft.com/en-us/dotnet/maui/android/linking).
- [Syncing data with Azure Mobile Apps and .NET MAUI](https://learn.microsoft.com/en-us/shows/dotnetconf-focus-on-maui/syncing-data-with-azure-mobile-apps-and-dotnet-maui).
- [Datasync Community Toolkit – Creating offline clients](https://communitytoolkit.github.io/Datasync/in-depth/client/).
- [Encryption with Microsoft.Data.Sqlite](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/encryption).

[^pref-learn]: Microsoft Learn, "Application settings management in .NET MAUI," accessed November 1, 2025. citeturn21search0
[^secure-storage]: Microsoft Learn, "Secure storage - .NET MAUI," accessed November 1, 2025. citeturn20search0
[^filesystem]: Microsoft Learn, "File system helpers - .NET MAUI," accessed November 1, 2025. citeturn22search0
[^linking]: Microsoft Learn, "Linking a .NET MAUI Android app," updated October 24, 2024. citeturn9search0
[^datasync]: .NET Community Toolkit, "Datasync - Creating offline clients," accessed November 1, 2025. citeturn23search0
[^azure-mobile]: Microsoft Learn, "Syncing data with Azure Mobile Apps and .NET MAUI," accessed November 1, 2025. citeturn23search1
[^sqlite-encryption]: Microsoft Learn, "Encryption with Microsoft.Data.Sqlite," accessed November 1, 2025. citeturn24search0


