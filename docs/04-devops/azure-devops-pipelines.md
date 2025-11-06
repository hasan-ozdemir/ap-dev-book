---
title: Azure DevOps Pipelines
description: Build and release Prodyum solutions with Azure DevOps hosted agents, hardened security, and store-ready automation.
last_reviewed: 2025-10-29
owners:
  - @prodyum/devops-guild
---

# Azure DevOps Pipelines

Azure DevOps remains critical for customers that require private network access, advanced approvals, or hybrid infrastructure. This guide reflects Microsoft’s 2025 hosted-agent updates plus Prodyum’s security and store-compliance standards for MAUI and .NET 9 workloads.citeturn2search8turn1search5turn3search1turn2search0

## Hosted agent strategy

| Workload | Agent image | Notes |
|----------|-------------|-------|
| Android & Windows MAUI builds | `windows-2025` (or pinned `windows-latest`) | Visual Studio 2022 17.14 images ship with .NET 9 and updated Android tooling in the October 2025 refresh.citeturn1search5 |
| iOS/macOS MAUI builds | `macos-15` | Required to satisfy Apple’s Xcode 16 / iOS 18 SDK submission mandate.citeturn1search6turn3search1 |
| Backend APIs / Functions | `ubuntu-22.04` | Ubuntu images include .NET 9, container tooling, and workload sets aligned with .NET 9 GA.citeturn1search4turn1search7 |

> Pin explicit `vmImage` values (for example `macos-15`, `windows-2025`) to avoid surprise updates when Microsoft rotates hosted images.citeturn1search5

## Pipeline blueprint

```yaml
trigger:
  branches: [ main ]

variables:
  DotNetSdk: '9.0.304'
  MauiWorkloadManifest: '9.0.202'
  Solution: 'src/Prodyum.TodoApp.sln'

pool:
  vmImage: 'macos-15'

steps:
  - task: UseDotNet@2
    inputs:
      packageType: sdk
      version: $(DotNetSdk)

  - script: |
      dotnet workload install maui --version $(MauiWorkloadManifest)
      dotnet workload list
    displayName: Install MAUI workload

  - task: DotNetCoreCLI@2
    displayName: Restore
    inputs:
      command: restore
      projects: $(Solution)

  - task: DotNetCoreCLI@2
    displayName: Build iOS
    inputs:
      command: build
      projects: $(Solution)
      arguments: >
        -c Release
        -f net9.0-ios
        /p:RuntimeIdentifier=ios-arm64
        /p:ArchiveOnBuild=true

  - task: DotNetCoreCLI@2
    displayName: Run unit tests
    inputs:
      command: test
      projects: tests/Prodyum.TodoApp.Tests/Prodyum.TodoApp.Tests.csproj

  - publish: $(Pipeline.Workspace)
    artifact: ios-archive
```

**Key practices**

- Pin SDK versions with `UseDotNet@2` and workload manifests to maintain parity with developer machines and GitHub Actions.citeturn1search7turn1search1
- Split pipelines per platform (Windows/Android vs macOS/iOS) to parallelise feedback and keep job times within hosted-agent timeouts.citeturn2search8
- Run `dotnet workload repair` in nightly maintenance pipelines to detect manifest drift.citeturn1search7
- Cache `.nuget/packages` and `~/.dotnet` using `Cache@2` for faster builds and to reduce load on private feeds.citeturn2search8

## Secure service connections

- Use Azure DevOps workload identity federation (OIDC) for Azure service connections instead of static secrets.citeturn2search7
- Store App Store Connect, Google Play, and Partner Center credentials in Azure Key Vault, referencing them through Key Vault-linked variable groups.citeturn4search2turn2search8
- Require dual approvers on release environments plus branch policies before deployment.citeturn2search8

## Store automation

| Store | Task | Notes |
|-------|------|-------|
| Apple App Store | Run fastlane or App Store Connect API scripts on `macos-15`; submissions after 24 Apr 2025 must use Xcode 16/iOS 18 SDK.citeturn3search1 |
| Google Play | Deploy `.aab` bundles via the Play Developer API and confirm API level 35 compliance by 31 Aug 2025.citeturn2search0 |
| Microsoft Store | Use StoreBroker or the Microsoft Store Submission API; onboarding fees were removed and analytics expanded in 2025.citeturn4search0turn4search2 |

## Troubleshooting checklist

- **Package restore failures:** Confirm `NuGet.config` feeds and service connections are accessible from the hosted network.citeturn2search8
- **Slow MAUI builds:** Cache workloads or use self-hosted agents pre-loaded with MAUI packs to avoid repeated installations.citeturn1search7
- **Signing issues:** Validate provisioning profiles and keystores; mismatches cause device install failures.citeturn3search1turn2search0
- **MSIX submission errors:** Use `dotnet publish` to honour the pinned SDK and run Windows App Certification Kit before store submission.citeturn4search0

## Governance

- Enforce branch policies requiring successful pipelines before merging to `main` and protect environment approvals for production releases.citeturn2search8
- Surface build/test telemetry in Azure DevOps dashboards for stakeholder visibility.citeturn2search8
- Review shared pipeline templates under `pipelines/templates/` quarterly to align with new policy and security updates.citeturn4search10turn5search8

Adhering to these practices keeps Azure DevOps pipelines aligned with Prodyum compliance, performance, and policy expectations.citeturn2search8turn3search1
