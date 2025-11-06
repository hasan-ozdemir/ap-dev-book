---
title: .NET Ecosystem Essentials
description: Understand the platform components, workload management, and release cadence that underpin Prodyum solutions.
last_reviewed: 2025-10-29
owners:
  - @prodyum/platform-architecture
---

# .NET Ecosystem Essentials

Prodyum ships mobile, web, and desktop experiences from a single .NET codebase, so we monitor Microsoft's modern lifecycle dates and release guidance to keep solutions on supported stacks.citeturn0search0turn0search1

## 1. Know the release train

| Release | Support type | End of support | Notes |
|---------|--------------|----------------|-------|
| .NET 9 | Standard-term (STS) | 2026-11-10 | Adds native AOT refinements, new MAUI diagnostics, and a community-first app template bundled with Syncfusion assets.citeturn0search0turn1search6 |
| .NET 8 | Long-term (LTS) | 2026-11-10 | Preferred baseline when long-lived support is required.citeturn0search0 |
| .NET 6 | Long-term (LTS) | 2024-11-12 | Migrate remaining workloads; security updates have concluded.citeturn0search0 |

> **Decision point:** Start new greenfield MAUI and cloud workloads on .NET 9, but keep shared libraries aligned with .NET 9 or the .NET 8 LTS to avoid split stacks across mobile and server tiers.citeturn0search0turn1search6

## 2. Understand workload manifests

.NET manages optional SDK experiences (for example .NET MAUI and Blazor AOT) through the workload CLI. Use these commands to keep developer machines and build agents aligned:citeturn4search0turn4search4

``powershell
dotnet workload search maui
dotnet workload update
dotnet workload repair
``

Adopt workload sets (introduced in .NET 8.0.400+) to pin entire teams to the same manifest versions and audit drift with dotnet workload history in .NET 9.citeturn4search1turn4search2

## 3. Platform building blocks

- **.NET base platform:** .NET 9 delivers performance, trimming, and AOT gains across the stack, plus new templates that wire up recommended MVVM, database, and navigation practices.citeturn1search6
- **.NET MAUI:** Release train investments add HybridWebView improvements, layout diagnostics via ActivitySource/Meter, and quality fixes for CollectionView/CarouselView.citeturn1search0turn1search2turn1search6
- **Blazor hybrid:** Updated templates and HybridWebView request interception make it easier to compose native and web UI inside MAUI shells.citeturn1search2turn1search6
- **ASP.NET Core back ends:** Native AOT support, trimming enhancements, and SignalR optimisations in .NET 9 keep mobile API tiers fast and hosting-friendly.citeturn1search10turn1search6

## 4. Toolchain interoperability

| Scenario | Recommended tool | Why |
|----------|------------------|-----|
| Mobile debugging | Visual Studio 2022 v17.14 + Android/iOS emulators | Live Preview, Hot Reload, and the updated MAUI debug engine streamline device loops.citeturn2search1 |
| Backend API development | dotnet CLI + Visual Studio Code | .NET 9 SDK tooling adds deeper MSBuild integration, parallel test runs, and richer terminal logging.citeturn3search1 |
| Cross-platform automation | PowerShell 7.5 and bash | PowerShell 7.5 (built on .NET 9) is supported on Windows, macOS, and Linux, keeping script parity across environments.citeturn5search0 |

## 5. Package management strategy

- Use public feeds (NuGet.org) for external packages and internal feeds (GitHub Packages or Azure Artifacts) for Prodyum components so workload sets and package baselines stay aligned.citeturn4search1turn4search4
- Lock dependency versions centrally (Directory.Packages.props) and update them in tandem with workload-set revisions to avoid drift between SDK manifests and package versions.citeturn4search1turn4search2

## 6. Configuration and secrets

- Use dotnet user-secrets (or IDE integrations) for local development credentials so secrets stay out of source control.citeturn6search0turn6search4
- Promote to managed vaults (for example Azure Key Vault) and environment-specific configuration providers once code lands in CI/CD or production slots.citeturn6search4

## 7. Observability expectations

- Instrument MAUI clients with the new ActivitySource/Meter diagnostics and surface traces through OpenTelemetry dashboards (including .NET Aspire).citeturn1search0turn5search3
- Align API back ends with the same OpenTelemetry exporters so Application Insights and Log Analytics capture correlated device-to-cloud transactions.citeturn1search0turn5search3
