---
title: GitHub Actions CI/CD
description: Build, test, and distribute .NET MAUI and backend workloads with GitHub Actions pipelines aligned to 2025 platform requirements.
last_reviewed: 2025-10-29
owners:
  - @prodyum/devops-guild
---

# GitHub Actions CI/CD

GitHub Actions remains our default automation engine for cross-team delivery. This playbook captures the 2025 hosted-runner changes, security practices, and store compliance rules affecting MAUI clients and .NET 9 services.citeturn1search1turn1search5turn3search1turn2search0turn4search0

## 1. Hosted runner strategy

| Target | Runner | Notes |
|--------|--------|-------|
| .NET MAUI (Android) | `ubuntu-22.04` | Latest LTS Linux image ships current .NET and Android SDKs—pin this label to avoid surprise moves when `ubuntu-latest` advances.citeturn1search1turn1search7 |
| .NET MAUI (iOS/macOS) | `macos-15` | GA macOS image bundles Xcode 16.4+ so builds meet Apple's April 24 2025 SDK requirement.citeturn1search5turn1search6turn3search1 |
| .NET 9 APIs / hybrid workloads | `ubuntu-22.04` or `windows-2025` | Use Windows when tests need Windows-only dependencies (MSI, registry).citeturn1search4turn1search5 |

> Always pin explicit runner labels (`macos-15`, `ubuntu-22.04`) so image changes do not break pipelines overnight.citeturn1search1

## 2. Secrets and identity

- Store App Store Connect, Google Play, and Partner Center credentials in repository or organisation secrets.
- Prefer OpenID Connect with `azure/login@v2` (`id-token: write`, `permissions: contents: read`) to issue short-lived cloud tokens instead of storing service principal secrets.citeturn2search0turn2search7turn2search8
- Surface shared variables—`DOTNET_VERSION`, `ANDROID_SDK_ROOT`, signing profile paths—through workflow `env` blocks or reusable workflow inputs.

## 3. Sample .NET MAUI workflow

```yaml
name: maui-build-test

on:
  pull_request:
    branches: [ main ]
    paths:
      - 'src/**'
      - '.github/workflows/maui-build.yml'

jobs:
  android:
    runs-on: ubuntu-22.04
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 9.0.104
      - name: Install MAUI workload
        run: dotnet workload install maui --version 9.0.202
      - name: Restore
        run: dotnet restore Prodyum.TodoApp.sln
      - name: Build Android
        run: dotnet build Prodyum.TodoApp/Prodyum.TodoApp.csproj -c Release -f net9.0-android
      - name: Run unit tests
        run: dotnet test tests/Prodyum.TodoApp.Tests/Prodyum.TodoApp.Tests.csproj

  ios:
    runs-on: macos-15
    needs: android
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 9.0.104
      - name: Install MAUI workload
        run: dotnet workload install maui --version 9.0.202
      - name: Select Xcode 16.4
        run: sudo xcode-select -s /Applications/Xcode_16.4.app
      - name: Provision signing assets
        uses: apple-actions/import-codesign-certs@v3
        with:
          p12-file-base64: ${{ secrets.IOS_CERT }}
          p12-password: ${{ secrets.IOS_CERT_PASSWORD }}
          mobileprovision-base64: ${{ secrets.IOS_PROVISION }}
      - name: Build iOS ipa
        run: dotnet build Prodyum.TodoApp/Prodyum.TodoApp.csproj -c Release -f net9.0-ios /p:RuntimeIdentifier=ios-arm64 /p:ArchiveOnBuild=true
      - name: Publish artifact
        uses: actions/upload-artifact@v4
        with:
          name: ios-ipa
          path: **/*.ipa
```

**Key takeaways**

- Pin the .NET SDK version so workload manifests align with the hosted runner and you control upgrades.citeturn1search7
- Install MAUI workloads inside each job because hosted images reset between runs.citeturn1search1
- Select the desired Xcode path explicitly (`xcode-select`) whenever Apple or GitHub ships a new toolchain.citeturn1search6turn1search5

## 4. Publish to stores

- **Apple App Store / TestFlight:** Apple accepts only Xcode 16 + iOS 18 SDK submissions from 24 Apr 2025 onward—embed metadata checks for age ratings, accessibility labels, and motion disclosures before requesting approval.citeturn3search1turn4search1
- **Google Play:** Google requires Android 15 (API 35) targets for all updates after 31 Aug 2025; drive staged rollouts using internal → closed → production tracks.citeturn2search0turn1search4
- **Microsoft Store:** Individual developer fees are gone and Partner Center now surfaces richer acquisition/health analytics—store credentials securely and use dashboards to confirm post-release health.citeturn4search0turn4search2

## 5. Quality gates

- Enforce automated coverage, formatter, and static-analysis checks (`coverlet`, `dotnet format`, `sonarsource/sonarqube-scan-action`) on every pull request.citeturn0search3
- Record deployment annotations in Application Insights/Log Analytics so operations can correlate incidents with rollouts.citeturn9search0
- Block promotion until store validation jobs succeed (Apple metadata, Play Data Safety, Partner Center readiness).citeturn3search1turn5search1turn4search0

## 6. Release automation checklist

- [ ] Build artifacts (AAB, IPA, MSIX) created and signed.
- [ ] Store submission metadata verified against active policies (Apple motion/accessibility labels, Play target API, Microsoft Store analytics).
- [ ] Release notes generated from Git history and attached to store submissions.
- [ ] Environment-specific configuration injected via secrets or key vault references.
- [ ] Monitoring hooks (Application Insights, Azure Monitor) configured with deployment annotations.

Wrap this workflow as a reusable template (for example `.github/workflows/_templates/maui-release.yml`) so product teams inherit policy changes automatically while overriding environment variables when needed.
