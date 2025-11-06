---
title: .NET MAUI Lifecycle in Visual Studio
description: Build, debug, profile, and publish MAUI apps end-to-end using Visual Studio 2022.
last_reviewed: 2025-10-29
owners:
  - @prodyum/maui-guild
---

# .NET MAUI Lifecycle in Visual Studio

Visual Studio 2022 (17.10 and later) delivers an integrated .NET MAUI experience across Windows and macOS—from workload installation through publishing to stores. This guide walks through each stage and highlights .NET 9 enhancements you should adopt.

---

## 1. Environment setup

### 1.1 Workload installation

- Launch the Visual Studio Installer → **Modify** → **.NET Multi-platform App UI development** (includes Android, iOS, Mac Catalyst, Windows).citeturn0search0
- On macOS, install Xcode 16.1+ and accept licenses; Visual Studio for Mac is retired, so use Visual Studio 2022 on Windows + networked Mac build host or remote connection.
- Install Android SDK platforms (API 35+) and emulators via **Tools → Android → Android SDK Manager**.citeturn0search1

### 1.2 Device/emulator provisioning

- Android: Create virtual devices in **Device Manager**; enable “Cold boot now” for reliable runs.
- iOS: Pair a Mac or use Xcode simulators; update automatic signing certificates.
- Windows: Ensure Windows App SDK runtime is installed (Visual Studio prompts when required).

---

## 2. Project creation & structure

1. **File → New → Project → .NET MAUI App**; choose `.NET 9` target frameworks.
2. Solution structure includes `Platforms` folder, shared XAML/CS files, `Resources` for fonts/images/raw assets, and `MauiProgram.cs` for dependency injection.
3. Use item templates (`Add → New Item → .NET MAUI ContentPage`, `Resource Dictionary`, etc.) to scaffold UI/pages quickly.

---

## 3. Build & debug

### 3.1 Hot Reload & Live Visual Tree

- Hot Reload refreshes XAML and C# changes without rebuilding. .NET 9 improves incremental build speed and supports most view-model updates.citeturn0search0turn0search4
- Live Visual Tree and Live Property Explorer let you inspect the UI hierarchy in real time; available for Android, iOS, Windows.

### 3.2 Run configurations

- Select target from the run dropdown (e.g., Android emulator, Windows machine, iOS simulator).
- Use **Run → Debug Properties** to configure environment variables, arguments, or custom MSBuild properties.
- Enable **Hot Restart** to deploy iOS apps to devices using Windows (requires Apple developer account).

### 3.3 Diagnostics

- **App diagnostics**: `View → Other Windows → Diagnostic Tools` for CPU/memory profiling during debug sessions.
- **Performance Profiler** (`Alt` + `F2`): Run instrumentation for CPU usage, memory consumption, network, UI responsiveness.
- **.NET Counter Monitor**: Visual Studio 17.10+ integrates `dotnet-counters` for real-time metrics.

---

## 4. Testing inside Visual Studio

- **Unit & integration tests**: Add xUnit/NUnit projects; run via Test Explorer. Enable Live Unit Testing for immediate feedback.
- **UI tests**: Use Playwright/Appium projects and trigger through command-line tasks integrated into Visual Studio (see Test Strategies module).
- **Snapshot testing**: Integrate with frameworks like Verify.Xunit to capture UI state.

---

## 5. Dev tunnels & remote testing

- Use **Dev Tunnels** (Preview) to expose localhost endpoints for device testing without USB or LAN constraints.
- For cross-team review, share the running app via **Live Share**, allowing collaborators to inspect, debug, and co-edit.

---

## 6. Packaging & publishing

### 6.1 Windows (MSIX)

1. Right-click project → **Publish → Create App Packages**.
2. Choose **MSIX**; configure signing certificate (.pfx) and identity.
3. Generate packages for sideloading or Microsoft Store submission.

### 6.2 Android (APK/AAB)

1. Set configuration to `Release`.
2. Update `AndroidManifest.xml` package name and version.
3. **Build → Archive**; use Android App Bundle for Play Store. Configure keystore in **Project Properties → Android Package Signing**.

### 6.3 iOS (IPA)

1. Connect to a Mac with Xcode 16.1+.
2. Ensure provisioning profile & signing certificate are installed.
3. **Build → Archive**; export IPA for TestFlight or App Store Connect.

### 6.4 Mac Catalyst

1. Switch target to `net9.0-maccatalyst`.
2. Provide signing identities and entitlements.
3. Publish as `.pkg` or submit via Transporter app to the Mac App Store.

For automated distribution, integrate Azure DevOps or GitHub Actions with the Visual Studio-generated artifacts (see DevOps modules).

---

## 7. Tips & troubleshooting

- **Hot Reload limitations**: Changes to generics or project file modifications still require rebuilds. Keep Hot Reload enabled but know when to restart.
- **Android emulator performance**: Use hardware acceleration (Hyper-V/WSA) and allocate sufficient RAM.
- **iOS pairing issues**: Reset network connection, ensure Xcode command-line tools are installed, and confirm the same Apple ID account.
- **Windows App SDK runtime mismatch**: Update via Visual Studio installer or use `winget install Microsoft.WindowsAppRuntime.1.5`.

---

## 8. Workflow checklist

- [ ] Workload + platform SDKs installed.
- [ ] Emulator/simulator/device registered.
- [ ] Hot Reload + Live Visual Tree enabled.
- [ ] Diagnostics/profile tools validated.
- [ ] Publish profiles per platform configured.
- [ ] Signing assets (keystore, certificates) stored securely.

Use this checklist at engagement kick-off to ensure developers have a consistent Visual Studio setup.

---

## Further reading

- Visual Studio MAUI feature roadmap and release notes.citeturn0search0turn0search4
- Android & iOS setup guides for MAUI in Visual Studio.citeturn0search1turn0search5
