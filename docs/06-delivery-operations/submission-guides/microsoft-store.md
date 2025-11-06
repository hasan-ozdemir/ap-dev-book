---
title: Microsoft Store
description: Publish and manage Windows packages in the Microsoft Store with the latest 2025 platform improvements.
last_reviewed: 2025-10-29
owners:
  - @prodyum/release-office
---

# Microsoft Store

## 1. Policy & platform updates (2025)

- Microsoft removed the individual developer registration fee and simplified onboarding for organizations, lowering barriers to entry. citeturn2news12
- Store analytics gained new engagement dashboards (acquisitions, usage funnels, retention) accessible via Partner Center APIs—use them to feed product OKRs. citeturn2news12
- Packaging guidance recommends MSIX with WinUI 3 for MAUI Windows targets; ensure packages pass the Windows App Certification Kit.

## 2. Build pipeline steps

1. Create MSIX packages via `dotnet publish -f net9.0-windows10.0.19041.0 -c Release -p:WindowsPackageType=MSIX`.
2. Sign the package using a code-signing certificate stored in Azure Key Vault.
3. Run `winappdriver` or UI automation tests if the app exposes desktop UI workflows.
4. Upload to Partner Center using the Microsoft Store Submission API or StoreBroker PowerShell module.

## 3. Metadata checklist

- App name, description, and search terms localised.
- Category and subcategory accurate (e.g., Productivity → Developer tools).
- Screenshots for desktop (light/dark theme), optional videos.
- Privacy policy URL and support contact updated.

## 4. Progressive rollout

- Use staged rollout percentages (5%, 20%, 50%, 100%) to monitor telemetry before global release.
- Monitor the new engagement dashboards and App Health metrics daily for the first week.
- Set up webhooks or API polling to capture certification failures in CI.

## 5. Common certification blockers

- Missing digital signature or using SHA-1 certificates.
- API calls not supported on Windows 10 19041 baseline.
- Failure to declare capabilities (e.g., background tasks, Bluetooth) in the app manifest.

Document each submission in the release ticket with Partner Center submission IDs and analytics baseline metrics.

