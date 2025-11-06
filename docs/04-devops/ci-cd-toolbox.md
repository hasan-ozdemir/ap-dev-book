---
title: CI/CD Playbook for .NET & .NET MAUI
description: Choose, implement, and scale continuous integration and delivery pipelines for .NET 9 and .NET MAUI with modern tooling, governance, and sample configurations.
last_reviewed: 2025-11-03
owners:
  - @prodyum/devops-guild
---

# CI/CD Playbook for .NET & .NET MAUI

High-performing mobile squads treat CI/CD as the nervous system of delivery—automating builds, signing, validation, and gated releases on every commit. Three shifts define the 2025 landscape: Visual Studio App Center build/test/distribute services retire on 31 March 2025, GitHub Actions and Azure Pipelines introduced macOS 15 and Windows 2025 hosted runners while deprecating older pools, and Microsoft now urges teams to pin MAUI workloads in CI whenever SDK manifests change to avoid unexpected failures.citeturn0search0turn0search4turn1search6turn2search0

---

## 1. Strategic context (Q4 2025)

- **App Center retirement:** Microsoft directs customers to migrate builds to Azure Pipelines or GitHub Actions, device testing to BrowserStack, and distribution to TestFlight/Google Play before the March 2025 shutdown; analytics/diagnostics follow later.citeturn0search0
- **Hosted runner refresh:** GitHub’s runner GA announcement highlights the new `macos-15` and `windows-2025` labels, while the migration issue tracks the retirement timeline for macOS 12 and other legacy images—update YAML before the brownouts.citeturn0search4turn1search6
- **Workload pinning:** The .NET team advises reinstalling workloads whenever SDK versions change so CI stays aligned with local tooling after manifest updates (e.g., Xcode 16.3 support).citeturn2search0turn2search3

---

## 2. Core pipeline building blocks

- **Continuous Integration (CI):** Automate restore, build, analyzers, unit tests, signing prep, and artefact upload on every push.
- **Continuous Delivery (CD):** Promote signed bundles to beta/staging rings with manual gates for production stores.
- **Continuous Deployment:** Reserve for internal/beta rings (TestFlight, Google Play Internal Testing) because public store submissions still require human review.
- **Canonical MAUI job flow:** set the SDK (`actions/setup-dotnet` or `UseDotNet@2`), install workloads, import signing assets, build platform targets (`dotnet publish`), run tests, notarise/sign, and publish artefacts. Microsoft’s Weather21 sample pipelines illustrate this pattern for both GitHub Actions and Azure DevOps.citeturn2search6
- **Quality gates:** Integrate SAST/DAST (CodeQL, GitHub Advanced Security, SonarQube), dependency scanning, SBOM export, accessibility tests, store policy linting, and device-lab smoke tests before promotion.

---

## 3. Service playbooks

### 3.1 GitHub Actions for .NET MAUI

**When to use it** — Teams hosting code on GitHub who need reusable workflows, environments, and hosted Apple/Windows runners (now with macOS 15 hardware).citeturn0search4

**Reference pipeline**

```yaml
name: maui-ci

on:
  pull_request:
  push:
    branches: [ main ]

jobs:
  build:
    runs-on: macos-15
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 9.0.104
      - name: Install MAUI workloads
        run: dotnet workload install maui --version 9.0.104 --skip-manifest-update
      - name: Restore
        run: dotnet restore src/App.sln
      - name: Build Android
        run: dotnet publish src/App/App.csproj -f net9.0-android -c Release -o artifacts/android
      - name: Build iOS
        run: dotnet publish src/App/App.csproj -f net9.0-ios -c Release -o artifacts/ios
      - name: Upload artifacts
        uses: actions/upload-artifact@v4
        with:
          name: maui-binaries
          path: artifacts/**
```

This workflow mirrors Microsoft’s Weather21 example and pins both SDK and workload versions to avoid regressions introduced by MAUI manifest updates, such as the 2025 Xcode 16.3 rollout.citeturn2search6turn2search3

**Operational notes**

- Prefer Apple Silicon runners (`macos-15`, `macos-15-large`, `macos-15-xlarge`) for faster MAUI builds; add Windows legs when you need Windows packaging or Play Console automation.citeturn0search4turn1search6
- Use workflow environments for gated releases and replace App Center distribution with App Store Connect / Google Play CLI tasks recommended in Microsoft’s migration guidance.citeturn0search0
- Cache NuGet/Gradle directories per runner; reinstall workloads whenever `global.json` changes to keep SDK/tooling parity.citeturn2search0

### 3.2 Azure DevOps Pipelines

