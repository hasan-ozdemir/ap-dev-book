---
title: .NET MAUI in Visual Studio Code
description: Configure the .NET MAUI VS Code extension, develop, debug, and deploy MAUI apps with a lightweight toolchain.
last_reviewed: 2025-10-29
owners:
  - @prodyum/maui-guild
---

# .NET MAUI in Visual Studio Code

The .NET MAUI extension for Visual Studio Code brings MAUI development to a lightweight, cross-platform editor by extending the C# Dev Kit with XAML tooling, templates, and debugging support. The extension is actively updated (v1.10.16 published 7 May 2025) and continues to evolve in preview.citeturn0search0turn0search8

> **Prerequisite reminder:** Visual Studio for Mac retired on 31 August 2024. Developers on macOS should rely on VS Code plus the MAUI extension or use Visual Studio on Windows with a paired Mac build host.citeturn0search9

---

## 1. Install & update tooling

1. **Install C# Dev Kit** (`ms-dotnettools.csdevkit`) from the VS Code Marketplace.
2. Install the **.NET MAUI extension** (`ms-dotnettools.dotnet-maui`). Confirm version ≥ 1.10.16 to ensure .NET 9 support.citeturn0search0
3. Verify .NET workloads:
   ```bash
   dotnet --list-sdks
   dotnet workload install maui
   dotnet workload update
   ```
4. On macOS, install Xcode 16.1 or newer and accept licenses; on Windows, set up the Android SDK via `winget install Microsoft.AndroidSDK.*` or Visual Studio Installer.
5. Optional: add vendor extensions (e.g., Syncfusion .NET MAUI Extension) for component templates.citeturn0search6turn0search10turn0search11

---

## 2. Project scaffolding

### 2.1 Command palette flow

1. `Ctrl/Cmd+Shift+P` → **.NET MAUI: Create Project**.
2. Choose template (`.NET MAUI App`, `.NET MAUI Blazor App`, or vendor-provided templates).
3. Select target frameworks (net9.0-android; net9.0-ios; net9.0-maccatalyst; net9.0-windows10.0.19041.0).
4. Choose location and optional Git initialization.

### 2.2 CLI flow

```bash
dotnet new maui -n Contoso.Mobile -f net9.0-android
code Contoso.Mobile
```

Use multi-targeting by editing the project `TargetFrameworks` property. Commit `Directory.Build.props` for shared settings.

---

## 3. Workspace configuration

### 3.1 `.vscode` folder

- `settings.json`:
  ```json
  {
    "dotnet.defaultSolution": "Contoso.Mobile.sln",
    "csharp.suppressGoToDefinitionWarning": true,
    "maui.android.adbPath": "/Users/you/Library/Android/sdk/platform-tools/adb"
  }
  ```
- `tasks.json`:
  ```json
  {
    "version": "2.0.0",
    "tasks": [
      {
        "label": "dotnet build",
        "type": "shell",
        "command": "dotnet build Contoso.Mobile.sln",
        "group": "build",
        "problemMatcher": "$msCompile"
      },
      {
        "label": "dotnet format",
        "type": "shell",
        "command": "dotnet format",
        "group": "test"
      }
    ]
  }
  ```
- `launch.json` includes per-platform configurations (Android emulator, Windows, iOS simulator paired over network).

### 3.2 Dev Containers & Codespaces

- Add `.devcontainer/devcontainer.json` with .NET 9 SDK, Android tools, and MAUI workloads.
- Use GitHub Codespaces (preview) for remote development; install VS Code MAUI extension inside the container.

---

## 4. Build & run

### 4.1 Android

```bash
dotnet build -t:Run -f net9.0-android
```

Select the device from the status bar; use `adb devices` to confirm connection.

### 4.2 Windows

```bash
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

Ensure Windows App SDK runtime installed. Running requires Windows 11 or Windows 10 19041+.

### 4.3 iOS / Mac Catalyst

```bash
dotnet build -t:Run -f net9.0-ios
```

Requires paired Mac with Xcode 16.1+. Use `maui doctor` for diagnostics.

---

## 5. Debugging & diagnostics

- **MAUI debugger:** VS Code uses the new Mono debug engine (May 2025 update) for richer stepping and improved symbol loading.citeturn0search7
- **Hot reload:** Enable `dotnet watch` (`Terminal → Run Build Task → dotnet watch`) to refresh XAML/C# changes. Preview supports reload for stateful pages; some scenarios still require rebuild.
- **In-app logging:** Add `builder.Logging.AddDebug()` in `MauiProgram`. View logs in the Debug Console.
- **Profiling:** Launch external tools (`dotnet-counters`, `dotnet-trace`) from integrated terminal.

---

## 6. Testing & quality hooks (VS Code)

- Integrate unit tests via `dotnet test`; expose as VS Code Test Explorer by installing `.NET Test Explorer` extension.
- UI tests: use Playwright (.NET) or Appium; configure scripts in `tasks.json` and pipeline YAML.
- Static analysis: run `dotnet format analyzers` or `dotnet build -warnaserror` from terminal; watch the Problems panel for analyzer output.

---

## 7. Distribution

- **Android**: `dotnet publish -f net9.0-android -c Release -p:AndroidPackageFormat=aab`.
- **iOS**: `dotnet publish -f net9.0-ios -c Release -p:ArchiveOnBuild=true`.
- **Windows**: `dotnet publish -f net9.0-windows10.0.19041.0 -c Release -p:WindowsPackageType=MSIX`.
- Use VS Code’s integrated terminal or tasks to automate store submissions via CLI (e.g., `appcenter distribute release`).

---

## 8. Troubleshooting

- **Extension not detected**: run `code --list-extensions | find "ms-dotnettools.dotnet-maui"`; reinstall if missing.
- **Android emulator slow**: enable hardware acceleration, check `~/.android/advancedFeatures.ini`.
- **Hot reload fails**: ensure `dotnet watch` uses .NET 9 SDK; restart extension host after updates.
- **Xcode pairing issues**: confirm `xcode-select --install` and `sudo xcodebuild -license` accepted.
- **Preview limitations**: review GitHub issue tracker linked from the extension announcement for known gaps.citeturn0search8

---

## Checklist

- [ ] .NET 9 SDK & MAUI workload installed.
- [ ] VS Code C# Dev Kit + MAUI extension up to date.
- [ ] Android/iOS tooling configured (ADB, Xcode).
- [ ] Workspace tasks/launch settings committed.
- [ ] Hot reload and debugging verified on primary platforms.
- [ ] Publish scripts tested for Release builds.

Keep this checklist in your repository wiki or onboarding docs to standardise the VS Code MAUI setup across the team.

---

## Further reading

- .NET MAUI extension release updates and feedback channel.citeturn0search8
- Marketplace VSIX listings for MAUI extension (version history).citeturn0search0turn0search3
- Syncfusion MAUI VS Code integration guides.citeturn0search6turn0search10turn0search11
