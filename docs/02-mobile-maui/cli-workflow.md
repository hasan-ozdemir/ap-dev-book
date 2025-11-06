---
title: .NET MAUI Command Line Workflow
description: Create, build, debug, and publish .NET MAUI apps entirely from the terminal.
last_reviewed: 2025-10-29
owners:
  - @prodyum/maui-guild
---

# .NET MAUI Command Line Workflow

The .NET CLI provides everything you need to scaffold, run, test, and publish .NET MAUI apps without a full IDE. This guide covers the essential commands and scripts you can combine in CI/CD pipelines or lightweight local environments.

---

## 1. Tooling prerequisites

- .NET 9 SDK (9.0.x). Check with `dotnet --list-sdks`.
- MAUI workload: `dotnet workload install maui`. Update regularly (`dotnet workload update`) to pick up platform SDK changes.citeturn0search7
- Platform dependencies:
  - Android: Android SDK/NDK via `sdkmanager` or Visual Studio components.
  - iOS/Mac Catalyst: Xcode 16.1+, command-line tools, Apple certificates.
  - Windows: Windows App SDK runtime (`winget install Microsoft.WindowsAppRuntime.1.5`).
  - Linux (WIP): watch runtime for preview support; currently limited to tooling builds.

---

## 2. Scaffolding projects

### 2.1 Templates

```bash
dotnet new maui -n Contoso.Mobile
dotnet new maui-blazor -n Contoso.BlazorApp
```

Customise output:

```bash
dotnet new maui -n Contoso.Mobile -f net9.0-android -ou src/Contoso.Mobile
```

Add solution & projects:

```bash
dotnet new sln -n Contoso.Mobile
dotnet sln add src/Contoso.Mobile/Contoso.Mobile.csproj
```

---

## 3. Workload management

List workloads:

```bash
dotnet workload list
```

Repair workload if install fails:

```bash
dotnet workload repair maui
```

Use the Workload Manifest Updater to pin specific versions (`dotnet workload update --from-rollback-file rollback.json`) or set `workloadVersion` in `global.json` so CI agents install deterministic assets.[^workload-cli]

---

## 4. Build & run commands

### 4.1 Multi-target builds

```bash
dotnet build Contoso.Mobile.sln -c Debug
dotnet build Contoso.Mobile.csproj -c Release -f net9.0-android
```

### 4.2 Run on target platforms

```bash
# Android emulator/device
dotnet build -t:Run -f net9.0-android

# Windows
dotnet build -t:Run -f net9.0-windows10.0.19041.0

# iOS simulator
dotnet build -t:Run -f net9.0-ios /p:_DeviceName=:v2:udid=00008110-000A0C3C0A95001E
```

Use `maui doctor` to diagnose environment issues:

```bash
maui doctor
```

---

## 5. Packaging & publishing

### 5.1 Android

```bash
dotnet publish -f net9.0-android -c Release \
  -p:AndroidPackageFormat=aab \
  -p:AndroidSdkDirectory=$ANDROID_HOME
```

To sign with a keystore:

```bash
dotnet publish -f net9.0-android -c Release \
  -p:AndroidPackageFormat=aab \
  -p:AndroidSigningKeyStore=contoso.keystore \
  -p:AndroidSigningKeyAlias=contoso \
  -p:AndroidSigningKeyPass=$(KEY_PASS) \
  -p:AndroidSigningStorePass=$(STORE_PASS)
```

### 5.2 iOS

```bash
dotnet publish -f net9.0-ios -c Release \
  -p:ArchiveOnBuild=true \
  -p:BuildIpa=true \
  -p:IpaPackageDir=artifacts/ipa
```

Requires matching provisioning profile and certificates in Keychain.

### 5.3 Windows (MSIX)

```bash
dotnet publish -f net9.0-windows10.0.19041.0 -c Release \
  -p:WindowsAppSDKSelfContained=true \
  -p:GenerateAppxPackageOnBuild=true \
  -p:AppxPackageDir=artifacts/msix/
```

### 5.4 Mac Catalyst

```bash
dotnet publish -f net9.0-maccatalyst -c Release \
  -p:UseAppHost=true \
  -p:MacCatalystPackageType=Pkg
```

---

## 6. Advanced build options

- **Trimming**: `-p:PublishTrimmed=true` to reduce package size (test thoroughly).
- **AOT/Native AOT**:
  ```bash
  dotnet publish -f net9.0-android -c Release \
    -p:RunAOTCompilation=true \
    -p:UseNativeAot=true
  ```
- **Configuration properties**: Use `Directory.Build.props` to share settings (e.g., `$(ApplicationId)`).
- **CI-friendly output**: `-bl` to generate MSBuild binary logs for diagnostics.

---

## 7. Automation scripts

### 7.1 PowerShell build script (Windows)

