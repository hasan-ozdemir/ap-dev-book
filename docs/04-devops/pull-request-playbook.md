---
title: Pull Request Playbook
description: Ship consistent, high-quality changes with structured PR workflows, resilient automation, and reviewer alignment.
last_reviewed: 2025-10-31
owners:
  - @prodyum/devops-guild
---

# Pull Request Playbook

Successful pull requests blend thoughtful communication with automated safeguards. GitHub recommends concise context, prompt reviews, and automated checks so contributors spend time on meaningful feedback instead of rework.citeturn2search0

---

## 1. Prepare the branch

1. **Protect critical branches** with rulesets: require reviews, status checks, signed commits, and non fast-forward merges on `main`/`release/*`.citeturn0search0turn0search1
2. **Apply Azure DevOps branch policies** for hybrid teams—block direct pushes, enforce linked work items, and configure build validation on protected branches.citeturn2search9
3. **Define PR templates and labels** (Issue Forms + CODEOWNERS) so authors supply testing notes, screenshots, and reviewer routing automatically.citeturn1search0turn2search0

---

## 2. Author checklist

- Keep PRs small (under ~400 lines touched) and ship frequently—GitHub reports that small, focused changes accelerate review turnaround and defect detection.citeturn2search5
- Provide context: summary, testing evidence, screenshots/video for UX changes, and linked work items.citeturn2search0
- Mark breaking or risky changes with labels and ping reviewers early when additional expertise is needed.
- Run the same lint/test commands locally (`pre-commit`, `dotnet test`, static analyzers) that CI will execute to avoid churn.citeturn2search1

---

## 3. Reviewer checklist

- Respond within one business day; negotiate ownership if overloaded and use CODEOWNERS or reviewer-rotation bots to balance load.citeturn2search0turn0search2
- Focus on correctness, security, maintainability, and alignment with architectural patterns; leave style nitpicks to automated linters.citeturn2search1
- Celebrate good changes, ask clarifying questions, and document rationale in the PR for future readers.citeturn2search0

---

## 4. Automation & quality gates

| Stage | Automation | References |
| --- | --- | --- |
| **Static analysis** | Roslyn analyzers, StyleCop, secret scanning, dependency scanning run on every PR. | citeturn2search3turn0search6turn0search10 |
| **Tests** | Matrix CI across Windows, macOS, Android, iOS; Android/iOS UI suites run on BrowserStack App Automate (App Center test lab retires Mar 31, 2025). | citeturn1search4turn3search4 |
| **Preview builds** | Workflow dispatch/bot commands trigger signing builds and uploading to BrowserStack/TestFlight dashboards linked in the PR. | citeturn3search2turn6search1 |
| **Compliance** | Danger/automation checks ensure changelog updates, docs/tests, and release labels before merge. | citeturn9search3 |

---

## 5. Merge queue operations

GitHub’s merge queue bundles approved PRs, runs merge-group CI (`merge_group` event), and keeps `main` green without manual rebases.citeturn0search2turn0search4 Track wait time and success rate—keep queue latency under 30 minutes for healthy repos and bisect quickly when a merge group fails.

1. Enable merge queue in branch protection, map required checks to `merge_group`, and set concurrency (start with 2–3 parallel groups).citeturn0search2
2. Automate freeze windows: pause queue during incidents/releases and announce via Slack/Teams bots.citeturn5search9
3. Investigate failures immediately—if a batch fails, remove the culprit and requeue remaining PRs so reviewers retain context.

---

## 6. AI assistance guardrails

Research shows graph-based recommenders like CORAL improve reviewer selection on large repositories, while modern auto-commenters reduce review effort when paired with human oversight.citeturn0academia12turn0academia13 Establish policy before enabling AI:

- Tag AI-authored commits and require humans to confirm the diff after running tests.citeturn0academia12
- Keep architectural decisions, security reviews, and approval authority with humans even when AI suggests changes.citeturn0academia13
- Log AI activity for compliance and rotate ownership so expertise spreads.

---

## 7. Implementation roadmap

| Sprint | Focus | Exit criteria |
| --- | --- | --- |
| **Sprint 1** | Branch protections & templates | Rulesets live, PR template merged, CODEOWNERS and labels automated.citeturn0search0turn0search2turn1search0 |
| **Sprint 2** | CI gates | Analyzer/test/secret scans blocking; device-cloud smoke tests wired into PR checks.citeturn2search3turn1search4turn0search10 |
| **Sprint 3** | Merge queue rollout | Merge queue enabled, metrics dashboard tracking latency/success, freeze automation documented.citeturn0search2turn5search9 |
| **Sprint 4** | AI augmentation | Reviewer rotation bot live; AI reviewer pilot with human sign-off & audit logging in place.citeturn0search2turn0academia12turn0academia13 |

---

## Checklist before enabling auto-merge

- [ ] Branch rulesets/policies enabled on `main` and `release/*`.citeturn0search0turn0search2
- [ ] Required checks (analyzers, tests, security scans, device-cloud tests) succeed on `merge_group` runs.citeturn0search2turn1search4turn0search10
- [ ] Merge queue latency and success rate monitored with alerting.citeturn0search4
- [ ] AI-generated changes tagged and validated by humans before merge.citeturn0academia12turn0academia13

---

## Further reading

- GitHub Docs – **About merge queues** & **Managing a merge queue**.citeturn0search2turn0search4
- GitHub Docs – **About protected branches** & **Creating rulesets for a repository**.citeturn0search0turn0search1
- Azure DevOps – **Protect your Git branches with policies**.citeturn2search9
- GitHub Blog – **How to write the perfect pull request**.citeturn2search5
- BrowserStack – **App Automate** documentation and CI integrations.citeturn3search2turn3search4


