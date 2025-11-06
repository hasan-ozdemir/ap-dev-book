---
title: PowerShell CLI Playbook
description: Cross-platform installation, hardening, and daily operations guidance for PowerShell 7.5 and later.
last_reviewed: 2025-10-31
owners:
  - @prodyum/delivery-enablement
---

# PowerShell CLI Playbook

PowerShell 7.5 is the current Standard-Term Support (STS) release built on .NET 9, delivering 18 months of support, hardened defaults, and productivity features such as enhanced predictive IntelliSense and new experimental switches.citeturn11search1 Use this playbook to install pwsh on every platform we target, keep it patched, manage modules with PSResourceGet, and run secure remoting in automated pipelines.

---

## 1. Understand the release window

- **Support model** – PowerShell 7.5 (STS) receives 18 months of updates; PowerShell 7.4 remains LTS through November 2026. Plan migrations so long-lived environments track LTS while developer workstations leverage the faster STS cadence.citeturn11search1
- **Security posture** – Monitor advisories like CVE‑2025‑21171 (affecting pre‑GA builds) and roll forward to patched 7.5 GA or newer immediately to avoid NTLM relay exposure.citeturn15search3
- **Feature highlights** – Validate new predictive settings (`PSCommandNotFoundSuggestion`, `PSModuleAutoLoadSkipOfflineFiles`) and experimental toggles (`PSRedirectToVariable`, `PSNativeWindowsTildeExpansion`, `PSSerializeJSONLongEnumAsNumber`) as part of regression testing.citeturn11search1

---

## 2. Install and pin PowerShell

### 2.1 Windows (desktop, server, hosted agents)

| Scenario | Command | Notes |
| --- | --- | --- |
| Standard install | `winget install --id Microsoft.PowerShell --source winget` | Downloads the correct MSI/MSIX for the architecture via the Store-backed feed.citeturn12view0 |
| Quiet CI install | `msiexec.exe /i PowerShell-7.5.x-win-x64.msi /quiet ADD_PATH=1 ENABLE_PSREMOTING=1 USE_MU=1` | Combine MSI properties to append PATH, enable remoting, and enroll Microsoft Update servicing.citeturn12view0 |
| Microsoft Update servicing | Set `USE_MU=1 ENABLE_MU=1` during install or via Settings to deliver pwsh patches through WSUS/WUfB alongside OS baselines.citeturn12view0 |
| Side-by-side preview | `winget install Microsoft.PowerShell.Preview` (installs into `Program Files\PowerShell\7-preview`).citeturn12view0 |

**Post-install smoke test**

```pwsh
pwsh -NoLogo -Command '$PSVersionTable | Format-List'
Get-Command pwsh -CommandType Application
```

Add the check to build-agent provisioning scripts so pipelines fail fast if the wrong engine is on PATH.

### 2.2 Linux (Ubuntu example) and containers

| Scenario | Command | Notes |
| --- | --- | --- |
| Ubuntu 24.04 via Microsoft repo | ```bash\nwget -q https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb\nsudo dpkg -i packages-microsoft-prod.deb\nsudo apt-get update && sudo apt-get install -y powershell\n``` | Installs official Microsoft packages so future `apt upgrade` delivers security fixes.citeturn19search4 |
| Snap install | `sudo snap install powershell --classic` | Fast-track deployment on snap-enabled distros (stable channel currently amd64).citeturn18search2 |
| Container automation | `docker pull mcr.microsoft.com/powershell` | Use the Microsoft Container Registry image for ephemeral automation and CI steps.citeturn18search5 |

### 2.3 macOS (Apple Silicon and Intel)

| Scenario | Command | Notes |
| --- | --- | --- |
| Homebrew install | `brew install powershell/tap/powershell` | Homebrew manages dependencies and PATH; upgrades track new STS releases.citeturn19search5 |
| Managed Macs / air-gapped | Download the notarised `.pkg` from the release page and distribute via MDM; notarise internally if Gatekeeper policies require.citeturn19search2 |

> **Shell tip:** Add `alias pwsh='/usr/local/bin/pwsh'` (or the Homebrew prefix) for shells that do not source `/usr/local/bin` automatically.

---

## 3. Keep pwsh patched

- **Windows** – `winget upgrade --id Microsoft.PowerShell` or rerun the MSI in quiet mode when MU servicing is unavailable.citeturn12view0
- **Linux/macOS** – Include `apt-get upgrade powershell`/`brew upgrade powershell` in the monthly patch cadence; rebuild containers from the latest `mcr.microsoft.com/powershell` tag.citeturn19search4turn18search5
- **Security monitoring** – Subscribe to the PowerShell GitHub “Announcements” feed so CVEs and servicing releases trigger regression runs; enforce `pwsh -v` checks against your approved manifest before deployment.citeturn15search3

---

## 4. Manage modules with PSResourceGet

PowerShell 7.5 ships with Microsoft.PowerShell.PSResourceGet as the default package management module, superseding PowerShellGet v2 for discovery, install, update, and publish flows.citeturn8search2

### 4.1 Baseline configuration

```pwsh
Import-Module Microsoft.PowerShell.PSResourceGet
Register-PSResourceRepository -Name PSGallery -Uri https://www.powershellgallery.com/api/v2 -Trusted
Set-PSResourceRepository -Name PSGallery -InstallationPolicy Trusted
```

- Use `Register-PSResourceRepository` for internal mirrors (Artifactory, ACR).citeturn8search2
- `Get-InstalledPSResource` replaces `Get-InstalledModule`; run it in health checks to spot drift.citeturn8search2

### 4.2 Install or update

