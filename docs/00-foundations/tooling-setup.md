---
title: Visual Studio & CLI Setup
description: Configure cross-platform tooling for .NET MAUI, Azure, and related workloads on Windows and macOS.
last_reviewed: 2025-10-29
owners:
  - @prodyum/maui-guild
---

# Visual Studio & CLI Setup

Prodyum standardises on Visual Studio 2022 version 17.14 (Current Channel) with the .NET 9.0.305 SDK so local machines, build agents, and production pipelines share the same Standard Term Support toolchain and language features.citeturn6search2turn7search1

## 1. Install the required SDKs

| Tool | Baseline | Notes |
|------|----------|-------|
| .NET SDK | 9.0.305 (STS) | Bundled with Visual Studio 17.14.14 and delivers the .NET 9/C# 13 toolset used by MAUI projects.citeturn6search2turn7search1 |
| Visual Studio 2022 (Windows) | 17.14 Current Channel | Adds design-time XAML Live Preview and MAUI debugger improvements that accelerate UI iteration and diagnostics.citeturn6search0turn6search2 |
| Xcode | 16.4 or later | Required for .NET 9 iOS/macOS workloads; Xcode 16.4 mandates macOS 15.3+ and ships the iOS 18.5 SDK.citeturn10search3turn10search5 |
| Android SDK & platform tools | Android 15 (API 35) | Target API level 35 is mandatory for new Google Play submissions after 31 Aug 2025, so install the Android 15 SDKs and emulators.citeturn8search0 |
| Azure CLI | 2.66 STS or later | Azure CLI 2.66 introduces the current STS support window and Azure Linux 3.0 container images--keep local installs aligned to avoid lifecycle gaps.citeturn9search0turn9search6turn9search9 |

> **Tip:** Match the exact SDK patch (9.0.305) that your CI agents use so `dotnet restore`, workload manifests, and global.json agree across environments.citeturn6search2

### Verify installation

```powershell
dotnet --info
dotnet workload list
az version
```

Confirm that `.NET SDK` lists 9.0.305 or newer, the `maui` workload appears in `dotnet workload list`, and `az version` reports an STS build (2.66.x or later).citeturn11search7turn9search0

## 2. Configure Visual Studio 2022 (Windows)

1. Launch the Visual Studio Installer and choose **Modify** for version 17.14.
2. Under **Desktop & Mobile**, select **.NET Multi-platform App UI development** plus supporting workloads you rely on (e.g., ASP.NET and Azure development) to provision Android, iOS, Mac Catalyst, and Windows tooling in one step.citeturn11search8
3. After installation, open **Tools -> Get Tools and Features** to confirm all components show **Installed**.

### Enable XAML Live Preview & Hot Reload

Visual Studio 17.14 enables design-time XAML Live Preview, Hot Reload, and Copilot-assisted diagnostics for MAUI projects--enable these under **Tools -> Options -> Debugging -> Hot Reload** to shorten UI build loops.citeturn6search0

### Configure Android emulators

1. Open **Android Device Manager** and create virtual devices targeting Android 15 (API 35) and Android 14 (API 34) to satisfy Google Play requirements for 2025 submissions.
2. Enable hardware acceleration (Hyper-V, Intel HAXM, or AMD equivalent) on Windows hosts before launching the emulators.citeturn8search0

## 3. Configure macOS build stations

Visual Studio for Mac is retired (31 Aug 2024), so macOS contributors use the CLI and third-party IDEs with MAUI workloads.citeturn5search0

1. Install Xcode 16.4 from the Mac App Store or Apple Developer portal, then accept the license:
   ```bash
   sudo xcode-select --switch /Applications/Xcode.app
   sudo xcodebuild -runFirstLaunch
   ```
2. Install the .NET SDK and MAUI workload:
   ```bash
   brew install --cask dotnet-sdk
   dotnet workload install maui
   ```
3. Sign in with the team Apple Developer account under **Xcode -> Settings -> Accounts** so provisioning profiles sync before building iOS or Mac Catalyst binaries.
4. Optional: Use Rider, VS Code with the C# Dev Kit, or other IDEs once the workload is in place.

Xcode 16.4 and the .NET 9 iOS/macOS SDKs require macOS 15.3 or later; stay current to avoid toolchain mismatches.citeturn10search3turn10search5

## 4. Set up the .NET MAUI workload

Install or repair the workload after every SDK upgrade to keep manifests aligned:

```powershell
dotnet workload install maui
dotnet workload repair
```

The CLI workload commands are the supported method for provisioning MAUI tooling outside Visual Studio, and `dotnet workload list` verifies the manifest versions in use.citeturn2search0turn11search7

## 5. Install cross-platform build dependencies

- **Windows:** Install the Azure CLI with winget and ensure global PowerShell modules inherit the updated PATH.
  ```powershell
  winget install --id Microsoft.AzureCLI -e
  ```
- **macOS:** Homebrew provides an Azure CLI formula that stays current with STS releases.
  ```bash
  brew update && brew install azure-cli
  ```
- **Shared:** Use the Azure Linux-based container image when running Azure CLI in Docker (`mcr.microsoft.com/azure-cli:azurelinux3.0`) so automation matches the supported runtime.citeturn12search0turn12search7turn9search6

## 6. Connect to Prodyum services

1. **GitHub Enterprise:** Generate SSH keys with `ssh-keygen -t ed25519` and register them in your GitHub profile to avoid credential prompts.
2. **Azure subscription:** Sign in and scope defaults with the CLI:
   ```powershell
   az login --use-device-code
   az account set --subscription "Prodyum Delivery"
   az configure --defaults location=westeurope group=ap-shared-rg
   ```
   The device-code login flow satisfies MFA policies across Windows, macOS, and remote shells.citeturn13search0
3. **Private package feeds:** Add the feed endpoints to `%APPDATA%\NuGet\NuGet.Config` (Windows) or `~/.nuget/NuGet/NuGet.Config` (macOS) so restore operations resolve Prodyum packages.

## 7. Validate the environment

Run a smoke test after cloning the repository:

```powershell
dotnet --info
dotnet workload list | Select-String "maui"
dotnet new maui -n ToolingValidation
dotnet build ToolingValidation/ToolingValidation.csproj -t:Run -f net9.0-android
az account show --query tenantId
```

Delete the temporary project once the build succeeds. Keep these commands handy whenever you upgrade SDKs, workloads, or emulator images to ensure your workstation still matches the portal baseline.citeturn11search7turn9search0
