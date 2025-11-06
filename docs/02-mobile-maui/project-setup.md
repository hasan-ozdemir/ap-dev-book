---
title: Project Setup & Scaffolding
description: Create production-ready .NET MAUI projects using both Visual Studio and the .NET CLI, aligned with Prodyum conventions.
last_reviewed: 2025-10-29
owners:
  - @prodyum/maui-guild
---

# Project Setup & Scaffolding

Follow this guide to spin up a maintainable .NET MAUI solution that targets iOS, Android, macOS, and Windows from a single codebase.

## 1. Prerequisites

- Visual Studio 2022 17.14 with the **.NET Multi-platform App UI development** workload. [^vsrelease]
- .NET SDK 9.0.305 or later with the `maui` workload installed. [^dotnet9]
- Apple Developer Program access (for iOS distribution) and configured provisioning profiles.
- Android emulators targeting API levels 34 and 35 to satisfy Google Play policies. [^googleplay]

> **Baseline:** Confirm your environment meets the requirements in [Visual Studio & CLI Setup](../00-foundations/tooling-setup.md) before proceeding.

## 2. Create a solution using Visual Studio

1. Select **Create a new project** → **.NET MAUI App**.
2. Name the solution `Prodyum.TodoApp` and set the location under `src/`.
3. Choose **Use MVVM Toolkit** to scaffold MVVM infrastructure.
4. Enable **Multi-platform** targets (Android, iOS, macOS, Windows).
5. After creation, run the project against the Android emulator and Windows desktop to verify baseline builds.
6. Add launch profiles:
   - `Windows Machine` (WinUI 3)
   - `iOS Local Device` (requires Mac build host)
   - `Android Emulator` (API 35)

### Recommended project structure

```
src/
  Prodyum.TodoApp/
    Prodyum.TodoApp.csproj
    App.xaml
    Resources/
    Platforms/
  Prodyum.TodoApp.Core/         → shared business logic
  Prodyum.TodoApp.Infrastructure/→ REST clients, storage
tests/
  Prodyum.TodoApp.Tests/
```

Split shared logic into `.Core` and `.Infrastructure` libraries to support reuse across mobile, web, and desktop fronts.

## 3. Create the same solution via the .NET CLI

```bash
mkdir src && cd src
dotnet new maui -n Prodyum.TodoApp -f net9.0
dotnet new classlib -n Prodyum.TodoApp.Core -f net9.0
dotnet new classlib -n Prodyum.TodoApp.Infrastructure -f net9.0
dotnet new xunit -n Prodyum.TodoApp.Tests -f net9.0
dotnet sln Prodyum.TodoApp.sln add \
  Prodyum.TodoApp/Prodyum.TodoApp.csproj \
  Prodyum.TodoApp.Core/Prodyum.TodoApp.Core.csproj \
  Prodyum.TodoApp.Infrastructure/Prodyum.TodoApp.Infrastructure.csproj \
  Prodyum.TodoApp.Tests/Prodyum.TodoApp.Tests.csproj
dotnet add Prodyum.TodoApp/Prodyum.TodoApp.csproj reference \
  Prodyum.TodoApp.Core/Prodyum.TodoApp.Core.csproj \
  Prodyum.TodoApp.Infrastructure/Prodyum.TodoApp.Infrastructure.csproj
dotnet add Prodyum.TodoApp.Infrastructure/Prodyum.TodoApp.Infrastructure.csproj package Refit
```

The CLI template ships with the single-project structure introduced in .NET MAUI; SlN references keep dependencies explicit. [^mauiquickstart]

### Enable multi-targeting constants

Add the following `PropertyGroup` to `Prodyum.TodoApp.csproj` to expose conditional compilation symbols:

```xml
<PropertyGroup>
  <UseMaui>true</UseMaui>
  <TargetFrameworks>net9.0-android;net9.0-ios;net9.0-maccatalyst;net9.0-windows10.0.19041.0</TargetFrameworks>
  <DefineConstants>$(DefineConstants);USE_APP_CENTER</DefineConstants>
</PropertyGroup>
```

Later modules will show how to swap `USE_APP_CENTER` with Azure Monitor once App Center retires.

## 4. Apply Prodyum conventions

- **Code style:** Add `.editorconfig` at the solution root to enforce analyzers. Use `dotnet format` in CI.
- **Directory packages:** Introduce `Directory.Packages.props` to centralize NuGet versions.
- **Secrets:** Use `dotnet user-secrets` for environment-specific configuration during development.
- **App icons:** Replace the default `Resources/AppIcon` assets with Prodyum branding guidelines.
- **Analytics hooks:** Configure dependency injection to register telemetry (Application Insights, Azure Monitor).

## 5. Verify workloads and targets

```powershell
dotnet workload list | Select-String "maui"
dotnet build Prodyum.TodoApp.sln
dotnet build Prodyum.TodoApp/Prodyum.TodoApp.csproj -t:Run -f net9.0-android
```

Run Windows builds locally; use connected Mac build agent or Dev Tunnel to produce iOS/macOS builds.

## 6. Seed environment configuration

Create `appsettings.Development.json` and `appsettings.Production.json` in `Prodyum.TodoApp` with the following structure:

```json
{
  "Api": {
    "BaseUrl": "https://api.prodyum.dev"
  },
  "Telemetry": {
    "InstrumentationKey": "<replace-in-dev-secrets>"
  }
}
```

Use `dotnet user-secrets set "Telemetry:InstrumentationKey" "<guid>"` during development.

## 7. Commit and push

```bash
git init
git checkout -b feature/maui-seed
git add .
git commit -m "chore: scaffold Prodyum MAUI solution"
git push origin feature/maui-seed
```

Open a pull request and request a peer review before merging into `main`.

---

## References

[^vsrelease]: Microsoft, "Visual Studio 2022 release history," accessed November 1, 2025. citeturn1search1
[^dotnet9]: Microsoft, "What's new in .NET 9," accessed November 1, 2025. citeturn4search0
[^googleplay]: Google Play Console Help, "Target API level requirements for Google Play apps," updated 2025. citeturn8search0
[^mauiquickstart]: Microsoft Learn, "Build your first .NET MAUI app," updated June 30, 2025. citeturn0search0



