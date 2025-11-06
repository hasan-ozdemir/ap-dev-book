---
title: SDLC & DevOps Lifecycle for .NET and .NET MAUI
description: Operational blueprint covering agile ceremonies, work management, source control, CI/CD, continuous testing, and environment strategies tailored to .NET 9 and .NET MAUI delivery teams.
last_reviewed: 2025-10-29
owners:
  - @prodyum/devops-guild
---

# SDLC & DevOps Lifecycle for .NET and .NET MAUI

Modern delivery demands aligned ceremonies, tooling, and automation. This playbook synchronises agile practice with .NET 9/.NET MAUI engineering so every squad can shepherd features from idea to production without sacrificing quality. Microsoft’s Sprint 252 update confirmed New Boards Hub would become the default Azure DevOps experience by May 2025, and the July 2025 sprint release notes closed the loop by retiring Old Boards for every organisation.citeturn0search0turn0search1

---

## 1. Operating model overview

1. **Discover** – product discovery, backlog refinement, roadmap shaping.
2. **Plan** – sprint planning, capacity modelling, dependency mapping.
3. **Build** – trunk-based development, automated build/test pipelines.
4. **Validate** – continuous testing (unit → integration → exploratory), observability gates.
5. **Release** – staged deployments, environment promotion, launch readiness.
6. **Learn** – telemetry review, retrospectives, AI-assisted insights, roadmap updates.

Tie these phases to short, timeboxed iterations—Scrum mandates sprints of one month or less, producing a potentially shippable Increment each time.citeturn15search0

---

## 2. Agile ceremonies & cadence

| Ceremony | Focus | Timebox & notes |
| --- | --- | --- |
| Sprint Planning | Confirm the Sprint Goal and forecast backlog items | <= 8 h for a one-month sprint; scale proportionally (~ 4 h for two weeks).citeturn1search0 |
| Daily Scrum | Inspect progress toward the Sprint Goal | 15 min at the same time/place to cut coordination overhead.citeturn1search0 |
| Backlog Refinement | Keep upcoming work ready | Treat refinement as ongoing collaboration; schedule touchpoints so items meet Definition of Ready before planning.citeturn1search0 |
| Sprint Review | Inspect the Increment with stakeholders | <= 4 h for a one-month sprint; shorter for shorter iterations.citeturn1search0 |
| Sprint Retrospective | Examine team practices and agree on improvements | Up to 3 h for a one-month sprint; focus on actionable follow-ups.citeturn1search0 |

**AI augmentation:** Remote squads can lean on facilitation platforms to keep hybrid ceremonies focused—Miro’s Create with AI jump-starts retrospective prompts and clustering on shared boards, while TeamRetro’s AI grouping and summaries accelerate theme discovery and action capture without losing facilitator control.citeturn6search0turn7search0

---

## 3. Work management & governance

### 3.1 Azure Boards (or equivalent)

- **Adopt New Boards Hub:** Enable the New Boards experience early and train teams on the updated swimlanes, cards, and analytics before it becomes mandatory.citeturn0search0turn0search1
- **Process templates:** Start from the Scrum process (PBIs, Bugs, Tasks) and limit custom fields/states to preserve reporting fidelity.citeturn3search0
- **Portfolio scaling:** Use parent/child hierarchies (Epics → Features → PBIs) to visualise cross-squad plans and dependencies.citeturn3search0
- **Capacity & velocity:** Use the Azure Boards capacity tool and sprint velocity charts for sustainable planning.citeturn3search1

### 3.2 Work-item hygiene

- Maintain INVEST-quality stories with Definition of Ready and Definition of Done checklists.citeturn3search2
- Link work items to code (pull requests, commits) and builds for compliance traceability.citeturn3search0
- Track impediments explicitly (Impediment work-item type) and raise them in Daily Scrum.citeturn3search0

---

## 4. Source control & branching

1. **Default to trunk-based development** with short-lived branches (< 2 days). Protect main with rulesets requiring reviews and checks.citeturn4search0turn4search1
2. **Branch policies:** require at least one reviewer, successful CI, and linked work items; enforce signed commits and linear history if compliance demands.citeturn4search1turn4search2
3. **Merge queue:** Use GitHub Merge Queue to serialise merges and keep main green at scale; monitor wait times to avoid bottlenecks.citeturn5search0
4. **Pull-request playbook:** Apply the portal’s PR playbook (templates, Danger, AI review guardrails) for consistent reviews.citeturn5search1

---

## 5. Build pipelines & automation

### 5.1 GitHub Actions

- Centralise standards via reusable workflows (workflow_call) for build, test, store compliance, and security scans.citeturn1search1
- Use environment protection rules and required reviewers before releasing to production environments.citeturn2search8

### 5.2 Azure DevOps Pipelines

- Pin hosted-agent images (e.g., macos-15, windows-2025) and use pipeline variables for SDK/workload versions.citeturn1search5turn3search1
- Adopt workload identity federation for service connections instead of static secrets.citeturn2search7

### 5.3 Shared practices

- Automate secrets retrieval via GitHub OIDC or Azure Key Vault variable groups, never storing plaintext secrets in YAML.citeturn2search0turn4search2
- Cache NuGet packages and workloads for faster builds and fewer private-feed hits.citeturn2search8

---

## 6. Continuous testing & quality gates

- Layer unit, integration, smoke, and exploratory tests; enforce coverage and static-analysis gates described in the Quality Gates playbook.citeturn0search3turn9search0
- Run performance benchmarks or dotnet-counters suites, comparing against baselines in nightly builds.citeturn8search1turn7search7
- Validate store compliance (target SDK, metadata, privacy manifests) automatically before promotion.citeturn3search1turn2search0turn4search1

---

## 7. Environments & release strategy

| Environment | Purpose | Automation focus |
| --- | --- | --- |
| Dev / CI | Feature validation, integration tests | Ephemeral infrastructure, seeded data, fast feedback.citeturn2search8 |
| QA / Staging | Full-fidelity validation, manual exploratory | Deploy via release pipelines; enable blue/green or canary toggles.citeturn0search4 |
| Beta / Internal | Store beta testing (TestFlight, Play internal) | Manage phased rollouts and staged groups before public launch.citeturn0search4turn2search0 |
| Production | Customer traffic | Use deployment protections, progressive exposure, and automated rollback criteria.citeturn4search2turn9search0 |

Document environment contracts (configs, toggles, data refresh cadence) in repo-managed runbooks for auditability.citeturn2search8

---

## 8. Observability & feedback loops

- Instrument clients and services with Application Insights, Azure Monitor, and GitHub Deployment APIs; annotate releases for correlation.citeturn9search0
- Review telemetry, support tickets, and store reviews within 24 hours; feed insights into backlog refinement.citeturn3search0turn8search8
- Hold sprint reviews/retrospectives across squads, capturing decisions and improvement actions.citeturn15search0turn2search5

---

## 9. Continuous improvement

- Track DORA metrics (lead time, deployment frequency, MTTR, change fail rate) in DevOps dashboards to measure flow.citeturn6search0turn6search2
- Use AI copilots (GitHub Copilot, Azure OpenAI) for code suggestions, test generation, and knowledge retrieval—always review and retain human accountability.citeturn5search2turn5search4turn5search6
- Review portal playbooks quarterly to capture new platform policies, hosted-agent changes, or audit findings.citeturn1search5turn4search10

By weaving these practices together, Prodyum squads can deliver continuous value with confidence while staying aligned with evolving platform and compliance expectations.citeturn1search1turn2search8
