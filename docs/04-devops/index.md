---
title: DevOps & Automation
description: Authoritative guidance on build, test, release, and operations automation for Prodyum teams.
last_reviewed: 2025-10-29
owners:
  - @prodyum/devops-guild
---

# DevOps & Automation

Our delivery teams rely on automated pipelines to produce reliable releases across Android, iOS, macOS, Windows, and backend services. This section describes the patterns, workflows, and tooling that keep quality high while shipping continuously.citeturn1search1turn2search8
These operating models are optimized for Prodyum squads, yet they also welcome outside teams who need a proven blueprint for dependable CI/CD and release governance.

## Key practices

- **Pipeline as code:** Keep workflows in `.github/workflows/` or `azure-pipelines.yml` and require pull-request reviews before merging pipeline changes.citeturn1search1turn2search8
- **Environment parity:** Pin .NET SDKs, MAUI workloads, and runner images so CI mirrors developer machines and store requirements.citeturn1search7turn1search5
- **Security-by-default:** Use GitHub/Azure workload identity federation and Key Vault-backed secrets so no plaintext credentials live in pipeline YAML.citeturn2search0turn2search7turn4search2
- **Observability in CI:** Publish build metrics, coverage, and deployment annotations to dashboards (Application Insights, Log Analytics, Azure Boards) for stakeholder visibility.citeturn9search0turn0search3

## Modules

- [GitHub Actions CI/CD](./ci-with-github-actions.md)
- [Azure DevOps Pipelines](./azure-devops-pipelines.md)
- [CI/CD Playbook for .NET & .NET MAUI](./ci-cd-toolbox.md)
- [SDLC & DevOps Lifecycle](./sdlc-devops-lifecycle.md)
- [Release Readiness Checklist](./release-checklist.md)
- [Quality Gates & Testing Automation](./quality-gates.md)
- [Code Quality Foundations](./code-quality-foundations.md)
- [Build Health & Branch Policies](./build-health-branch-policies.md)
- [Static Analysis Toolbox](./static-analysis-toolbox.md)
- [Failure Detection Across Build & Runtime](./failure-detection-runtime-tooling.md)
- [Pull Request Playbook](./pull-request-playbook.md)

Start with GitHub Actions for net-new products; use Azure DevOps when customers require private networking or advanced release approvals.citeturn1search1turn2search8
