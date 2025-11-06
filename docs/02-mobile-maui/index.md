---
title: .NET MAUI Delivery
description: Master the patterns, tooling, and checklists required to deliver production-ready mobile apps with .NET MAUI.
last_reviewed: 2025-10-29
owners:
  - @prodyum/maui-guild
---

# .NET MAUI Delivery

Prodyum expects senior engineers to own the full .NET MAUI delivery cycle because MAUI ships Android, iOS, macOS, and Windows apps from a single project with shared UI and logic.citeturn1search2
The practices documented here codify Prodyum standards while remaining a public resource that any mobile engineer can apply to accelerate high-quality .NET MAUI deliveries.

## Learning path

1. **Project Setup:** Create projects with dotnet new maui, manage multi-targeting in Visual Studio, and align with Prodyum templates for consistent project structure and assets.citeturn7search0turn2search1
2. **UI Architecture:** Apply MVVM, Shell navigation, and responsive XAML layouts to keep shared code maintainable across form factors.citeturn1search2turn7search5
3. **Platform Integration:** Use .NET MAUI Essentials and platform handlers to access device capabilities such as sensors, secure storage, notifications, and geolocation.citeturn7search4
4. **Performance:** Profile with the MAUI diagnostics tooling, enable ahead-of-time (AOT) or native AOT publishing, and monitor memory allocations introduced in .NET 9.citeturn8search2turn2search1
5. **Quality & Release:** Combine unit/UI testing guidance with store submission checklists so every build meets analytics, policy, and telemetry requirements.citeturn7search6turn2search1

Each module includes prerequisites, hands-on labs, and links back to Azure, DevOps, and operations guidance for an integrated experience across the programme.citeturn2search1turn7search6

> **Keep it current:** Revisit this area each quarter to capture new .NET MAUI releases, OS store policy changes, and lessons learned from retrospectives.citeturn1search6turn1search2

## Available guides

- [Project Setup & Scaffolding](./project-setup.md)
- [UI Components & Experience Design](./ui-components.md)
- [MAUI Control Catalog](./ui-controls/index.md)
- [Data Access & Offline Sync](./data-access.md)
- [Essentials Companion](./essentials-guide.md)
- [.NET MAUI Community Toolkit Playbook](./community-toolkit.md)
  - [Foundation & Setup](./community-toolkit.md#1-foundation-setup)
  - [UI Building Blocks](./community-toolkit.md#2-ui-building-blocks-and-recipes)
  - [Integration Patterns](./community-toolkit.md#3-integration-patterns)
  - [Upgrade Checklist](./community-toolkit.md#4-upgrade-and-quality-checklist)
  - [Further Learning](./community-toolkit.md#5-further-learning)
- [Platform SDK Playbooks](./platform-sdk/index.md)
  - [iOS Platform SDK](./platform-sdk/ios-sdk-guide.md)
  - [Android Platform SDK](./platform-sdk/android-sdk-guide.md)
  - [Windows Platform SDK](./platform-sdk/windows-sdk-guide.md)
- [Dependency Injection & Platform Services](./dependency-injection.md)
- [Performance Optimization](./performance-optimization.md)
- [Testing Methodologies Playbook](./testing-methods.md)
- [Testing & Quality Assurance](./testing-quality.md)
- [Migrating Custom Renderers & Platform Code](./migration-custom-renderers.md)
- [Building & Linking App Packages](./app-packaging-linking.md)
- [App Lifecycle: Develop to Publish](./app-lifecycle.md)
- [Authentication & Authorization](./authentication-authorization.md)
- [Local Storage & Offline Data](./local-storage-offline.md)
- [Xamarin.Forms to .NET MAUI Migration Guide](./xamarin-to-maui.md)
- [Visual Studio Workflow](./vs-workflow.md)
- [Visual Studio Hot Restart Guide](./hot-restart-visual-studio.md)
- [VS Code Workflow](./vscode-workflow.md)
- [VS Code Hot Restart Guide](./hot-restart-vscode.md)
- [CLI Workflow](./cli-workflow.md)
- [Testing & Quality Strategy](./testing-strategies.md)
- [Migration: Xamarin.Forms to MAUI](./migration-guide.md)
- [Platform Integration & Device APIs](./platform-apis.md)
- [Mobile AI Blueprint Catalog](../08-mobile-ai-blueprints/index.md)
