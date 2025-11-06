---
title: IDE & Editor Setup
description: Configure Visual Studio, VS Code, and supporting extensions for .NET 9 and .NET MAUI delivery.
last_reviewed: 2025-11-03
owners:
  - @prodyum/platform-engineering
---

# IDE and Editor Configuration

This guide shows how to configure Visual Studio 2022, Visual Studio Code, and companion tooling to match Prodyum standards. The goal is a productive environment capable of building MAUI, web, and cloud projects on a single workstation.

## 1. Visual Studio 2022 (Windows)

### 1.1 Installation

1. Install **Visual Studio 2022 17.14 (Current Channel)** via the Visual Studio Installer.
2. Select the **.NET Multi-platform App UI development** workload; Android, iOS, Mac Catalyst, and WinUI targets ship with this bundle.?cite?turn10search1?
3. Add supporting workloads: **ASP.NET and web development**, **Azure development**, **.NET Desktop development**—keeping parity with our build agents.?cite?turn10search1?

### 1.2 Critical components

- **XAML Live Preview & Hot Reload:** Version 17.14 delivers refreshed inspectors and layout updates; enable them under **Tools → Options → Debugging → Hot Reload**.?cite?turn2search0?
- **AI-assisted debugging:** Event breakpoints, performance tips, and Copilot-powered insights simplify complex UI troubleshooting.?cite?turn10search1?
- **Android Emulator:** From **Tools → Android → Android SDK Manager** install API 35 packages and hardware acceleration components to stay compliant with Google Play policy.?cite?turn1search0?turn0search0?

### 1.3 Hot Restart and device management

To deploy iOS apps from Windows with Hot Restart:

- Install Apple iTunes and Apple Mobile Device Support.?cite?turn16search5?
- Connect the device over USB, enable developer mode, and authorise with your Apple ID.?cite?turn16search5?
- Enable Hot Restart via **Tools → Options → Xamarin → iOS Settings**.

## 2. Visual Studio Code (Windows & macOS)

Visual Studio for Mac retired on 31 August 2024, so MAUI development on macOS relies on VS Code plus CLI workflows.?cite?turn5search0?

### 2.1 Recommended extensions

- **C# Dev Kit** – provides the Roslyn language server and project-system integration.?cite?turn17search0?
- **.NET MAUI Extension for VS Code** – delivers XAML IntelliSense, Live Preview, and debugging profiles.?cite?turn17search0?
- **Azure Tools**, **GitHub Actions**, and **Docker** – recommended for DevOps and cloud workflows.

### 2.2 Daily workflow

```bash
dotnet workload install maui
code .
```

- Press `F5` to start debugging; VS Code selects the .NET Debugger for MAUI profile.?cite?turn17search0?
- Building for iOS still requires Xcode 16.4+ and an Apple Developer account on macOS.?cite?turn0search4?

## 3. CLI-focused daily tasks

| Command | Purpose |
|---------|---------|
| `dotnet new maui -n Contoso.Mobile` | Creates a project from the standard MAUI template.?cite?turn6search0? |
| `dotnet workload update --print-rollback` | Generates the rollback manifest that locks .NET 9 workload set versions.?cite?turn11search0? |
| `maui-check --ci` | Audits MAUI dependencies (workload repair).
| `android emulator -avd Pixel_8_Android_15` | Launches the API 35 emulator.?cite?turn0search0? |
| `xcodebuild -scheme ContosoMobile -configuration Debug` | Validates iOS builds.?cite?turn0search4? |

## 4. Team-standard checklist

- [ ] Visual Studio 2022 17.14 installed with the `.NET Multi-platform App UI` workload.?cite?turn10search1?
- [ ] VS Code has the C# Dev Kit and .NET MAUI extensions.?cite?turn17search0?
- [ ] Android SDK Manager downloaded API 35 packages.?cite?turn0search0?
- [ ] Xcode 16.4+ is available and `dotnet --info` reports 9.0.306.?cite?turn0search4?turn3search2?
- [ ] Apple Mobile Device Support is installed and the device is authorised for Hot Restart.?cite?turn16search5?

With this configuration, the same codebase builds iOS, Android, Windows, and Mac Catalyst targets on both Windows and macOS while producing artefacts ready for CI/CD pipelines.
