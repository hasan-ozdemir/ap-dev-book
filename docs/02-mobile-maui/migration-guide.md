---
title: Migration: Advanced Xamarin.Forms to .NET MAUI Upgrade Guide
description: Execute end-to-end migration projects from Xamarin.Forms to .NET MAUI with .NET 9, covering tooling, timelines, and risk mitigation.
last_reviewed: 2025-11-03
owners:
  - @prodyum/maui-guild
---

# Migration: Xamarin.Forms to .NET MAUI

Xamarin.Forms reached end of support on **May 1, 2024**, and Google and Apple deadlines now require Android API 34+ and iOS 18 SDK submissions—creating hard stops for un-migrated apps.citeturn2search2turn1search2turn3search5 This playbook helps senior engineers lead complex migrations to .NET MAUI with .NET 9 while maintaining release cadence and store compliance.

## Migration phases

| Phase | Key work | Tooling |
| --- | --- | --- |
| **Assessment** | Inventory projects, NuGet dependencies, renderers/effects, native bindings, build scripts. Identify platform gaps (notifications, background services, custom renderers). | Project audits, Dependency Graph, `dotnet-outdated`. |
| **Preparation** | Update Xamarin.Forms to 5.0, align target SDKs (Android 14 / iOS 17), and resolve obsolete APIs to reduce diff before migrating.citeturn1search4 | Visual Studio 17.10+, Xamarin diagnostic analyzers. |
| **Upgrade execution** | Run the .NET Upgrade Assistant to convert projects, or create a clean MAUI solution and copy assets when you need a single-project layout.citeturn1search6turn1search8 | `upgrade-assistant upgrade`, `.NET MAUI` project templates. |
| **Modernisation** | Replace renderers/effects with handlers, adopt `MauiProgram` DI, integrate Essentials/CommunityToolkit alternatives.citeturn1search1turn1search6 | .NET MAUI Community Toolkit, Handler mappings. |
| **Validation & release** | Execute automated/UI tests, store pre-checks (API 35 readiness, Xcode 16 builds), and stage rollouts. | App Center Device Manager, Google Play closed testing, App Store Connect phased release. |

## Tooling workflow

1. **Generate an upgrade report**  
   ```bash
   dotnet tool install --global upgrade-assistant
   upgrade-assistant upgrade MyLegacyXamarin.sln --extension
   ```  
   The report highlights incompatible TFMs, package replacements, and renderer usage so you can plan remediation before modifying source.citeturn1search6

2. **Create the MAUI shell**  
   ```bash
   dotnet new maui -n MyApp.Maui -f net9.0
   dotnet new gitignore
   ```  
   Using a clean template ensures SDK-style project settings, shared resource directories, and multi-target defaults that map to .NET 9 workloads.citeturn1search6

3. **Configure workloads**  
   ```bash
   dotnet workload install maui-android maui-ios maui-maccatalyst maui-windows
   dotnet workload update
   ```  
   Keep workloads current to pick up Android API level upgrades (API 35) and iOS SDK toolchains ahead of store deadlines.citeturn2search1turn2search0

4. **Port resources & XAML**  
   - Copy `Resources\Raw`, `Resources\Fonts`, and `Assets` into the MAUI `Resources` hierarchy.  
   - Merge `App.xaml` and `Shell` files, converting `Application.Current.Properties` usage to `Preferences` or secure storage equivalents.citeturn1search4

5. **Replace custom renderers**  
   Map each renderer to a handler using `Mapper.AppendToMapping`. When no handler exists, wrap native views with `PlatformView`. Telerik, Syncfusion, and CommunityToolkit components provide MAUI equivalents for common controls.citeturn1search1turn1search2

6. **Modernise DI & services**  
   Move service registration to `MauiProgram.CreateMauiApp()` and leverage Essentials APIs for sensors, storage, and secure credentials. Retire `DependencyService` once replacements are registered in the new service container.citeturn1search6turn1search1

7. **Automate testing & release**  
   - Run unit/UI suites on .NET 9 (Android Emulator, iOS simulator).  
   - Configure staged rollouts (Google Play) or phased releases (App Store) to monitor crash-free sessions before 100% rollout.citeturn3search6turn3search2

## Risk mitigation checklist

| Risk | Mitigation |
| --- | --- |
| **API support gaps** (Bluetooth, background services) | Validate Essentials coverage; plan native bindings via handlers or platform-specific projects. |
| **Dependency parity** | Audit third-party packages for MAUI versions; use compatibility shims or rewrite features if vendors have sunset Xamarin support.citeturn1search2 |
| **Performance regressions** | Enable AOT/LLVM where available, profile `CollectionView` virtualization, and migrate to handlers for layout-sensitive controls.citeturn1search2turn1search6 |
| **Store policy breaches** | Ensure Android target SDK 35 readiness and build iOS binaries with Xcode 16/iOS 18 SDK before April 24, 2025 submissions.citeturn3search5turn2search2 |

## Migration timeline template

- **Week 0–1:** Assessment, dependency audit, device matrix planning.
- **Week 2–3:** Upgrade assistant dry run, create MAUI shell, migrate shared projects.
- **Week 4–5:** Port custom renderers/effects, integrate Essentials, implement new DI.
- **Week 6:** Performance tuning, instrumentation, automated test hardening.
- **Week 7:** Beta/beta track release, phased rollout, telemetry validation.
- **Week 8:** Production rollout, post-release monitoring, retrospective capture.

## Deliverables

- Updated MAUI solution targeting `net9.0-android`, `net9.0-ios`, `net9.0-windows10.0.19041.0`, and `net9.0-maccatalyst`.
- Architecture decision record summarising handler migration strategy and dependency outcomes.
- Rollback plan for Xamarin.Forms branch (frozen after security patches only) with store submission blackout dates logged.citeturn2search2turn3search5

--- 

**Next:** Dive into the [.NET MAUI Platform SDK playbooks](./platform-sdk/index.md) to map remaining native feature work.
