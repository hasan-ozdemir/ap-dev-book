---
title: Build Health & Branch Policies
description: Keep mainline builds green with branch protections, pre-commit automation, and resilient CI pipelines.
last_reviewed: 2025-10-31
owners:
  - @prodyum/devops-guild
---

# Build Health & Branch Policies

Healthy builds are the heartbeat of continuous delivery. This playbook explains how to harden your main branches, wire quality gates into every commit, and respond quickly when pipelines fail.

---

## 1. Branch protection principles

| Practice | Why it matters | GitHub / Azure DevOps implementation |
| --- | --- | --- |
| **Rulesets & protection** | Prevents direct pushes and enforces consistent review/CI policies across repositories.citeturn0search0turn0search2 | GitHub: Branch rulesets (`Settings → Code and automation → Rules → Rulesets`). Azure DevOps: Branch policies under Repos → Branches. |
| **Status checks** | Blocks merges until required builds/tests pass and branches are up to date.citeturn0search0 | Require named checks (for example `ci/build`, `codeql`, `sonar`) and enable **Require branches to be up to date before merging**. |
| **Review requirements** | Ensures independent approvals and knowledge sharing before code lands in protected branches.citeturn0search0 | Set minimum reviewers, require CODEOWNERS for sensitive paths, and dismiss stale reviews on new commits. |
| **Signed commits & linear history** | Guards against tampering and keeps the timeline easy to revert.citeturn0search0turn0search6 | Enable **Require signed commits** alongside **Non fast-forward merges**/**Linear history** rules. |
| **Merge queue** | Serialises merges after checks pass so mainline stays green without manual rebases.citeturn0search3turn0search4 | GitHub: Enable merge queue in branch protection. Azure DevOps: configure build validation with automatic completion. |

> Tip: Document protected-branch rules in an onboarding guide so contributors understand why certain checks are mandatory.

---

## 2. Build health best practices

### 2.1 Always-green main

- Blocking checks must run on every PR. If a check fails, fix it or revert quickly—never disable the check.
- Establish a “red build response” rotation: the on-call engineer triages within 15 minutes.
- Track MTTR (mean time to restore) for build failures; DORA reports elite performers restore service in under one hour, an excellent benchmark for mean time to green.citeturn1search0

### 2.2 Build validation gates

- Trigger validation on PR creation and updates; require revalidation when the branch lags behind `main`.
- For release branches, add extra gates (release checklist, smoke tests, security scans).
- For long-running feature branches, consider daily rebases to reduce integration risk.

### 2.3 On-demand test matrix

- Use CI matrix jobs (Android, iOS, Windows) so cross-platform regressions surface before merge.
- Quarantine flaky suites but keep reporting enabled; file issues and prioritise stabilisation.
- Run static analysis (formatting, analyzers, secret scans) in parallel to shorten feedback loops.

---

## 3. Pre-commit hook workflow

Stop noisy diffs and simple regressions before they reach CI by installing `pre-commit` hooks.

```yaml
# .pre-commit-config.yaml
repos:
  - repo: https://github.com/pre-commit/pre-commit-hooks
    rev: v4.6.0
    hooks:
      - id: check-added-large-files
      - id: end-of-file-fixer
      - id: trailing-whitespace
  - repo: local
    hooks:
      - id: dotnet-format
        name: dotnet format
        entry: dotnet format --verify-no-changes
        language: system
        types: [csharp]
      - id: dotnet-test-fast
        name: dotnet test (fast)
        entry: dotnet test --filter "Category!=Slow"
        language: system
        pass_filenames: false
```

```bash
pipx install pre-commit
pre-commit install
pre-commit install --hook-type pre-push
```

Document the workflow in onboarding guides and mirror the same checks in CI so local and server-side enforcement stay aligned.citeturn2search0

---

## 4. Branching strategies

| Strategy | When to use | Guidelines |
| --- | --- | --- |
| **Trunk-based** | Small, high-velocity teams shipping daily. | Feature flags for incomplete work; keep branches <3 days. |
| **Short-lived feature branches** | Medium teams that need code review and CI before merge. | Rebase frequently, enforce PR templates, auto-close stale PRs. |
| **Release branches** | Regulated releases or coordinated launches. | Freeze features, cherry-pick hotfixes, tag releases, add extra validation. |

Regardless of strategy:

- Adopt branching conventions (`feature/*`, `bugfix/*`, `release/*`).
- Automate version bumping/changelog generation (GitVersion, Nerdbank.GitVersioning).
- Protect `main`/`release/*` with the rules defined above.

---

## 5. Build health runbook

1. **Detect**: Build fails → notify the responsible squad (Slack/Teams + email).citeturn1search0
2. **Diagnose**: Review logs and reproduce locally (`dotnet build`, `dotnet test`); acknowledge within 15 minutes.
3. **Repair**: Fix forward or revert the offending commit; avoid force pushes that hide history.
4. **Learn**: Update the scorecard, log root cause in the build incident register, and automate guardrails so the issue cannot recur.

---

## 6. Quick reference

- Create branch rulesets with required reviews, status checks, signed commits, and non fast-forward merges.citeturn0search0turn0search2turn0search6
- Adopt merge queues (or equivalent build validation) for mainline stability.citeturn0search3turn0search4
- Enforce pre-commit hooks for formatting/analyzers and mirror them in CI.citeturn2search0
- Track MTTR, change failure rate, deployment frequency, and lead time—the DORA Four Key Metrics—for transparency with stakeholders.citeturn1search0

---

## Further reading

- GitHub Docs: **Available rules for rulesets** and **Creating rulesets for a repository**.citeturn0search0turn0search2
- GitHub Docs: **Managing a merge queue** and **Merging a pull request with a merge queue**.citeturn0search3turn0search4
- DORA Research: **Accelerate State of DevOps** (Four Key Metrics).citeturn1search0
- CSharpier documentation: **Pre-commit hook**.citeturn2search0

