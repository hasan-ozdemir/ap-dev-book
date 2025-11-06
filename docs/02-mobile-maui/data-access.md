---
title: Data Access & Offline Sync
description: Design resilient data flows for .NET MAUI apps, covering HTTP clients, serialization, caching, and secure local storage.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

# Data Access & Offline Sync

Modern .NET MAUI solutions combine cloud-backed services with local caches so mobile and desktop users see responsive, reliable experiences even when connectivity fluctuates. This guide consolidates recommended patterns for .NET 9, including resilient HTTP pipelines, System.Text.Json source generators, SQLite-backed offline stores, and secure token storage.

---

## 1. Layered architecture

```
ViewModels/Pages
    -> (interfaces)
Repositories (application services)
    -> Remote data providers (HTTP, gRPC, GraphQL)
    -> Serialization (System.Text.Json source generators)
    -> Local caches (SQLite, Preferences, SecureStorage)
```

- Keep UI testable by depending on interfaces implemented by repositories.
- Map transport DTOs to domain entities inside the repository to isolate backend changes.
- Wrap cache implementations so they can be swapped (for example, in-memory vs. SQLite) during tests.

> Tip: Register repositories and platform services in `MauiProgram` using dependency injection to respect the lifecycle guidance in the MAUI architecture e-book.[^layering]

---

## 2. Resilient HTTP clients

### 2.1 Configure `IHttpClientFactory`

```csharp
builder.Services
    .AddHttpClient("CatalogClient", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["Api:Catalog"]);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("devyum/1.0");
    })
    .AddStandardResilienceHandler();
```

- `AddStandardResilienceHandler` wires retry, timeout, circuit-breaker, and hedging strategies that ship with `Microsoft.Extensions.Http.Resilience`.[^resilience]
- Prefer named or typed clients per backend to keep logging and telemetry scoped.
- Inject `IHttpClientFactory` into repositories and call `CreateClient("CatalogClient")`.[^http-logging]

### 2.2 Handle transient failures

- Implement policies (token refresh, retry-after back-off) on top of the resilience handler.
- Capture HTTP traces via built-in logging so crashes can be correlated with backend telemetry.[^http-logging]

---

## 3. Efficient JSON serialization

- Use System.Text.Json source generation to trim startup costs and stay linker-safe when you ship Native AOT or trim-heavy builds.[^json-source-gen]

```csharp
[JsonSerializable(typeof(ProductDto))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Serialization)]
internal partial class CatalogJsonContext : JsonSerializerContext
{
}

var product = await response.Content
    .ReadFromJsonAsync<ProductDto>(CatalogJsonContext.Default.ProductDto);
```

- Keep context classes in shared projects so they are available on every platform.
- Source generation avoids runtime reflection, which is critical once you enable trimming or Native AOT for Android packaging.[^native-aot]

---

## 4. Offline-first caching

### 4.1 SQLite for structured data

- Persist sync queues, reference data, and user drafts with `sqlite-net-pcl` or Entity Framework Core.
- Follow the Microsoft Learn module to set up asynchronous CRUD operations and keep the UI thread responsive.[^sqlite-module]

```csharp
public class LocalStore
{
    readonly SQLiteAsyncConnection _db;

    public LocalStore(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
    }

    public Task InitAsync() =>
        _db.CreateTableAsync<ProductEntity>();

    public Task<List<ProductEntity>> GetPendingAsync() =>
        _db.Table<ProductEntity>()
           .Where(p => p.SyncState == SyncState.Pending)
           .ToListAsync();
}
```

### 4.2 Sync workflow

1. Every mutation writes to SQLite with a `SyncState` flag.
2. A background sync service dequeues records when `Connectivity.Current.NetworkAccess` reports internet availability.
3. Successful items are marked as `Synced` or removed; failures increment retry counters to avoid hammering the backend.
4. Schedule work with `PeriodicTimer`, Android WorkManager, or iOS BackgroundFetch so sync runs even when the UI is closed.

