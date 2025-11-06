---
title: Node & npm CLI Playbook
description: Install, manage, and secure the Node.js + npm toolchain for MAUI-friendly JavaScript workflows on Windows, macOS, and Linux.
last_reviewed: 2025-10-31
owners:
  - @prodyum/delivery-enablement
---

# Node & npm CLI Playbook

Node.js 22.x (codename “Jod”) has been the Active LTS line since 29 October 2024 and will receive security fixes until 30 April 2027. Node.js 24.x became the Current line in May 2025 and will enter Active LTS in October 2025.citeturn0search0turn0search1 This guide standardises how Prodyum teams install, upgrade, and automate the Node + npm toolchain when building MAUI front ends, hybrid Blazor shells, and supporting services.

---

## 1. Install & verify Node.js

| Platform | Command | Notes |
| --- | --- | --- |
| Windows | `winget install --id OpenJS.NodeJS.LTS -e` | Installs the latest LTS MSI (22.x as of Oct 2025) via WinGet; rerun `winget upgrade OpenJS.NodeJS.LTS` during patch windows.citeturn1search0 |
| macOS | `brew install node@22` (LTS) or `brew install node` (Current 24.x) | Homebrew bottles both streams; `brew link --overwrite node@22` pins LTS while leaving Current for experimental work.citeturn1search1turn1search4 |
| Ubuntu / Debian | ```bash\ncurl -fsSL https://deb.nodesource.com/setup_22.x | sudo -E bash -\nsudo apt-get install -y nodejs\n``` | NodeSource maintains apt repositories for Node 22 LTS and newer; they include npm 10.citeturn0search2turn0search3 |
| Containers | `docker pull node:22-bullseye` | Official Docker Hub images track nightly, Current, and LTS releases—use slim tags to shrink CI images.citeturn2search0 |

After installation:

```bash
node --version
npm --version
corepack --version
```

Pin the expected `node --version` in onboarding docs so CI fails fast if agents drift.

---

## 2. Manage multiple Node releases

### 2.1 nvm (macOS, Linux, WSL)

```bash
curl -fsSL https://raw.githubusercontent.com/nvm-sh/nvm/v0.40.3/install.sh | bash
nvm install 22
nvm use 22
```

nvm 0.40.3 (released 23 April 2025) adds Node 22 compatibility and remains the de facto POSIX manager. Commit an `.nvmrc` file per repo (`22.11.0` etc.) to enforce the desired version.citeturn1search2

### 2.2 nvs (Windows-first, cross-platform)

```pwsh
winget install jasongin.nvs
nvs add lts
nvs use lts
```

Node Version Switcher offers per-shell shims, remote feeds, and auto switching for `.node-version` / `.nvmrc` files across PowerShell, macOS, and Linux.citeturn1search1

### 2.3 Volta (project-level pinning)

```bash
curl https://get.volta.sh | bash
volta install node@22
volta pin node@22 pnpm@9.12.0
```

Volta caches toolchains once and enforces per-project engines—ideal for CI runners or engineers who prefer a single install.citeturn2search1

---

## 3. Align Node with MAUI & .NET workflows

- Hybrid templates such as `dotnet new maui-blazor` and updated SPA starters expect Node 20+; align Node 22 in local and CI environments before `dotnet publish`.citeturn0search0
- Add a pre-build target that runs `npm run build` and copies artefacts into `Resources/Raw` (or platform asset folders) so WebView content stays consistent with the mobile build pipeline.
- Document `npm scripts` alongside `dotnet` commands in READMEs and CI pipelines to maintain parity across developer workstations and hosted agents.

---

## 4. Package managers & Corepack

Node 22 ships with Corepack enabled by default, but the Node Technical Steering Committee plans to remove Corepack in Node 25—start pinning package managers explicitly.citeturn3search0

```bash
corepack enable
corepack prepare pnpm@9.12.0 --activate
```

- Set `COREPACK_ENABLE_AUTO_PIN=1` in CI so the `packageManager` field inside `package.json` governs the exact tool version.citeturn3search5
- For Node 25+, install package managers directly (`npm install -g pnpm`) or via Volta/Nix and remove `corepack` commands from automation scripts.

---

## 5. npm account security

GitHub now enforces passkey or hardware-backed MFA for maintainers of high-impact npm packages and has rolled out passkey sign-in for all publishers (January 2025). Enable passkeys for every npm account, publish with provenance (`npm publish --provenance`), and rotate automation tokens every 90 days.citeturn4news0

---

## 6. Operational runbook

| Task | Command | Cadence |
| --- | --- | --- |
| Verify installed versions | `node --version && npm --version` | After patch |
| Refresh npm cache | `npm cache clean --force` | Quarterly |
| Upgrade npm | `npm install -g npm@latest` or `corepack prepare npm@10.9.0 --activate` | Quarterly |
| Rotate npm tokens | `npm token list` / `npm token revoke <id>` | 90 days |
| Validate package manager pin | `corepack prepare` as part of CI bootstrap | Every pipeline |

Embed these commands in GitHub Actions (`setup-node`) or Azure DevOps templates so hosted agents stay aligned with developer machines.

---

## 7. Troubleshooting quick wins

| Issue | Fix |
| --- | --- |
| WinGet installs the legacy MSI after using nvs | Ensure `%LOCALAPPDATA%\nvs\node\<version>` precedes `C:\Program Files\nodejs` in `PATH`, then run `nvs link lts` so shims take precedence.citeturn1search1 |
| `corepack enable` fails on Node 25+ | Install package managers manually (`npm install -g pnpm`) or switch to Volta/Nix; Corepack is being removed starting Node 25.citeturn3search0 |
| `npm publish` blocked for missing 2FA | Add a passkey or hardware key to your npm account before reattempting the publish.citeturn4news0 |
| CI picks the wrong Node channel | Explicitly run `nvm use`, `nvs use`, or `volta pin` inside pipeline scripts before invoking npm commands. |

---

## Quick reference

```bash
# Install Node LTS (Windows)
winget install --id OpenJS.NodeJS.LTS -e

# Switch runtime (nvm)
nvm install 22 && nvm use 22

# Prepare pnpm with Corepack
corepack enable
corepack prepare pnpm@9.12.0 --activate

# Build MAUI web assets
npm ci
npm run build

# Security posture
npm audit --omit=dev
npm token list
```

Use this playbook alongside the Git, .NET, PowerShell, Azure CLI, and Python guides to keep your tooling consistent across the portfolio.citeturn2search1turn3search5
