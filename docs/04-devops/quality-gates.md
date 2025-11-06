---
title: Quality Gates & Testing Automation
description: Enforce quality thresholds directly in CI/CD pipelines.
last_reviewed: 2025-10-29
owners:
  - @prodyum/devops-guild
---

# Quality Gates & Testing Automation

Quality gates prevent regressions from reaching production. This playbook shows how to implement automated checks for coverage, performance, security, and policy compliance.citeturn0search3turn9search0

## 1. Static analysis & formatting

- Run `dotnet format` together with StyleCop/Roslyn analyzers on every pull request to keep coding standards consistent.citeturn0search3
- Fail builds when analyzers raise diagnostics at `error` severity and keep rulesets in source control for auditability.citeturn0search3
- Publish SARIF results to GitHub Advanced Security or Azure DevOps code-analysis dashboards so security and compliance teams can review them centrally.citeturn0search3turn2search8

## 2. Test coverage

- Collect coverage via `coverlet.collector` (or `coverlet.msbuild`) during `dotnet test`.citeturn0search3
- Generate reports with ReportGenerator and fail the pipeline if core libraries fall below 80 % line coverage.citeturn0search3turn8search5
- Surface coverage badges using `actions/upload-artifact` + GitHub Pages (or a badge service) so contributors see deltas after every merge.citeturn0search3

## 3. Performance thresholds

- Execute `dotnet-counters` (or custom benchmark suites) nightly and on release branches to detect startup and memory regressions early.citeturn8search1
- Compare results against a baseline JSON (`artifacts/perf-baseline.json`) and fail when metrics drift by more than 10 % to protect SLAs.citeturn8search1turn7search7

## 4. Mobile policy checks

- Automate validation of Apple, Google, and Microsoft store requirements (target SDK, Xcode version, Partner Center health gates) before promoting a build.citeturn3search1turn2search0turn4search2
- Diff privacy manifests, App Privacy answers, and Google Play Data Safety descriptors against source-controlled templates whenever permissions or SDKs change.citeturn4search1turn5search1turn1search0

## 5. Security gates

- Run SCA/OSS scans with Dependabot, GitHub Advanced Security, or Microsoft Security DevOps on every branch.citeturn0search3turn2search8
- Scan container images and publish findings to Microsoft Defender for Cloud; block deployment until high‑severity issues are resolved or waived.citeturn5search5turn5search8
- Require mitigation plans or exemptions for any critical CVEs identified during scans.citeturn5search5turn5search1

## 6. Manual verifications

- Link exploratory test session notes (for example Azure Test Plans attachments) in the pipeline summary for traceability.citeturn7search2
- Capture store asset reviews (screenshots, metadata diffs) as build artefacts so auditors can confirm compliance with Apple and Google guidelines.citeturn4search1turn1search4

Implement these gates in reusable pipeline templates so every product benefits from consistent enforcement.citeturn0search3turn2search8
