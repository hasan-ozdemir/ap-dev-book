---
title: Release Management & Feedback Loops
description: Plan, stage, and monitor .NET MAUI releases across Apple App Store, Google Play, and Microsoft Store with modern compliance requirements.
last_reviewed: 2025-11-03
owners:
  - @prodyum/release-office
---

# Release Management & Feedback Loops

This playbook orchestrates mobile store releases with phased rollouts, automated gates, and post-launch telemetry so every .NET MAUI release meets Apple, Google, and Microsoft policies in 2025/2026.

## Release stages

| Stage | Objectives | Key activities |
| --- | --- | --- |
| **Plan** | Align release scope, dependencies, and store deadlines. | Confirm Android target SDK ≥ 35, iOS build with Xcode 16 / iOS 18 SDK, and map release windows against court-mandated Play Store changes.citeturn3search5turn2news14 |
| **Stabilise** | Freeze features, complete beta tests, and validate analytics dashboards. | Execute closed testing tracks in Google Play, TestFlight/TestFlight internal groups, and Microsoft Store flighting.citeturn2search6turn2search0 |
| **Roll out** | Ship gradually with automated rollback criteria. | Use phased release in App Store (7-day ramp) and staged rollout percentages in Google Play; keep release pausable.citeturn2search0turn2search6 |
| **Monitor** | Track crash-free users, store feedback, and emerging policy changes. | Instrument crash analytics, respond to reviews within 24h, monitor Google’s new developer verification timelines for sideloading.citeturn2search2turn2search4 |

## Apple App Store cadence

- Enable **phased release** to ramp from 1% to 100% of automatic-update users over seven days; pause when telemetry spikes or ship immediately when metrics are green.citeturn2search0turn2search5
- Combine phased rollout with **manual release** control when you need legal approval before GA; keep notes in release ticket with the paused-day count (limit 30 days).citeturn2search0
- Capture **store review feedback** directly into Jira via webhook automation; use sentiment + crash correlation to decide on hotfix triggers (≤2h response for 1★ regression).

## Google Play strategy

- Use **production staged rollouts** via console or Play Developer API to ship to a percentage subset, raising the cohort only after crash metrics stay within SLA.citeturn2search6
- Maintain **closed/open testing tracks** with pre-release testers; populate release notes and use internal app sharing for last-minute binary validation.citeturn2search6
- Track Google’s **developer identity verification** roadmap (early access Oct 2025, enforced Sep 2026 in first markets, global rollout 2027) to ensure partner teams register early, especially for sideloaded enterprise channels.citeturn2search2turn2search4turn2news12
- Prepare for Play Store legal changes stemming from the Epic injunction—billing flexibility rolls out through Oct 29, 2025; document alternative payment UX reviews.citeturn2news14turn2news15

## Microsoft Store considerations

- Schedule submissions against **flight rings** (private flight → beta → public) and coordinate Intune deployment scripts for sideloading scenarios.
- Validate package signing and **Windows App SDK** min version; align crash telemetry with App Center Diagnostics before moving from beta to retail.

## Governance checklist

1. **Release readiness review:** Confirm automated tests, manual exploratory sign-off, store metadata, and legal/compliance approvals completed 48 hours before submission.
2. **Rollout guardrails:** Define rollback threshold (e.g., crash-free users < 98% for 2 consecutive hours) and publish to release Slack channel before pushing to 20%+.
3. **Telemetry command centre:** During ramp, monitor dashboards (App Insights, Crashlytics) every 30 minutes; escalate via PagerDuty to responsible guild.
4. **Feedback loops:** Summarise reviews, support tickets, and telemetry after full rollout; file improvement items into the next sprint retro.

## Artefacts

- Release ticket template capturing store metadata, phased rollout plan, and rollback owner.
- Automated spreadsheet/report merging App Store, Google Play, and Microsoft Store analytics post-launch.
- Store compliance calendar highlighting regional policy changes (Google verification, EU DMA obligations, Apple privacy updates) with owners per market.citeturn2search2turn2search4turn2news12turn2news13
