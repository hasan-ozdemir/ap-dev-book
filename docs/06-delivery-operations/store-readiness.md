---
title: Store Readiness Checklist
description: Ensure every release meets Apple, Google, and Microsoft requirements before submission.
last_reviewed: 2025-10-29
owners:
  - @prodyum/release-office
---

# Store Readiness Checklist

Run this checklist during the final sprint review for every mobile release. It consolidates 2025 platform updates alongside Prodyum’s internal quality bars.citeturn0news12turn0search7turn3search0

## 1. Compliance

- [ ] **Apple App Store:** Build with Xcode 16+ on the iOS 18 SDK, complete the new age-rating questionnaire by 31 Jan 2026, and publish Accessibility Nutrition Labels plus motion information for spatial content.citeturn0search1turn6search1turn1search6turn4search1
- [ ] **Google Play:** Target Android 15 (API level 35) and update the Data Safety form whenever permissions, SDKs, or data flows change.citeturn2search0turn5search1
- [ ] **Microsoft Store:** Submit the IARC questionnaire (or provide the rating ID) and review refreshed Partner Center acquisition/health analytics before launch.citeturn6search6turn3search0

## 2. Functional quality

- [ ] Smoke-test on physical flagship and constrained devices (Android Go/low-memory, iPhone Pro, Windows 11) to confirm cold-start times ≤ 2 s on lead devices and ≤ 4 s on mid-tier hardware.citeturn7search5
- [ ] Validate offline flows, sync conflict handling, deep links, push notifications, and background tasks under throttled networks (≤ 3 Mbps, 150 ms RTT).citeturn7search7
- [ ] Track stability goals—keep crash-free sessions ≥ 99.85 % and stay below Google Play’s bad-behaviour thresholds for crashes (≤ 1.09 %) and ANR (≤ 0.47 %).citeturn8search6turn8search0

## 3. Performance & telemetry

- [ ] Use release annotations and workspace-based Application Insights dashboards, and migrate to connection-string ingestion before instrumentation keys lose support.citeturn9search0turn9search3
- [ ] Review telemetry for cold/warm start regressions, ANR spikes, and latency outliers after every staged rollout; document findings in Log Analytics and the shared workbook.
- [ ] Keep crash/ANR dashboards and alert rules aligned with Google Play Vitals and MetricKit baselines for the current release window.citeturn8search0turn8search8

## 4. Security & privacy

- [ ] Confirm privacy policies, Data Safety answers, and App Store privacy labels are accurate for each embedded SDK or analytics change.citeturn5search1turn1search1turn1search8
- [ ] Store credentials in platform-secure storage (Keychain, Android Keystore, Windows Credential Locker); prohibit hardcoded secrets in binaries and CI artefacts.
- [ ] Execute OWASP MASVS-aligned penetration tests (dynamic and source review) for major releases and remediate issues before submission.citeturn7search11

## 5. Store assets

- [ ] Localise metadata, screenshots, videos, and Accessibility Nutrition Labels for each target market; confirm age-rating badges and motion labels render correctly.citeturn1search6turn4search1turn6search1
- [ ] Summarise the release (“What’s New”) in two or three user-facing bullet points and align with in-app announcements.
- [ ] Validate icons, splash screens, and branded assets against Prodyum’s design system checklist.

## 6. Operations

- [ ] Define percentage- or region-based phased rollouts (Apple phased release, Google staged rollout) and monitor vitals before expanding the cohort.citeturn0search0turn0search4
- [ ] Equip support with release notes, known issues, and escalation contacts; update the shared operational playbook.
- [ ] Close the feedback loop: triage store reviews within 24 hours, sync churn signals from Play Console and Partner Center dashboards, and feed insights into the backlog.citeturn3search0turn8search8

Document completion of this checklist in the release ticket and attach evidence (screenshots, reports). Use GitHub Actions or Azure DevOps gates to enforce completion before promoting builds to production channels.
