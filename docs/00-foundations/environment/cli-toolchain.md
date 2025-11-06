---
title: CLI Toolchain & Automation
description: Standard command-line stack for .NET 9, MAUI, Azure, and frontend tooling across Windows, macOS, and CI environments.
last_reviewed: 2025-11-03
owners:
  - @prodyum/platform-engineering
---

# CLI Toolchain and Automation

Prodyum projects rely on command-line automation for versioning, testing, and deployment. The tooling below is the single source of truth for the versions we use on .NET 9 and .NET MAUI deliveries.

## 1. Core tools

| Tool | Standard version | Why it matters |
|------|------------------|----------------|
| Git | 2.51 (Aug 2025) | Cruft-free multi-pack indexes, stash interchange format, and other performance updates keep large repos responsive for CI and release automation.citeturn5search7 |
| .NET SDK | 9.0.306 STS | Current STS build for MAUI workloads; .NET 9 STS now receives 24 months of support, giving squads additional runway before upgrading.citeturn1search2turn7search2 |
| PowerShell | 7.5 | GA release focuses on security, quality, and PSResourceGet enhancements while aligning with .NET 9.citeturn1search1 |
| Azure CLI | 2.74+ | June 2025 release adds AKS improvements and ships with Azure Linux 3.0 base images used across Microsoft-hosted environments.citeturn1search3turn1search5 |
| Azure Developer CLI (azd) | 1.19+ | October 2025 update introduces layered provisioning, `azd publish`, and updated authentication tooling.citeturn2search0 |
| Node.js | 22 LTS | Node 22 entered LTS in October 2025; keep front-end shells on the 22.21.x channel for long-term support.citeturn5search5 |
| Python | 3.14 | Latest stable release with free-threaded runtime, deferred annotations, and other platform updates for data and automation scripts.citeturn3search0 |

## 2. Installation commands

### Windows (PowerShell)

```powershell
winget install --id Git.Git -e --source winget
winget install --id Microsoft.DotNet.SDK.9 -e
winget install --id Microsoft.PowerShell -e
winget install --id Microsoft.AzureCLI -e
winget install --id Microsoft.AzureDeveloperCLI -e
winget install --id OpenJS.NodeJS.LTS -e
winget install --id Python.Python.3.14 -e
choco install maui-check --pre
```

Azure CLI and azd packages are published on winget; run `winget upgrade --all` each sprint to stay within the supported release bands.citeturn1search3turn2search3

### macOS (zsh)

```bash
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
brew install git
brew install --cask dotnet-sdk
brew install --cask powershell
brew install azure-cli
brew install --cask azure-dev
brew install node@22
brew install python@3.14
```

The Homebrew formula tracks the Azure CLI 2.74+ channel—confirm `az version` after installation to ensure the CLI matches hosted images.citeturn1search3

### Linux (CI containers)

```bash
apt-get update && apt-get install -y git curl jq python3 python3-pip
curl -sSL https://aka.ms/install-dotnet.sh | bash /dev/stdin --version 9.0.306
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
curl -fsSL https://aka.ms/install-azd.sh | bash
```

Keep CI runners on Azure Linux 3.0 or Ubuntu 24.04 images so CLI and azd versions mirror Microsoft-hosted agents.citeturn1search5turn2search0

## 3. Validation steps

```powershell
git --version              # 2.51.x
dotnet --info              # 9.0.306, workload set enabled
dotnet workload update --print-rollback
pwsh -v                    # 7.5.x
az version                 # >= 2.74
azd version                # 1.19.x
node -v && npm -v          # v22.x / npm 10+
python3 --version          # 3.14.x
```

`dotnet workload update --print-rollback` now emits clean JSON output that you can commit for consistent workload sets across machines.citeturn2search8

## 4. Automation tips

- **Script repositories:** Store PowerShell (`pwsh`) and Bash scripts under `tools/`, and manage shared dependencies with `Directory.Packages.props` to keep automation modules in sync.citeturn9search0
- **Update cadence:** Refresh Git, .NET SDK, Azure CLI, and Node each sprint; .NET 9 STS now receives 24 months of support, so pin `global.json` to 9.0.306 and document upgrade checkpoints.citeturn1search2turn7search2
- **Polyglot projects:** Use `dotnet new tool-manifest` and `dotnet tool restore` so CLI tools such as `maui-check`, `dotnet-format`, and `azd` versions match across dev and CI.citeturn2search0turn2search8
- **Security posture:** Adopt Git 2.51’s SHA-256 default hashing and reftable backend to harden history integrity while improving fetch/push throughput.citeturn5search1turn5search7

This toolchain keeps Windows, macOS, and Linux environments on consistent versions. Track major upgrades in `docs/landing/roadmap.md` and add CI steps that verify the expected toolset.


