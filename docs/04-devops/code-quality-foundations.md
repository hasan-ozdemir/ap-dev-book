---
title: Code Quality Foundations
description: Build a culture, toolchain, and roadmap that keeps .NET/.NET MAUI codebases healthy as teams scale.
last_reviewed: 2025-10-31
owners:
  - @prodyum/devops-guild
---

# Code Quality Foundations

High-quality code does not happen by accident—it is the result of aligned culture, reliable automation, and continuous learning. Use this guide to anchor your .NET and .NET MAUI teams on proven practices.

---

## 1. Quality pillars checklist

| Pillar | What “good” looks like | Leading indicators |
| --- | --- | --- |
| **Culture & collaboration** | Psychological safety, clear review etiquette, gratitude in feedback, and reviewers engaged early.citeturn0search3turn0search2 | Review turnaround < 1 business day, low stale-PR count, balanced reviewer load. |
| **Automation & coverage** | Tiered tests (unit/integration/UI) run on every push and gate merges; device-cloud tests replace legacy App Center labs.citeturn1search6turn1search4 | Test pass rate, flake rate backlog, coverage delta per PR, device-cloud utilisation. |
| **Static analysis & metrics** | Roslyn analyzers, code-style rules, and maintainability thresholds enforced in CI.citeturn2search0turn2search3 | Analyzer violations per build, maintainability index trend, unresolved warnings. |
| **Learning & improvement** | Retrospectives capture escaped-defect root causes, and teams track DORA quality metrics.citeturn1search0turn0search7 | MTTR, change failure rate, action-item completion rate. |

---

## 2. Static analysis and metrics

1. **Enable analyzers**: Add `EnableNETAnalyzers=true`, `AnalysisMode=AllEnabledByDefault`, and project-specific analyzer packages (StyleCop, Roslynator).citeturn2search0
2. **Set thresholds**: Use `CodeMetricsConfig.json` or `GlobalAnalyzerConfig` to enforce maintainability index and cyclomatic complexity limits; fail CI when thresholds are exceeded.citeturn2search3
3. **Pipe results into dashboards**: Publish SARIF outputs to GitHub Advanced Security or Azure DevOps code scanning so regression trends are visible.citeturn1search6

---

## 3. Automated testing gates

1. **Adopt tiered pipelines**: Run fast unit tests first, then integration/UI suites, mirroring Microsoft’s guidance for continuous testing.citeturn1search6turn1search3  
2. **Move device testing to cloud labs**: BrowserStack App Automate replaces retired App Center test clouds and offers 20k+ real devices for MAUI validation.citeturn1search4  
3. **Quarantine flakes, track MTTR**: Investigate flaky tests daily; report time-to-green alongside build MTTR so systemic issues surface quickly.citeturn1search0

---

## 4. Review excellence

- **Summarise context**: Encourage authors to explain the “why”, include screenshots for UX changes, and link work items.citeturn0search3  
- **Keep PRs small and timely**: Smaller, frequent PRs reduce review fatigue and improve defect capture.citeturn0search7  
- **Rotate reviewers**: Use rosters or AI suggestions to spread knowledge and avoid bottlenecks.citeturn0search2turn0academia12  
- **Celebrate feedback**: Reinforce positive review behaviour; highlight excellent reviews in retrospectives.citeturn0search3

---

## 5. AI and automation guardrails

| Capability | Why it matters | Guardrails |
| --- | --- | --- |
| **Reviewer recommendation (e.g., CORAL)** | Raises review relevance on large repos.citeturn0academia12 | Monitor adoption, false positives, and ensure human owners can override. |
| **Auto-commenters & co-pilots** | Spot style regressions, best-practice violations, and risky patterns.citeturn0academia13turn0search8 | Keep AI suggestions non-blocking; require human approval for architectural decisions. |
| **AI code review services** | Vendors offer bot-to-human blended reviews for compliance-heavy teams.citeturn0search5turn0search6 | Run privacy/compliance review, log AI activity, and keep manual sign-off in place. |

---

## 6. Implementation roadmap (first 4 sprints)

| Sprint | Focus | Exit criteria |
| --- | --- | --- |
| **Sprint 1** | Analyzer baseline | `.editorconfig` committed; analyzers fail CI on new violations; maintainability thresholds defined.citeturn2search0turn2search3 |
| **Sprint 2** | Test discipline | Tiered pipelines published; device-cloud smoke tests running; build break MTTR tracked.citeturn1search6turn1search4turn1search0 |
| **Sprint 3** | Review excellence | Review etiquette doc published; reviewer rotation automated; PR health dashboard live.citeturn0search3turn0search2 |
| **Sprint 4** | AI augmentation & metrics | Quality scorecard links analyzer/test data; AI reviewer pilot running with guardrails; secret/dependency scanning blocking merges.citeturn0academia13turn0search10turn1search6 |

---

## Further reading

- GitHub Engineering Playbook: **Review guidance and quality playbooks**.citeturn0search3
- GitHub Blog: **How small pull requests make big code reviews**.citeturn0search7
- Microsoft Learn: **Continuous integration / testing guidance**.citeturn1search6turn1search3
- BrowserStack: **Guide to automated mobile testing on real devices**.citeturn1search4
- Microsoft Learn: **.NET code-quality analysis and metrics**.citeturn2search0turn2search3