**When to use it** — Enterprises standardising on Azure DevOps Boards/Repos that rely on multi-stage approvals, service connections, and managed identities. Sprint 253 introduced `macOS-15` and `windows-2025` hosted agents while scheduling macOS 12 retirement—update the `vmImage` property before the cutoff.citeturn1search1

```
trigger:
  branches: { include: [ main ] }

stages:
  - stage: Build
    jobs:
      - job: MacBuild
        pool: { vmImage: 'macOS-15' }
        steps:
          - task: UseDotNet@2
            inputs: { packageType: 'sdk', version: '9.0.104' }
          - script: dotnet workload install maui --version 9.0.104 --skip-manifest-update
          - task: DotNetCoreCLI@2
            inputs:
              command: 'publish'
              projects: 'src/App/App.csproj'
              arguments: '-f net9.0-ios -c Release -o $(Build.ArtifactStagingDirectory)/ios'
          - publish: $(Build.ArtifactStagingDirectory)/ios
            artifact: ios

      - job: WindowsBuild
        pool: { vmImage: 'windows-2022' }
        steps:
          - task: UseDotNet@2
            inputs: { packageType: 'sdk', version: '9.0.104' }
          - script: dotnet workload install maui --version 9.0.104 --skip-manifest-update
          - task: DotNetCoreCLI@2
            inputs:
              command: 'publish'
              projects: 'src/App/App.csproj'
              arguments: '-f net9.0-android -c Release -o $(Build.ArtifactStagingDirectory)/android'
          - publish: $(Build.ArtifactStagingDirectory)/android
            artifact: android
```

Apply the same secrets handling, approvals, and release orchestration showcased in the Weather21 Azure Pipelines sample when adding TestFlight/Google Play promotion stages.citeturn2search6

### 3.3 Codemagic

Codemagic provides mobile-first CI/CD with MAUI-ready templates, Apple Silicon hardware, secure signing vaults, and first-party integrations for Google Play and App Store Connect.citeturn1search5

- YAML workflows include tasks to install MAUI workloads, bump Android version codes, and package signed `.aab`/`.ipa` artefacts.
- `mac_mini_m2` instances shorten iOS build times while secrets stay encrypted in Codemagic groups.
- Built-in publishing steps replace App Center distribution and feed analytics dashboards for beta adoption tracking.

Choose Codemagic when you want turnkey mobile pipelines without managing runners or when teams already ship other cross-platform apps (Flutter, React Native) via Codemagic.citeturn1search5

### 3.4 Bitrise

Bitrise offers a guided migration path from App Center with OTA distribution, TestFlight/Google Play deploy steps, device registration automation, and Public Install Pages to keep tester flows intact.citeturn1search0

- Use the migration toolkit to clone App Center build configuration, then convert steps to `dotnet restore/build/publish`.
- Pick stacks that bundle Xcode 16.x and .NET 9 SDK to stay aligned with MAUI requirements.
- Monitor runtime metrics and rollout health through Bitrise Insights and install pages.

---

## 4. Migration & distribution guidance

1. **App Center exit plan:** Export configurations, certificates, and release history before March 2025; move builds to Azure Pipelines or GitHub Actions, device tests to BrowserStack, and distribution to TestFlight/Google Play/Windows package flights.citeturn0search0
2. **Runner alignment:** Replace deprecated images (`macos-12`, `ubuntu-20.04`) with the new hosted pools to avoid brownouts during the retirement windows.citeturn0search4turn1search6
3. **Workload pinning:** Commit `global.json` and explicit `dotnet workload install --version` commands so manifest updates cannot break the build.citeturn2search0turn2search3
4. **Signing modernisation:** Prefer OIDC/managed identity flows (App Store Connect API keys, Google Play service accounts) to minimise long-lived secrets in pipeline variable stores.

---

## 5. Implementation checklist

- [ ] Pipelines target current hosted images (`macos-15`, `windows-2025`, `ubuntu-24.04`) with fallback self-hosted runners where required.citeturn0search4turn1search6
- [ ] `global.json` and workload versions are committed; CI installs matching workloads before build stages.citeturn2search0
- [ ] Certificates and provisioning profiles live in hardware-backed vaults (Codemagic secure groups, Bitrise Secrets) and are downloaded just-in-time.citeturn1search5turn1search0
- [ ] Quality gates cover unit/UI tests, static analysis, SBOM export, and device-lab smoke tests before promotion.citeturn0search0
- [ ] Distribution steps call App Store Connect, Google Play, or Windows package flight APIs—no reliance on App Center endpoints.citeturn0search0
- [ ] Release artefacts (`.msix`, `.aab`, `.ipa`) include changelogs and SBOMs so production deployments remain auditable.
