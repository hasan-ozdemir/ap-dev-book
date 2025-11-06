---
title: .NET CLI Playbook
description: Install, manage, and automate the complete .NET 9 command-line lifecycle for cross-platform MAUI and cloud workloads.
last_reviewed: 2025-10-31
owners:
  - @prodyum/delivery-enablement
---

# .NET CLI Playbook

Prodyum squads rely on the `dotnet` CLI to bootstrap projects, pin SDK feature bands, manage MAUI workloads, and ship artefacts through automated pipelines. This playbook reflects the .NET 9 toolchain as of October 2025 so teams can configure workstations quickly, keep CI/CD in sync, and satisfy mobile store policies.citeturn0search1turn2search1

---

## 1. Install & verify the CLI

| Platform | Recommended install path | Keep it current |
| --- | --- | --- |
| Windows | `winget install --id Microsoft.DotNet.SDK.9 --source winget` | `winget upgrade Microsoft.DotNet.SDK.9` keeps the SDK patched without manual downloads.citeturn1search0 |
| macOS | `brew install --cask dotnet-sdk` (or run Microsoft’s notarised installer) | `brew upgrade dotnet-sdk` or rerun the official installer when new feature bands ship.citeturn1search0turn2search0 |
| Linux (Ubuntu) | `sudo apt-get update && sudo apt-get install dotnet-sdk-9.0` | Manage updates through Microsoft’s apt repository (`sudo apt-get upgrade dotnet-sdk-9.0`).citeturn1search0 |
| Air-gapped / build agents | `dotnet-install.ps1` / `dotnet-install.sh` | Automate with `--version <band>` or `--version latest` for deterministic builds in disconnected environments.citeturn2search2 |

After installation run:

```bash
dotnet --info
dotnet --list-sdks
dotnet --list-runtimes
```

Target the current patched feature band (e.g., `9.0.306`, released 14 Oct 2025) so local machines, containers, and hosted agents stay aligned.citeturn2search1

---

## 2. Manage SDKs, runtimes, and workloads

### 2.1 Pin feature bands with `global.json`

```json
{
  "sdk": {
    "version": "9.0.306",
    "rollForward": "minor",
    "workloadVersion": "9.0.306"
  }
}
```

- `version` locks the SDK feature band for deterministic restores.
- `rollForward: "minor"` allows security patches while blocking major jumps.
- `workloadVersion` (introduced with .NET 9 workload sets) keeps MAUI, Android, iOS, Aspire, and other workloads in step with the SDK.citeturn2search1turn3search0

### 2.2 Workload sets & history

Workload sets prevent surprise manifest upgrades and preserve change history:

```bash
dotnet workload config --update-mode workload-set
dotnet workload install maui --version 9.0.306
dotnet workload history
```

- `config` toggles between workload-set and legacy manifest behaviour.citeturn3search0
- `install --version` pins the workload-set release that shipped with your SDK band.citeturn3search0
- `history` (new in .NET 9) lists every workload operation so you can roll back or audit changes.citeturn3search0turn2search1

> **Tip:** If a build agent drifts, run `dotnet workload update --from-history <id>` before reinstalling the pinned set declared in `global.json`.citeturn2search1turn3search0

---

## 3. Project lifecycle commands

| Phase | Command | Why it matters in .NET 9 |
| --- | --- | --- |
| Scaffold | `dotnet new maui`, `dotnet new webapi` | Templates generate cross-platform MAUI apps and cloud-ready APIs with current defaults.citeturn2search1 |
| Restore | `dotnet restore` | Resolves NuGet packages and workloads referenced in `global.json`; runs implicitly when required.citeturn2search1turn3search1 |
| Build | `dotnet build --tl:auto` | Terminal Logger is on by default, producing concise, clickable logs; disable with `--tl:off` if legacy parsers require it.citeturn4search0 |
| Watch | `dotnet watch maui` | Iterates quickly across Android, iOS, Mac Catalyst, and Windows targets using hot reload.citeturn2search1 |
| Run | `dotnet run -f net9.0-ios` | Executes the platform target configured in multi-target MAUI projects.citeturn2search1 |
| Test | `dotnet test --filter Category=Integration` | Test output streams through Terminal Logger for faster feedback in CI.citeturn2search1turn4search0 |
| Format | `dotnet format --verify-no-changes` | Enforce `.editorconfig` settings before opening a pull request.citeturn0search3 |

---

## 4. Publish, trim, and optimise

### 4.1 Release builds

```bash
dotnet publish -c Release -r android-arm64 --self-contained true --output artifacts/android
dotnet publish -c Release -r ios-arm64 --self-contained true --output artifacts/ios
dotnet publish -c Release -r win10-x64 --self-contained true --output artifacts/win
```

- `dotnet publish` targets `Release` by default for `net8.0+`; override with `-c Debug` only when collecting diagnostics.citeturn0search2
- Use `--artifacts-path` to centralise outputs (e.g., `dotnet publish --artifacts-path artifacts`).citeturn0search2
- Layer `-p:PublishTrimmed=true` or `-p:PublishReadyToRun=true` to reduce binary size or improve startup time.citeturn0search2

### 4.2 Multi-platform pipelines

```yaml
strategy:
  matrix:
    runtime: [android-arm64, ios-arm64, linux-x64]
steps:
  - uses: actions/setup-dotnet@v4
    with:
      dotnet-version: 9.0.306
  - run: dotnet workload restore
  - run: dotnet publish -c Release -r ${{ matrix.runtime }} --self-contained true --output artifacts/${{ matrix.runtime }}
```

Pair with signing tasks (`jarsigner`, `codesign`, `xcodebuild`) before distributing to app stores or enterprise catalogues.citeturn3search1

---

## 5. Tooling & automation

- **.NET tools** – `dotnet tool update --global dotnet-ef` keeps global tools current; add `--allow-roll-forward` when authors have not published new binaries yet.citeturn2search3turn2search1
- **MAUI asset workflows** – Combine `dotnet workload history` with `dotnet workload update --version <band>` to revert accidental workload upgrades on developer machines or agents.citeturn3search0turn2search1
- **Aspire orchestration** – Install `dotnet new install Aspire.ProjectTemplates` to scaffold Aspire-hosted microservices that complement MAUI clients.citeturn3search1

---

## 6. Diagnostics & troubleshooting

| Symptom | Resolution |
| --- | --- |
| CLI output is unreadable in legacy log processors | Add `--tl:off` or set `MSBUILDTERMINALLOGGER=off` to revert to the classic console logger.citeturn4search0 |
| Workload mismatch after SDK upgrade | Run `dotnet workload list`, then `dotnet workload update --version <band>` to apply the pinned workload set.citeturn3search0turn2search1 |
| MAUI workloads missing on fresh install | Execute `dotnet workload install maui` (elevated if needed) inside the SDK band declared in `global.json`.citeturn3search0turn0search1 |

---

## 7. Quick reference

```bash
# Environment
dotnet --info
dotnet workload history

# Workstation setup
dotnet workload config --update-mode workload-set
dotnet workload install maui --version 9.0.306
dotnet new install Aspire.ProjectTemplates

# Lifecycle
dotnet new maui -n Contoso.Mobile
dotnet restore
dotnet build -c Release
dotnet test --filter Category=Smoke
dotnet publish -c Release -r android-arm64 --self-contained true
```

Use these commands as building blocks in GitHub Actions, Azure DevOps, or local scripts so your teams share consistent, policy-aligned .NET workflows.citeturn3search1turn2search1
