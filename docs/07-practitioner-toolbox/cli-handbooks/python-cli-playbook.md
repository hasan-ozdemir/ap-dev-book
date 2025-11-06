---
title: Python CLI Playbook
description: Standardise Python 3.13+ installation, version management, and packaging workflows across Windows, macOS, and Linux.
last_reviewed: 2025-10-31
owners:
  - @prodyum/delivery-enablement
---

# Python CLI Playbook

Python 3.13.0 shipped on 7 October 2024 and remains in full support until 1 October 2026 (with security fixes through 31 October 2029). Python 3.14.0 landed on 7 October 2025 as part of the annual release cadence defined in PEP 602.citeturn0search0turn1search0 Use this playbook to keep Prodyum teams aligned on supported runtimes, modern packaging, and security expectations across development workstations and CI/CD.

---

## 1. Install & upgrade Python consistently

| Platform | Command | Notes |
| --- | --- | --- |
| Windows | `winget install 9NQ7512CXL7T` | Installs the Python Install Manager MSIX so you can add/remove interpreters via GUI or automation.citeturn2search0 |
| macOS | `brew install python@3.13` (LTS) or `brew install python@3.14` | Homebrew bottles both streams; link with `brew link --overwrite python@3.13`.citeturn2search1 |
| Ubuntu / Debian | ```bash\nsudo apt install software-properties-common -y\nsudo add-apt-repository ppa:deadsnakes/ppa -y\nsudo apt install python3.13 python3.13-venv -y\n``` | The Deadsnakes PPA provides modern CPython builds plus matching `-venv` packages.citeturn2search2turn2search3 |
| Containers | `docker pull python:3.13-bookworm` | Official Docker Library images track every CPython release; use `-slim` tags for lean CI runners.citeturn3search0 |

After installation run:

```bash
python3 --version
pip --version
pipx --version
```

Document the expected versions in onboarding guides so build agents fail fast when they drift.

---

## 2. Manage multiple Python versions

### pyenv (macOS, Linux, WSL)

```bash
curl -fsSL https://pyenv.run | bash
pyenv install 3.13.2
pyenv global 3.13.2
```

pyenv automates downloading, compiling, and shimming interpreters so every shell honours the `.python-version` committed to your repo.citeturn4search0

### Python Install Manager (Windows)

The Python Install Manager CLI (`python.exe /Manage`) lets you add or remove supported major/minor versions without hunting for MSI installers; Microsoft plans to make it the default starting with Python 3.16.citeturn2search0

### uv (cross-platform project runner)

```bash
curl -fsSL https://astral.sh/uv/install.sh | sh
uv venv
uv pip install -r requirements.txt
```

uv bundles interpreter downloads, dependency resolution, and virtual environments into a single fast CLI—ideal for ephemeral CI runners.citeturn5search0

---

## 3. Packaging & isolation workflow

- **Create dedicated environments** with `python3 -m venv .venv && source .venv/bin/activate` in every project to comply with PEP 668 and avoid polluting system packages.citeturn2search0turn6search0
- **Upgrade pip** to 25.3+ (`python3 -m pip install --upgrade pip`) to benefit from hardened build isolation and vulnerability fixes.citeturn6search1
- **Use pipx 1.8.1** for CLI utilities (`pipx install commitizen`) so tools stay isolated from application dependencies.citeturn7search0
- **Enable trusted publishing** (Sigstore-backed) for internal packages so provenance metadata accompanies every wheel upload.citeturn6search0turn7search4

---

## 4. Security posture

- Stay on interpreter builds that include PEP 706 (safe tar extraction) and pip 25.3+ to mitigate CVE‑2025‑8869.citeturn6search1turn8search0
- Rotate PyPI tokens every 90 days when trusted publishing is unavailable; scope tokens per project.
- Add `pip install --dry-run --report report.json` and `pip audit` to release pipelines to capture SBOM metadata and surface known vulnerabilities.citeturn6search2

---

## 5. Integrate with .NET / MAUI delivery

- MAUI hybrid templates (`dotnet new maui-blazor`) and some build tasks rely on Node and Python—pin Python 3.13 in GitHub Actions (`actions/setup-python@v5`) or Azure DevOps (`UsePythonVersion@0`) to keep parity with developer machines.citeturn9search0
- Back-end services used by MAUI apps should start from `python:3.13-bookworm` or `python:3.13-slim` Docker images so running workloads match the approved interpreter set.citeturn3search0

---

## 6. Operational runbook

| Task | Command | Cadence |
| --- | --- | --- |
| Verify interpreter & pip | `python3 --version && pip --version` | After patch |
| Upgrade pip/pipx | `python3 -m pip install --upgrade pip pipx` | Quarterly |
| Refresh CLI tools | `pipx upgrade --upgrade-all` | Monthly |
| Update uv | `uv self update` | Monthly |
| Audit dependencies | `pip install --dry-run --report report.json` then `pip audit` | Before release |

Add these steps to CI templates so hosted agents stay aligned with developer workstations.

---

## Troubleshooting quick wins

| Issue | Resolution |
| --- | --- |
| `python3` still points to the system interpreter | Run `pyenv global <version>` (POSIX) or `python.exe /Manage /SetDefault` (Windows Install Manager).citeturn4search0turn2search0 |
| `pip install` errors on Ubuntu | Ensure the `python3.13-venv` package is installed; Deadsnakes splits venv modules into a separate package.citeturn2search3 |
| CI installs wrong interpreter | Explicitly run `pyenv global`, `uv python install`, or configure `actions/setup-python` with the `python-version` matrix.citeturn5search0turn9search0 |
| `pipx` missing from PATH on Windows | Re-run `python.exe /Manage` to add the Scripts directory or configure `pipx ensurepath`.citeturn7search0 |

---

## Quick reference

```bash
# Windows: install latest Python via Install Manager
winget install 9NQ7512CXL7T

# macOS: switch to LTS
brew install python@3.13
brew link --overwrite python@3.13

# Ubuntu: add Deadsnakes repo and install
sudo add-apt-repository ppa:deadsnakes/ppa -y
sudo apt install python3.13 python3.13-venv -y

# Environment hygiene
python3 -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt

# CLI isolation
pipx install httpie
pipx list

# Modern project runner
uv venv && uv pip install -r requirements.txt
```

Use this playbook alongside the Git, Node, PowerShell, and Azure CLI guides to keep your tooling consistent across platforms.citeturn7search0turn5search0
