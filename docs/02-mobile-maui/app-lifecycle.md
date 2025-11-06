---
title: .NET MAUI App Lifecycle from Development to Store
description: Coordinate planning, development, testing, packaging, store submission, telemetry, and updates for .NET MAUI apps.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

# .NET MAUI App Lifecycle from Development to Store

Delivering production-ready .NET MAUI apps across Android, iOS, macOS, and Windows demands reliable tooling, disciplined quality practices, and repeatable release operations. Use this lifecycle playbook to align teams from first sprint to post-release monitoring.

---

## 1. Environment and planning

- **Tooling**: Install the .NET 9 SDK, MAUI workload, and Visual Studio 2022 17.14 or later. The latest release adds Live Preview mirroring and emulator streaming that shorten UI feedback loops.[^vs1714]
- **Hosted build agents**: Secure macOS hardware for iOS signing, configure Windows runners for MSIX packaging, and script Android SDK/NDK installs in CI.
- **Accounts and certificates**: Provision Apple Developer Program assets (App IDs, provisioning profiles, privacy manifests), Google Play Console access with Play App Signing keys, and Partner Center permissions if you distribute on Windows.
- **Release governance**: Agree on semantic versioning, supported OS baselines, rollout cadence, rollback triggers, and ownership rotations before implementation begins.

---

## 2. Development and debugging workflow

- Adopt a modular architecture (MVVM, MVU, Clean) backed by dependency injection so view logic remains testable and platform abstractions can be mocked.
- Consolidate colours, typography, and spacing in shared `ResourceDictionary` files; exercise Live Preview and Live Visual Tree during development to validate layouts without redeploying.[^vs1714]
- Use Visual Studio for the primary build/debug loop and the MAUI VS Code extension for lightweight changes; capture local telemetry with `dotnet monitor`, `dotnet-counters`, and structured logging to surface performance issues early.

---

## 3. Quality automation and device coverage

- Run unit and integration suites on every pull request (see the Testing Methodologies playbook) with coverage and mutation thresholds.
- Follow Microsoft’s Appium + NUnit reference harness to share fixtures across Android, iOS, macOS, and Windows while keeping `AutomationId` usage consistent.[^appium]
- App Center device labs retire on March 31 2025 and diagnostics follow in 2026; migrate scheduled runs to BrowserStack App Automate or similar device clouds that provide hosted Appium endpoints and throttled network profiles.[^appcenter-retirement]
- Fail builds on crash/ANR smoke suites and beta telemetry before promoting artifacts to production tracks.

---

## 4. Packaging and signing

### Android

```bash
dotnet publish -f net9.0-android -c Release \
  -p:AndroidPackageFormat=aab \
  -p:AndroidSigningKeyStore=contoso.keystore \
  -p:AndroidSigningKeyAlias=contoso \
  -p:AndroidSigningKeyPass=$KEY_PASS \
  -p:AndroidSigningStorePass=$STORE_PASS
```

- Generate or rotate keystores with `keytool`, enroll in Play App Signing, and validate output with `bundletool build-apks --bundle app.aab --output app.apks`.[^bundletool]

### iOS / Mac Catalyst

```bash
dotnet publish -f net9.0-ios -c Release \
  -p:ArchiveOnBuild=true \
  -p:BuildIpa=true \
  -p:IpaPackageDir=artifacts/ios
```

- Maintain provisioning profiles and certificates (export `.p12` for CI), then upload archives via Xcode or Transporter after CLI publishing.[^ios-publish]

### Windows (optional)

- Produce MSIX packages with `dotnet publish -f net9.0-windows10.0.19041.0 -c Release -p:GenerateAppxPackageOnBuild=true` and sign them with Partner Center certificates.

Centralise version codes, application identifiers, and signing metadata in `Directory.Build.props` so every head shares the same configuration.

---

## 5. Store submission

### Apple App Store

1. Upload the signed build through Xcode or Transporter and connect it to a TestFlight group.
2. Provide App Store Connect metadata (screenshots, privacy nutrition labels, compliance statements).
3. Address review considerations such as descriptive permission usage strings and AOT page-size limits before requesting release approval.[^ios-publish]

### Google Play

1. Create internal or closed testing tracks and upload the `.aab`.
2. Complete Data Safety forms, content ratings, Play Integrity configuration, and release notes.
3. Promote with staged rollout percentages while monitoring crash and ANR dashboards.[^gp-release]

### Windows Store / Enterprise

1. Submit MSIX packages via Partner Center with Store listing assets.
2. For enterprise distribution, publish internal MSIX feeds or private Winget repositories.

---

## 6. Feedback, telemetry, and operations

- Wire crash reporting (Application Insights, Sentry, Firebase Crashlytics) and correlate incidents with release identifiers.
- App Center Diagnostics remains available through June 30 2026; plan migrations to alternative telemetry stacks or BrowserStack observability ahead of that date.[^appcenter-retirement]
- Track startup, memory, and frame pacing metrics by automating `dotnet-trace` and `dotnet-counters` captures during device lab runs.[^dotnet-diag]
- Capture in-app feedback prompts and monitor store reviews; triage findings into a shared backlog with owners and service-level targets.

---

## 7. Update, rollout, and republish cycle

1. **Plan** enhancements based on telemetry, user sentiment, and dependency advisories.
2. **Develop** behind feature flags for safe staged releases.
3. **Test** by rerunning automated suites, device-cloud runs, and beta programs; enforce crash-free thresholds of at least 99%.
4. **Package** signed release candidates and archive “last known good” artifacts.
5. **Submit** to store tracks, using phased or percentage-based rollouts to limit blast radius.
6. **Monitor** crash analytics, telemetry alerts, and review sentiment; pause promotion if thresholds slip.
7. **Communicate** with release notes, stakeholder updates, and in-app announcements; document follow-up items.

---

## 8. Rollback and hotfix playbook

- Maintain feature flags that disable problematic functionality instantly.
- Android: upload a hotfix `.aab` with a higher version code and promote it through the existing track.
- iOS: request an expedited review for critical fixes and pause phased releases while Apple processes the update.
- Windows: withdraw or supersede the flighted MSIX and redeploy the previous package.

---

## Lifecycle checklist

- [ ] Tooling, accounts, certificates, and governance processes established.
- [ ] Automated unit, integration, and UI suites running in CI with coverage gates.
- [ ] Device-cloud strategy migrated before the March 2025 App Center retirement.
- [ ] Release build outputs signed, validated, and stored for every platform.
- [ ] Beta distribution, crash analytics, and telemetry dashboards monitored during rollout.
- [ ] Documented rollback, hotfix, and communication plans per release train.

Executing this lifecycle keeps Prodyum releases on schedule while adapting to evolving platform tooling and store policies.

---

[^vs1714]: Microsoft, "Visual Studio 2022 v17.14 is now generally available!," May 14, 2025. citeturn2search1
