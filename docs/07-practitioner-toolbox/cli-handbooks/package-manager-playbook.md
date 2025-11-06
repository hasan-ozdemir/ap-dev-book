---
title: winget & Chocolatey Playbook
description: Operational guidance for Windows package automation with winget 1.12+ and Chocolatey CLI 2.5+.
last_reviewed: 2025-10-31
owners:
  - @prodyum/delivery-enablement
---

# winget & Chocolatey Playbook

Windows developers at Prodyum rely on two complementary package managers:

- **winget** (Windows Package Manager) ships with Windows 11 and Windows 10 23H2+ and, as of version 1.12 (August 2025), supports configuration manifests, import/export, and cross-tenant policy controls.citeturn0search0
- **Chocolatey CLI 2.5.0** (February 2025) delivers enterprise packaging, private feeds, and delta downloads while improving TLS defaults.citeturn1search0turn1search4

This playbook standardises installation, upgrades, and security practices for both tools across developer workstations and CI agents.

---

## 1. Install or update winget

winget is preinstalled on Windows 11 and modern Windows 10 builds. If you need to bootstrap it manually, install the latest **App Installer** from the Microsoft Store or download the MSIX from the GitHub release page.citeturn0search0

Verify the CLI:

```powershell
winget --info
winget upgrade
```

### Everyday winget commands

| Goal | Command | Notes |
| --- | --- | --- |
| Find packages | `winget search <name>` | Filter by `--source winget` or `--source msstore`.citeturn2search0 |
| Install | `winget install --id Git.Git --scope machine` | `--scope` chooses per-user vs machine installs; combine with `--silent` for CI.citeturn2search0 |
| Upgrade all | `winget upgrade --all --include-unknown` | Captures packages without version metadata.citeturn2search0 |
| Export inventory | `winget export --output packages.json --include-versions` | Share a curated package list with teammates or pipeline images.citeturn3search0 |
| Import inventory | `winget import --import-file packages.json` | Restores the exported list on a fresh machine.citeturn3search0 |

### Configuration-as-code

`winget configure` applies DSC-like manifests that install packages, enable Windows features, and run scripts:

```powershell
winget configure --file .\dev-machine.dsc.yaml --accept-configuration-agreements
```

- Use `winget configure export` on a known-good machine to capture its state.citeturn4search0
- Pair configuration manifests with CI base images so new laptops reach a “ready to code” baseline in minutes.

### Policies & trust

- Enable source pinning: `winget settings --enable sourcePinning` to prevent untrusted source swaps.citeturn2search0
- `winget settings --open` edits the JSON file where you define telemetry, default scope, and execution policies.

---

## 2. Install or update Chocolatey CLI

Install Chocolatey with the official script (run from an elevated PowerShell prompt):

```powershell
Set-ExecutionPolicy Bypass -Scope Process -Force
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
```

Chocolatey CLI 2.5.0 introduced faster delta downloads, improved progress reporting, and native TLS 1.3 support while remaining compatible with existing repositories.citeturn1search0turn1search6

Verify:

```powershell
choco --version
choco feature list
```

### Everyday Chocolatey commands

| Goal | Command | Notes |
| --- | --- | --- |
| Install | `choco install vscode --confirm` | Use `--confirm` (or enable the `allowGlobalConfirmation` feature) in automation.citeturn1search5 |
| Upgrade all | `choco upgrade all --confirm --exit-when-reboot-detected` | Combine with scheduled tasks to keep desktops patched.citeturn1search5 |
| List outdated | `choco outdated` | Review impact before bulk upgrades.citeturn1search5 |
| Cache packages | `choco download <id> --internalize --source=https://community.chocolatey.org/api/v2/` | Internalise packages for offline/private feeds.citeturn1search4 |
| Upgrade Chocolatey | `choco upgrade chocolatey --confirm` | Keeps the CLI itself on the latest servicing build.citeturn1search5 |

### Features & configuration

- Enable global confirmations in CI images: `choco feature enable -n allowGlobalConfirmation`.citeturn1search5
- Enforce checksum validation by keeping the default `checksum` metadata in internal packages; Chocolatey aborts installs when hashes do not match.citeturn1search5
- Redirect caches for large build machines: `choco config set cacheLocation D:\ChocoCache`.citeturn1search5
- Chocolatey Central Management (licensed) orchestrates update rings and compliance reporting across fleets—the 2.5.x releases improve syncing with private repositories.citeturn1search0

---

## 3. Security & auditing checklist

| Tool | Action | Why |
| --- | --- | --- |
| winget | `winget install <id> --accept-package-agreements --accept-source-agreements` | Records explicit acceptance in CI logs for compliance.citeturn2search0 |
| winget | `winget settings --enable sourcePinning` | Prevents silent source swaps that could introduce malicious packages.citeturn2search0 |
| Chocolatey | `choco outdated --ignore-pinned` | Review breaking changes before mass upgrades; unpin after validation.citeturn1search5 |
| Chocolatey | Maintain `checksum` metadata in internal packages | Installs fail when hashes mismatch, reducing supply-chain risk.citeturn1search5 |

Embed these steps in onboarding and monthly patch runbooks so every machine follows the same governance.

---

## Quick reference

```powershell
# Install VS Code with winget
winget install --id Microsoft.VisualStudioCode --scope machine --silent

# Export current machine packages
winget export --output winget-packages.json --include-versions

# Apply a configuration manifest
winget configure --file .\dev-machine.dsc.yaml --accept-configuration-agreements

# Install Chocolatey (elevated PowerShell)
Set-ExecutionPolicy Bypass -Scope Process -Force
iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

# Install and upgrade packages with Chocolatey
choco install git --confirm
choco upgrade all --confirm --exit-when-reboot-detected

# Cache Chocolatey packages for offline feeds
choco download dotnet-aspnetruntime --internalize --source https://community.chocolatey.org/api/v2/
```

Keep this playbook close when you build Windows developer images, automate laptop provisioning, or bake tools into CI/CD runners.citeturn0search0turn1search5