```powershell
param(
    [string]$Configuration = "Release",
    [string]$Framework = "net9.0-android"
)

dotnet restore
dotnet build Contoso.Mobile.csproj -c $Configuration -f $Framework
dotnet publish Contoso.Mobile.csproj -c $Configuration -f $Framework `
    -p:AndroidPackageFormat=aab `
    -o artifacts/$Framework
```
# Example scripts

Sample PowerShell scripts (`scripts/build-android.ps1`, `scripts/build-ios.ps1`) ship with the repository. Update them with your solution names and keystore paths before use.

### 7.2 Bash pipeline snippet

```bash
#!/usr/bin/env bash
set -euo pipefail

dotnet restore
dotnet workload restore
dotnet format --verify-no-changes
dotnet build -c Release
dotnet test tests/Contoso.Mobile.Tests.csproj \
  --logger "trx;LogFileName=TestResults.trx"
```

---

## 8. Deploy automation (distribution services)

```bash
appcenter distribute release \
  --app Contoso/Mobile-Android \
  --file artifacts/net9.0-android/Contoso.Mobile-Signed.aab \
  --group "Beta Testers" \
  --release-notes "Sprint 24 build"
```

> **Heads-up:** App Center build and test retire on **March 31 2025**, with distribution sunsetting on **March 31 2026**. Treat the CLI as a migration bridge and plan to move to GitHub Actions, Azure Pipelines, or third-party device clouds.[^appcenter-retirement]

For long-term pipelines, script store uploads with:

- Apple: xcrun altool or App Store Connect API via Fastlane.
- Google Play: `bundletool build-apks` + Play Developer Publishing API.
- Microsoft Store: wingetcreate or Windows Partner Center submission APIs.

All of the publishing commands in Sections 5–6 can run inside GitHub Actions or Azure DevOps jobs; see the weather sample pipelines for ready-made YAML.[^devops-sample]

---

## 9. Troubleshooting

- **Workload install failures**: run `dotnet workload repair maui` and delete `%USERPROFILE%\.dotnet\workloadmetadata`.
- **Android signing errors**: ensure keystore path is absolute and passwords are provided via secure environment variables.
- **iOS build fails**: check `xcodebuild -showsdks`, ensure provisioning profile matches bundle identifier.
- **Missing platform tooling**: run `maui-check` (community tool) to verify environment dependencies.

---

## Checklist

- [ ] .NET 9 SDK & MAUI workload installed.
- [ ] `dotnet new` templates validated.
- [ ] Build scripts (PowerShell/Bash) committed.
- [ ] Publish commands tested for all target platforms.
- [ ] Artifact directories (`artifacts/`) added to `.gitignore`.
- [ ] CI pipelines executing CLI commands end-to-end.

---

## Further reading

- [dotnet workload install command (Microsoft Learn)](https://learn.microsoft.com/dotnet/core/tools/dotnet-workload-install)[^workload-cli]
- [Use the CLI to publish for Android](https://learn.microsoft.com/dotnet/maui/android/deployment/overview#publish)[^android-cli]
- [Publish a .NET MAUI iOS app using the command line](https://learn.microsoft.com/en-us/dotnet/maui/ios/deployment/?tabs=cli)[^ios-cli]
- [Publish a .NET MAUI Mac Catalyst app outside the Mac App Store](https://learn.microsoft.com/en-us/dotnet/maui/mac-catalyst/deployment/?tabs=cli)[^maccatalyst-cli]
- [Publish a .NET MAUI app for Windows with the CLI](https://learn.microsoft.com/en-us/dotnet/maui/windows/deployment/overview?tabs=cli)[^windows-cli]
- [App Center retirement plan](https://techcommunity.microsoft.com/t5/app-center-blog/app-center-retirement-plan/ba-p/4255567)[^appcenter-retirement]
- [DevOps for .NET MAUI sample pipelines (.NET Blog)](https://devblogs.microsoft.com/dotnet/devops-for-dotnet-maui/)[^devops-sample]

[^workload-cli]: Microsoft Learn, "dotnet workload install," updated 24 Aug 2024, accessed 29 Oct 2025.
[^android-cli]: Microsoft Learn, "Use the CLI to publish for Android," accessed 29 Oct 2025.
[^ios-cli]: Microsoft Learn, "Publish a .NET MAUI iOS app using the command line," accessed 3 Nov 2025.
[^maccatalyst-cli]: Microsoft Learn, "Publish a .NET MAUI Mac Catalyst app for distribution outside the Mac App Store," accessed 3 Nov 2025.
[^windows-cli]: Microsoft Learn, "Publish a .NET MAUI app for Windows with the CLI," accessed 3 Nov 2025.
[^appcenter-retirement]: Microsoft Tech Community, "App Center retirement plan," published 12 Sep 2024, accessed 3 Nov 2025.
[^devops-sample]: James Montemagno, "Getting Started with DevOps and .NET MAUI," .NET Blog, accessed 29 Oct 2025.





