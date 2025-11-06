---
title: Workstation System Requirements
description: Baseline hardware, OS, and platform prerequisites for delivering .NET 9 and .NET MAUI solutions.
last_reviewed: 2025-11-03
owners:
  - @prodyum/platform-engineering
---

# System Requirements and Hardware Recommendations

This guide summarises the minimum infrastructure required to deliver .NET 9 and .NET MAUI solutions end to end. The specifications stay aligned with Visual Studio, Android, and iOS release policies.

## 1. Supported operating systems

| Platform | Minimum | Recommended |
|----------|---------|-------------|
| **Windows** | Windows 10 20H2 (x64) | Windows 11 23H2 Enterprise with Hyper-V enabled |citeturn1search0|
| **macOS** | macOS 15.3 (Sequoia) | macOS 15.5+, Apple Silicon (M3 or newer) |citeturn0search4|
| **Linux/CI** | Ubuntu 22.04 LTS | Ubuntu 24.04 LTS-based Azure Linux 3.0 container runner |citeturn9search0|

> **Note:** Visual Studio for Mac reached end of life on 31 August 2024; use Visual Studio Code + C# Dev Kit with CLI workflows on macOS.citeturn5search0

## 2. Hardware baseline

- **CPU:** 6 physical cores (Intel i5 12th gen / Ryzen 7 7840 class) or better; prefer 8+ cores when compiling AOT builds.
- **RAM:** 32 GB for developer workstations, 64 GB for CI agents to accommodate Android/iOS emulators and Docker workloads.
- **Storage:** NVMe SSD with at least 1 TB free; Android and iOS SDK payloads consume roughly 200 GB.
- **GPU:** DirectX 11-capable GPU or recent integrated graphics; hardware acceleration must be enabled for the Android Emulator.citeturn1search0

## 3. Mobile platform requirements

- **Android:** Google Play mandates targeting Android 15 (API 35) starting 31 August 2025; install API 35 platform tools and emulator images on every machine.citeturn0search0
- **iOS/macOS:** .NET 9 iOS workloads require Xcode 16.4 and the iOS 18.5 SDK, which in turn needs macOS 15.3 or later.citeturn0search4
- **Hot Restart (Windows):** Install iTunes and authorise the Apple Developer account; Hot Restart fails until the device association is validated via iTunes.citeturn16search5

## 4. .NET and SDK alignment

- **.NET SDK:** Use the 9.0.306 STS release (supported through November 2026) and pin it with `global.json`.citeturn3search2turn3search0
- **Workload management:** Workload set mode is the default in .NET 9; `dotnet workload update --print-rollback` now emits JSON only-lock versions across the repository.citeturn11search2turn11search0
- **PowerShell:** Version 7.5 runs on .NET 9 with 18 months of support; install it as the default shell for scripts.citeturn7search2
- **Azure CLI:** Require 2.74+ to benefit from Azure Linux 3.0 support; Mariner 2.0 images reach end of life in July 2025.citeturn9search0turn13search4

## 5. CI/CD agent profiles

- **Windows build agent:** Windows Server 2022 with Visual Studio Build Tools 17.14, .NET SDK 9.0.306, PowerShell 7.5, Git 2.51, and the Android 15 SDK.citeturn10search1turn7search2turn8search6turn0search0
- **macOS build agent:** macOS 15.5+, Xcode 16.4, .NET 9 workload sets, and the fastlane CLI.citeturn0search4turn11search2
- **Linux container agent:** Azure Linux 3.0 (Ubuntu 24.04 base image) with Azure CLI 2.74+ and automated `dotnet workload install` synchronisation.citeturn9search0turn11search2

## 6. Health checklist

- [ ] `dotnet --info` reports 9.0.306 and the correct workload set version.citeturn3search2turn11search2
- [ ] `android list targets` lists API 35 packages.citeturn0search0
- [ ] `xcodebuild -version` returns 16.4+ and `xcrun simctl list` shows iOS 18.5 simulators.citeturn0search4
- [ ] `az version` >= 2.74 and `pwsh -v` >= 7.5.citeturn9search0turn7search2
- [ ] `git --version` reports 2.51 and `node -v` prints the 22.x LTS release.citeturn8search6turn7search6
- [ ] `python3 --version` is 3.14.0 or newer; virtual environments are refreshed to pick up the runtime.citeturn12search1

Review this checklist at the start of every sprint and update it alongside Android and Apple policy changes-Play Store and Xcode requirements advance at least twice per year.citeturn0search0turn0search4