```pwsh
Install-PSResource Az -Scope AllUsers -TrustRepository
Update-PSResource Az -Scope AllUsers
```

Use `-Prerelease` for preview feeds and `-TrustRepository` only after repository policies are assessed.

### 4.3 Publish to internal feeds

```pwsh
Publish-PSResource -Path ./Artifacts/MyModule -Repository ContosoGallery -ApiKey (Get-Secret ACR_PSGallery_Key)
```

Pair with Azure Key Vault or GitHub OIDC secrets so tokens never appear in plaintext.citeturn4search2turn8search2

---

## 5. Remoting strategies (SSH & WS-Man)

### 5.1 Windows remoting over SSH

```pwsh
Add-WindowsCapability -Online -Name OpenSSH.Server~~~~0.0.1.0
Start-Service sshd
Set-Service sshd -StartupType Automatic
Add-Content -Path $env:ProgramData\ssh\sshd_config -Value 'Subsystem powershell C:/Program Files/PowerShell/7/pwsh.exe -sshs -NoLogo -NoProfile'
```

- Require host keys signed by your enterprise CA (`ssh-keygen -lf` to audit).citeturn10search1
- Restrict inbound users with `AllowUsers`/`Match` blocks; audit connections via Windows Event Log 400 series (OpenSSH/Operational).citeturn10search1

### 5.2 Linux/macOS remoting

- Install OpenSSH server (`sudo apt install openssh-server`) and configure `Subsystem powershell /usr/bin/pwsh -sshs -NoLogo -NoProfile` to enable cross-platform session sharing.citeturn19search4
- Use certificate- or hardware-backed keys plus `ssh -o PubkeyAuthentication=yes` to enforce MFA-backed remoting.

### 5.3 WS-Man considerations

- Windows PowerShell remoting (WinRM) persists for legacy hosts; run `Enable-PSRemoting -SkipNetworkProfileCheck -Force`, but prefer HTTPS listeners with enterprise certificates and JEA endpoints when possible.citeturn10search1

---

## 6. Automation integrations

| Workflow | Suggested command | Why it matters |
| --- | --- | --- |
| GitHub Actions runner | `pwsh` steps reference cached modules via PSResourceGet and respect `Invoke-PSResourceCollection`.citeturn8search2 |
| Azure DevOps pipeline | `task: PowerShell@2` with `pwsh: true` ensures cross-platform agents run PowerShell 7+.citeturn2search1 |
| Build agent baseline | Script `winget install Microsoft.PowerShell` + PSResourceGet bootstrap to keep images reproducible.citeturn12view0turn8search2 |
| Containers | Use `FROM mcr.microsoft.com/powershell` in Dockerfiles, layer modules with PSResourceGet, and copy scripts under `/opt/microsoft/powershell/7`.citeturn18search5turn8search2 |

---

## 7. Maintenance checklist

- Verify engine version with `Get-Command pwsh | Select-Object Source` across the fleet after patch windows.citeturn12view0
- Audit installed resources monthly: `Get-InstalledPSResource | Sort-Object Name`.citeturn8search2
- Clean prerelease modules once preview cycles end: `Uninstall-PSResource -Name <Module> -Prerelease`.citeturn8search2
- Refresh PSReadLine quarterly (`Install-PSResource PSReadLine -Version 2.3.6 -Reinstall`) to pick up console and completion fixes.citeturn8search2
- Rotate registry/service credentials (e.g., ACR tokens) and re-register repositories with `Set-PSResourceRepository`.citeturn8search2turn4search2

Embed these steps in GitHub Actions or Azure DevOps templates so every pipeline shelling out to pwsh runs on a compliant engine.

---

## 8. Troubleshooting quick wins

| Symptom | Resolution |
| --- | --- |
| `pwsh` not found on PATH | Verify `Program Files\PowerShell\7` (Windows) or `/usr/local/bin` (macOS/Homebrew). Re-run the installer with `ADD_PATH=1`.citeturn12view0turn19search5 |
| SSH remoting handshake fails | Check host fingerprints (`ssh-keygen -lf`), confirm the `Subsystem powershell` entry, and review Windows firewall rules (`Get-NetFirewallRule -DisplayGroup "OpenSSH Server"`).citeturn10search1turn5search3 |
| Module drift across environments | `Find-PSResource -Repository PSGallery -Name <Name> -Version (Get-InstalledPSResource <Name>).Version` to compare versions and remediate.citeturn8search2 |
| Package-manager conflicts | On Linux freeze a specific version with `apt-mark hold powershell`; manage exceptions by temporarily using the standalone installer.citeturn19search4 |

---

## Quick reference

```pwsh
# Install PowerShell (Windows)
winget install --id Microsoft.PowerShell --source winget

# Install PowerShell (Ubuntu)
sudo apt-get update && sudo apt-get install -y powershell

# Check version
pwsh -NoLogo -Command '$PSVersionTable.PSVersion'

# Enable Microsoft Update during MSI install
msiexec /i PowerShell-7.5.x-win-x64.msi /quiet USE_MU=1 ENABLE_MU=1

# Configure PSResourceGet repositories
Register-PSResourceRepository -Name PSGallery -Uri https://www.powershellgallery.com/api/v2 -Trusted

# Enable SSH subsystem on Windows
Add-Content -Path $env:ProgramData\ssh\sshd_config -Value 'Subsystem powershell C:/Program Files/PowerShell/7/pwsh.exe -sshs -NoLogo -NoProfile'
```

Use this sheet when bootstrapping developer images, provisioning build agents, or embedding PowerShell tasks within CI/CD pipelines.citeturn12view0turn19search4turn8search2
