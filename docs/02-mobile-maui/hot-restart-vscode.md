---
title: VS Code Hot Restart Guide
description: Set up VS Code with MAUI workloads, .NET CLI, and Remote iOS Debugging for Hot Restart on Windows and macOS.
last_reviewed: 2025-10-31
owners:
  - @prodyum/maui-guild
---
# VS Code Hot Restart Guide

This guide explains how to configure Visual Studio Code to use Hot Restart when targeting iOS devices from Windows. It complements the Visual Studio workflow by giving teams a lightweight editor option for rapid MAUI iteration.

---

## 1. Prerequisites

| Platform | Requirements | Notes |
| --- | --- | --- |
| Windows 10/11 | .NET SDK 9.0, Visual Studio Code with the C# Dev Kit and .NET MAUI extensions, Apple Developer Program membership, and Microsoft Store iTunes drivers | The MAUI extension (built on C# Dev Kit) enables MAUI templating and device targets inside VS Code, while Hot Restart still depends on Apple drivers supplied by the Store version of iTunes.citeturn5view0turn6view0turn11view0 |

Hot Restart still requires a 64-bit iOS device connected over USB and unlocked.citeturn11view0

---

## 2. Install required VS Code extensions

1. Install **C# Dev Kit** (`ms-dotnettools.csdevkit`) to unlock project/workspace tooling in VS Code.citeturn6view0
2. Install the **.NET MAUI** VS Code extension (`ms-dotnettools.maui-vscode-extension`) so you can scaffold, debug, and deploy MAUI apps from the editor.citeturn5view0
3. Reload VS Code when prompted so the extensions initialise and register launch/task templates.citeturn5view0turn6view0

---

## 3. Configure the development environment

### 3.1 Install MAUI workloads

```powershell
dotnet workload install maui
dotnet workload install maui-ios
```

These commands install the MAUI workload set for .NET 9 so the CLI can build the iOS head from Windows.citeturn7search0

### 3.2 Sign in to Apple Developer

```powershell
dotnet workload configure --accept-sign-agreements
```

The configuration step accepts licence agreements and prepares provisioning assets that the MAUI tooling needs to deploy to iOS devices.citeturn7search1

Ensure the iOS device is trusted in iTunes before launching the debugger so Windows surfaces it to VS Code.citeturn11view0

---

## 4. Launching Hot Restart from VS Code

1. Open the MAUI solution in VS Code; the C# Dev Kit view organises projects and surfaces MAUI dashboards/targets.citeturn6view0
2. In **Run and Debug**, pick the `MAUI iOS Local Device (Hot Restart)` configuration emitted by the MAUI extension.citeturn5view0
3. Press **F5**. VS Code deploys via the Hot Restart shell bundle and attaches the debugger once the app launches on the connected device.citeturn5view0turn11view0
4. Launch the app manually on first run and tap **Trust** when prompted; subsequent deployments reuse the Hot Restart shell for faster inner loops.citeturn11view0

---

## 5. Iteration tips

- Because Hot Restart reuses the deployed shell, rebuild-and-run loops from VS Code complete in seconds after the initial deployment.citeturn11view0
- Use the MAUI extension's debugger controls or CLI tasks (for example `dotnet build -t:Run -f net9.0-ios`) when you need to verify other device targets alongside Hot Restart.citeturn5view0turn7search0
- Switch to Visual Studio on Windows or Pair to Mac when you need features that require a full Mac build host (release builds, asset catalog validation, TestFlight packaging).citeturn11view0

---

## 6. Troubleshooting

| Issue | Resolution |
| --- | --- |
| Device not detected | Confirm the device appears in iTunes, unlock it, and tap **Trust** so Windows exposes it to VS Code.citeturn11view0 |
| App terminates before debugger attaches | Minimise startup work or test on newer hardware; the iOS watchdog can kill debug builds delivered via Hot Restart.citeturn11view0 |
| Need to escalate | Use **Help > Send Feedback > Report a Problem** or the MAUI extension's marketplace issue link to share logs with Microsoft.citeturn5view0turn11view0 |

---

## 7. Best practices

- Commit `.vscode/launch.json` and `.vscode/tasks.json` Hot Restart configurations with the repo so teammates can run from VS Code immediately.citeturn6view0
- Keep an automation path (Azure DevOps, GitHub Actions, or local Mac builds) for notarised or App Store-ready packages, because Hot Restart only produces debug builds.citeturn11view0

> ℹ️ As of October 31, 2025 we could not locate public Microsoft guidance on sharing Apple Developer credentials for VS Code-based Hot Restart. Document any team-specific process internally and review it regularly.