---

## 5. Lightweight settings and secure storage

### 5.1 Preferences

- Store non-sensitive flags (feature toggles, UI hints) using `Preferences.Default`.[^preferences]
- Keep keys in a static class to avoid typos and document default values.

```csharp
const string HasSeenOnboardingKey = "settings.hasSeenOnboarding";

Preferences.Default.Set(HasSeenOnboardingKey, true);
bool hasSeen = Preferences.Default.Get(HasSeenOnboardingKey, false);
```

### 5.2 Secure tokens

- Store tokens and secrets in `SecureStorage.Default`.[^secure-storage]
- Handle exceptions gracefully; lock-screen changes or key rotation may invalidate entries.

```csharp
public async Task PersistSessionAsync(Session session)
{
    await SecureStorage.Default.SetAsync("session.accessToken", session.AccessToken);
    Preferences.Default.Set("session.expiresUtc", session.ExpiresUtc);
}
```

- When migrating from Xamarin.Forms, use the Community Toolkit `LegacySecureStorage` helper to move encrypted values once.[^legacy-secure-storage]

### 5.3 Platform entitlements

- iOS and Mac Catalyst require Keychain entitlements in `Entitlements.plist`; add your bundle identifier to `keychain-access-groups`.[^keychain]
- On Android, exclude secure storage from auto-backup so tokens are not restored on a different device.[^secure-storage]

---

## 6. Testing and diagnostics

- Unit-test repositories with mocked `HttpMessageHandler` or `WireMock.Net` stubs to verify DTO mappings.
- Run instrumentation tests against SQLite caches to exercise migrations and concurrency.
- Capture device logs with `adb logcat` or Xcode Console when validating secure storage entitlements.
- Feed HTTP resilience metrics into Application Insights or BrowserStack dashboards to spot retry storms.

---

## 7. Checklist

- [ ] Register named HTTP clients with resilience handlers.
- [ ] Generate JSON contexts for every DTO used in trimmed builds.
- [ ] Initialize SQLite and run migrations at app start.
- [ ] Separate non-sensitive settings (Preferences) from secrets (SecureStorage).
- [ ] Configure iOS/Mac entitlements and Android backup rules for secure storage.
- [ ] Implement sync retry policies and telemetry.
- [ ] Add regression tests covering offline and online transitions.

By following these patterns, Prodyum teams can ship MAUI experiences that remain responsive, resilient, and secure across platforms-even when connectivity is a moving target.

---

## References

[^layering]: Microsoft Learn, "Architect modern web applications with .NET MAUI," accessed October 30 2025.citeturn1search6
[^resilience]: Microsoft Learn, "Build resilient HTTP apps with .NET," accessed October 30 2025.citeturn2search1
[^http-logging]: Microsoft Learn, "Use IHttpClientFactory to implement resilient HTTP requests," accessed October 30 2025.citeturn1search0
[^json-source-gen]: Microsoft Learn, "High-performance JSON serialization with System.Text.Json source generators," accessed October 30 2025.citeturn0search0
[^native-aot]: Microsoft Learn, "Publish .NET MAUI apps using Native AOT," accessed October 30 2025.citeturn8search0
[^sqlite-module]: Microsoft Learn, "Add a local database to a .NET MAUI app," accessed October 30 2025.citeturn3search0
[^preferences]: Microsoft Learn, "Application settings management for .NET MAUI apps," accessed October 30 2025.citeturn5search1
[^secure-storage]: Microsoft Learn, "Secure storage - .NET MAUI," accessed October 30 2025.citeturn11search0
[^legacy-secure-storage]: .NET MAUI Community Toolkit, "LegacySecureStorage helper for Xamarin Forms migration," accessed October 30 2025.citeturn11search2
[^keychain]: Microsoft Learn, "Enable keychain access for secure storage in .NET MAUI," accessed October 30 2025.citeturn10search10
