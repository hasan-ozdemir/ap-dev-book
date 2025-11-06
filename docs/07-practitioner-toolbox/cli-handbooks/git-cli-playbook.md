---
title: Git CLI Playbook
description: End-to-end command-line guidance for configuring, securing, and scaling Git workflows for .NET and .NET MAUI delivery teams.
last_reviewed: 2025-10-31
owners:
  - @prodyum/delivery-enablement
---

# Git CLI Playbook

This playbook distils the Git command-line techniques Prodyum engineers use while shipping .NET 9 and .NET MAUI solutions. It layers rapid environment setup, secure identities, partial-clone productivity, and the Git 2.49+ feature set so squads keep delivery moving without adding risk.citeturn1search0turn1search2turn1search3

---

## 1. Install & upgrade Git everywhere

- **Windows (winget/App Installer)**  
  ```powershell
  winget install --id Git.Git -e
  winget upgrade Git.Git
  ```
  WinGet ships the supported Git for Windows build and exposes an `upgrade` path so patched releases arrive without manual downloads.citeturn3search0
- **macOS (Homebrew)**  
  ```bash
  brew install git
  brew upgrade git
  ```
  Homebrew keeps Git for macOS aligned with the latest bottles.citeturn3search1
- **Linux distributions**  
  ```bash
  sudo apt install git
  sudo apt upgrade git
  ```
  Install through the distro package manager (APT, dnf, zypper, pacman) so security updates flow with OS patches.citeturn3search2
- **Verify minimum version**  
  ```bash
  git --version
  ```
  Target Git 2.49.1 or newer for zlib-ng packing, `git backfill`, and CVE-2025-48957 fix coverage.citeturn1search2turn1search4

> **Tip**  
> Run `git maintenance start --schedule=daily` on developer workstations; Git registers OS tasks that refresh commit-graphs, run incremental GC, and fetch quietly in the background.citeturn0search1

---

## 2. First-time configuration & identity hygiene

1. **Set author metadata & safe defaults**
   ```bash
   git config --global user.name "First Last"
   git config --global user.email "you@example.com"
   git config --global init.defaultBranch main
   git config --global pull.ff only
   ```
   Configure your identity and defaults immediately to keep history attributable and aligned with Prodyum's branching baseline.citeturn1search0turn4search1

2. **Enforce signed commits**
   ```bash
   git config --global gpg.format ssh        # or openpgp
   git config --global commit.gpgsign true
   git config --global user.signingkey <fingerprint>
   ```
   Signing with SSH or GPG keys marks commits as **Verified**, satisfying the organisation's supply-chain controls.citeturn4search2

3. **Adopt passkeys or hardware-backed MFA**  
   Register platform or roaming passkeys with GitHub and keep at least two authenticators enrolled so credential theft cannot hijack CLI usage.citeturn4search3

4. **Upgrade Git Credential Manager (GCM)**  
   Install GCM 2.6.2 or later (patched for CVE-2024-50338) to block newline spoofing attacks that can exfiltrate PATs during clone flows.citeturn1search3turn4search4

---

## 3. Daily flow: clone → branch → review

### 3.1 Clone options

- **Standard clone**
  ```bash
  git clone https://dev.azure.com/<org>/<project>/_git/<repo>
  ```
  Azure DevOps serves full, blobless, and treeless clones from the same endpoint, so you can tailor transport per device.citeturn2search0
- **Blobless partial clone for MAUI monorepos**
  ```bash
  git clone --filter=blob:none <url>
  git backfill --min-batch-size=5000        # hydrate hotspots later
  ```
  Partial clones slash download size; Git 2.49’s `git backfill` hydrates missing blobs when history-heavy commands need them.citeturn1search2turn1search4
- **Thin CI clone**
  ```bash
  git clone --revision refs/heads/main --depth 10 <url>
  ```
  The `--revision` option lets automation fetch only the target ref—ideal for ephemeral CI agents.citeturn1search4

### 3.2 Structured branch workflow

