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
| Git | 2.51 (Aug 2025) | Includes sparse-checkout improvements and security fixes; optimised SSH certificate flows.?cite?turn8search0?turn8search6? |
| .NET SDK | 9.0.306 STS | Official STS build for MAUI projects, C# 13, and workload sets.?cite?turn3search2?turn11search2? |
| PowerShell | 7.5 | Ships with the latest .NET 9-based modules and PSResourceGet improvements.?cite?turn7search2? |
| Azure CLI | 2.74+ | Compatible with Azure Linux 3.0 containers; Mariner 2.0 support ends 1 July 2025.?cite?turn9search0?turn13search4? |
| Azure Developer CLI (azd) | 1.10 | Provides current commands for Azure Deployment Environments and GitHub Actions integrations.?cite?turn13search1? |
| Node.js | 22 LTS | Long-term support for Vite-based frontend shells and dev servers.?cite?turn7search6? |
| Python | 3.14 | Latest security-hardened runtime for data tooling and pipeline scripts.?cite?turn12search1? |

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

Azure CLI and azd packages are published by Microsoft on Winget, so run `winget upgrade --all` monthly to stay current.?cite?turn13search4?turn13search1?

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

The Homebrew formula tracks the Azure CLI 2.74+ STS channel; verify with `az version` after installation.?cite?turn13search4?

### Linux (CI containers)

```bash
apt-get update && apt-get install -y git curl jq python3 python3-pip
curl -sSL https://aka.ms/install-dotnet.sh | bash /dev/stdin --version 9.0.306
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
curl -fsSL https://aka.ms/install-azd.sh | bash
```

Keep CI runners on Azure Linux 3.0 or Ubuntu 24.04 images to align CLI versions with the support window.?cite?turn9search0?turn13search4?

## 3. Validation steps

```powershell
git --version              # 2.51.x
dotnet --info              # 9.0.306, workload set enabled
dotnet workload update --print-rollback
pwsh -v                    # 7.5.x
az version                 # >= 2.74
azd version                # 1.10.x
node -v && npm -v          # v22.x / npm 10+
python3 --version          # 3.14.x
```

`dotnet workload update --print-rollback` exports workload-set versions to JSON so the rollback file stored at the repo root keeps CI and developer machines aligned.?cite?turn11search0?

## 4. Automation tips

- **Script repositories:** Store PowerShell (`pwsh`) and Bash scripts separately under `tools/`; create Python virtual environments with `python -m venv .venv`.?cite?turn12search1?
- **Update cadence:** Refresh Git, .NET SDK, Azure CLI, and Node each sprint; pin SDK versions in `global.json` because .NET 9 STS support lasts 18 months.?cite?turn3search0?turn3search2?
- **Polyglot projects:** Use `dotnet new tool-manifest` and `dotnet tool restore` to keep local and CI tooling in sync; managing tools such as `maui-check` and `dotnet-format` via the manifest improves build consistency.?cite?turn11search2?
- **Security scanning:** Enable Git 2.51 SSH certificate enhancements and signed commits; configure `git config --global gpg.program` to automate code-signing flows.?cite?turn8search6?

This toolchain keeps Windows, macOS, and Linux environments on consistent versions. Track major upgrades in `docs/landing/roadmap.md` and add CI steps that verify the expected toolset.
