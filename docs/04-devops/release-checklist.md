---
title: Release Readiness Checklist
description: Standardise release approvals across GitHub Actions and Azure DevOps.
last_reviewed: 2025-10-29
owners:
  - @prodyum/devops-guild
---

# Release Readiness Checklist

Use this checklist as an environment protection gate in GitHub Actions or an approval stage in Azure DevOps before promoting builds to production.citeturn2search8turn3search0

## Technical validation

- [ ] Build artifacts generated for every platform (AAB, IPA, MSIX, macOS pkg) and linked to the pipeline summary.citeturn3search0
- [ ] Unit, integration, and UI smoke tests succeed in CI (GitHub Actions or Azure Pipelines) for the release branch.citeturn0search3turn8search5
- [ ] Performance baselines (startup time, memory, FPS) meet agreed SLAs; attach the latest `dotnet-counters` or BenchmarkDotNet report.citeturn8search1turn7search7
- [ ] Security scans (SAST, dependency audit, container scanning) show no outstanding critical issues or have recorded mitigations.citeturn5search5turn0search3

## Compliance & policy

- [ ] Apple metadata refreshed for Xcode 16 / iOS 18 requirements, age-rating questionnaire, App Privacy/Accessibility disclosures, and motion labels.citeturn3search1turn4search1turn1search6
- [ ] Google Play target API level (35) validated and Data Safety answers reviewed after SDK/permission changes.citeturn2search0turn5search1
- [ ] Microsoft Store submission uses current Partner Center onboarding rules and health analytics dashboards.citeturn4search0turn4search2

## Operations

- [ ] Rollout plan documented (phased percentages or regional stages) with rollback criteria and monitoring hooks.citeturn0search4turn9search0
- [ ] Feature flags toggled to intended defaults; state tracked in the release ticket.citeturn8search5
- [ ] Monitoring dashboards updated with deployment annotations (Application Insights / Log Analytics).citeturn9search0
- [ ] Support, customer success, and stakeholder communications prepared (release notes, FAQs, store listing updates).citeturn3search0turn1search4

## Approvals

- [ ] Engineering lead sign-off recorded in the pipeline approval log.
- [ ] Product or PM sign-off confirming scope and go/no-go decision.
- [ ] Security/privacy representative sign-off when data handling or permissions change.citeturn2search8turn5search1

Archive the completed checklist in the release ticket (GitHub issue, Azure Board, or ServiceNow record) for audit and retrospectives.citeturn2search8turn7search2