```bash
git switch -c feature/login-refresh
git status --short
git add src/
git commit -m "feat: refresh login screen for MAUI"
git push --set-upstream origin feature/login-refresh
```
`git switch` and `git status --short` keep feature development tight while delivering review-ready commits.citeturn4search1

### 3.3 Sync safely

- `git fetch --prune` before rebasing to clear stale refs.citeturn4search1
- `git pull --ff-only` to prevent unwanted merge commits on integration branches.citeturn4search1
- `git push --atomic origin feature/login-refresh main` when updating multiple refs so the server stays consistent.citeturn4search1

---

## 4. Handling large repositories

| Scenario | Recommended command | Why it helps |
|---|---|---|
| Load only active folders | `git sparse-checkout set src MauiApp` | Cone-mode sparse checkout keeps worktrees minimal for MAUI monorepos and Azure Pipelines sparse checkout.citeturn1search1 |
| Parallel feature streams | `git worktree add ../hotfix hotfix/2025-10-31` | Isolates hotfixes without juggling stashes or dirty trees.citeturn4search1 |
| Hydrate assets after baseline clone | `git backfill --sparse` | Uses Git 2.49’s `git backfill` to hydrate only referenced assets post-clone.citeturn1search2turn1search4 |
| Speed up pack operations | `git config --global pack.useSparse true` | zlib-ng packing plus sparse heuristics reduce CPU time during fetch/push.citeturn1search4 |

---

## 5. Background maintenance & automation

- **Enable scheduled maintenance**
  ```bash
  git maintenance start --schedule=daily
  ```
  Registers OS schedulers to run incremental maintenance so repositories stay responsive.citeturn0search1
- **Reduce fetch noise in pipelines**  
  Configure Azure Pipelines `checkout: self` with `sparseCheckoutPatterns` to avoid cloning entire monorepos in CI.citeturn2search0
- **Monitor repository health**  
  Schedule nightly `git fsck --strict` jobs; catch dangling objects before they impact release branches.citeturn4search1

---

## 6. Security posture & incident response

1. **Stay ahead of CVEs** – Patch workstations, build agents, and containers to Git 2.49.1/2.50.1+ to neutralise CVE-2025-48957 before CISA’s 15 Sep 2025 deadline.citeturn1search2turn1search4
2. **Harden credential flows** – Restrict PAT issuance in Azure DevOps, prefer Entra OAuth tokens, and upgrade GCM; ensure `credential.useHttpPath true` isolates secrets per repo.citeturn2search0turn1search3turn4search4
3. **Submodule safety checklist** – Avoid recursive clones on untrusted repos, set `protocol.version 2`, and treat `.gitmodules` edits like code.citeturn4search1
4. **Credential & passkey hygiene** – Require passkey-backed MFA and run `git credential-manager erase https://dev.azure.com/<org>` during offboarding.citeturn4search3turn4search4

---

## 7. Troubleshooting quick wins

| Symptom | Fix |
|---|---|
| `git` not recognised after `winget install` | Sign out or add `%ProgramFiles%\\Git\\cmd` to `PATH`; WinGet issue #2815 tracks automatic PATH updates.citeturn3search0 |
| Partial clone commands hang | Confirm the remote supports filters (Azure DevOps enables blobless/treeless clones) and rerun `git backfill` to hydrate required blobs.citeturn2search0turn1search2 |
| Signed commits show “Unverified” | Align signing key email with the verified identity and leave commit signing enabled globally.citeturn4search2

---

## 8. Embed Git discipline in the team

- **Definition of Done:** feature branch merged, CI green, signed commits, release tagged with an annotated signature.citeturn4search1turn0search3
- **Onboarding checklist:** install Git/GCM, configure identity & signing, clone the seed repo, exercise partial clone + `git backfill`.citeturn1search2
- **Runbooks:** capture the commands above in automation scripts (`scripts/git/bootstrap.ps1`) so new devices become productive quickly.citeturn4search1

Keep this playbook handy while we extend the CLI series with `.NET`, PowerShell, Azure CLI, Node/npm, Python, winget/Chocolatey, and cross-platform shell guides.citeturn3search0turn3search3
