---
title: Linux, Unix & macOS Terminal Playbook
description: Modern shell, terminal, and tooling practices for cross-platform .NET MAUI teams working on Linux, Unix, and macOS.
last_reviewed: 2025-10-31
owners:
  - @prodyum/delivery-enablement
---

# Linux, Unix & macOS Terminal Playbook

Use this playbook to align Prodyum teams on current (October 2025) terminal tooling: upgraded shells, GPU-backed emulators, multiplexers that share sessions in the browser, and WSL integrations that bridge Windows and Linux workflows.citeturn2search0turn2search2

---

## 1. Shell foundations

| Shell | Why we standardise | Recommended actions |
| --- | --- | --- |
| **Bash 5.2+** | Ships on every major Unix; POSIX scripts stay portable. | Keep `/bin/bash` for automation, but use `bash -l` only for legacy login scripts. |
| **Zsh** | Default interactive shell on macOS; rich completion ecosystem. | Manage plugins via `antidote` or `zinit` to avoid Oh‑My‑Zsh startup lag. |
| **Fish 4.x** | February 2025 release rewrote the core in Rust, added modern keybinding notation, and improves completions.citeturn0search1turn0search2 | Install via Homebrew/apt; enable new bindings with `fish_config` or set `fish_features 'qmark-noglob' 'transient-prompt'`. |

**Path hygiene**

* Use `/etc/profile.d/authority.sh` (Linux) or `/etc/zshenv` (macOS) to prepend shared paths (toolchains, SDKs).
* Keep user overrides in `$HOME/.config/authority/path.d/*.sh` so personal experimentation never pollutes global configs.

---

## 2. Terminal emulators & AI-assisted workflows

| Tool | Highlights (2024‑2025) | Operational guidance |
| --- | --- | --- |
| **WezTerm 20240203** | Rust + GPU-accelerated renderer across Windows, macOS, Linux, BSD; built-in multiplexer panes, Lua-config scripting, image protocol support.citeturn3search1 | Ship a baseline `wezterm.lua` that sets fonts, color scheme, workspace keybinds. Upgrade with `brew upgrade wezterm` or `winget upgrade wez.wezterm`; note 20240203 adds blocking `wezterm -e` semantics—keep scripts aware.citeturn3search3 |
| **Warp 2.0** | Windows build (Feb 2025) delivers the same GPU-native terminal with optional login; Agent Mode brings AI-driven command plans, multi-agent management, and MCP integrations (Log 3).citeturn0search0turn0search4turn0search7 | Publish a team Warp profile that disables auto Agent Mode on production hosts; document how to toggle per environment. |

**AI guardrails**

* Require agent prompts to run in read-only mode first; only allow auto-execution in sandboxes.
* Encourage engineers to capture accepted agent plans in pull requests for auditability.

---

## 3. Multiplexers & shared sessions

| Multiplexer | Current release | Why it matters now | Team standard |
| --- | --- | --- | --- |
| **tmux 3.5** | Released 27 Sep 2025.citeturn0search8 | Keeps traditional scripts working while bundling latest patches; ideal for headless servers. | Maintain `/usr/local/share/tmux/authority.conf` with consistent prefix, mouse, and OSC 52 clipboard settings. |
| **Zellij 0.43** | Aug 2025: built-in web client, multiple pane selection, improved resurrection, browser sharing.citeturn4search6 | Browser-based sessions help onboarding and pair debugging; declarative layouts sync well in git. | Provide a default `config.kdl`; run `zellij setup --generate-config` per host then merge updates. |

**When to choose which**

* Use **tmux** for legacy automation or minimal dependencies.
* Use **Zellij** when collaborative features, declarative layouts, or Web sessions are required.

---

## 4. File exploration & previews

| Tool | Highlights | Operational notes |
| --- | --- | --- |
| **Yazi 25.x** | New package-manager commands, tmux-over-SSH image preview, platform-specific keybindings, Warp support.citeturn1search6 | Bundle `yazi.toml` defaults in dotfiles; ensure terminals support the needed image protocol (kitty/WezTerm inline). Use `ya pack -d` to prune unused plugins monthly.citeturn1search6 |

**Recommended workflow**

1. Launch Yazi from Zellij workspaces to reuse pane layouts.
2. Use `ya pack update` in CI to detect plugin compatibility issues.
3. For remote servers, keep Ueberzug++ installed as a fallback for non-graphics terminals (documented in the Yazi image preview guide).citeturn1search6

---

## 5. Cross-platform workflows with WSL

| Update | Why it matters | Actions |
| --- | --- | --- |
| WSL is now open-source (May 2025).citeturn2search2 | Community contributions accelerate kernel & tooling fixes; easier to pin to known-good builds. | Mirror WSL source fork for auditing; track upstream issues relevant to MAUI automation. |
| RHEL 8/9/10 images available for WSL (Sept 2025).citeturn2search0 | Developers can match production RHEL locally; simplifies Podman usage. | Provide `wsl-import` scripts plus activation key instructions; document Image Builder pipeline. |
| Ransomware groups abusing WSL to run Linux encryptors (Oct 2025).citeturn2search6 | Cross-platform malware risk increases; treat WSL as high-privileged environment. | Enforce endpoint policies: disable WSL where unneeded, monitor `wsl.exe` events, require signed images. |

**Best practices**

* Set `wsl --set-default-version 2` so Linux kernel updates flow via Microsoft Store packages.
* Use `wsl.conf` to cap memory/CPU; check in a hardened template with `processors=4`, `memory=8GB`, `swap=0`.
* Mirror RHEL repos via Satellite/Insights; ensure `subscription-manager` auto-registers on import.

---

## 6. Automation & dotfiles

* Maintain a central `dotfiles` repo with host-specific overlays (`macos`, `linux`, `wsl`).
* Ship a CLI (`authority-dotfiles bootstrap`) that:
  1. Detects OS and installs package manager prerequisites (Homebrew, apt + Deadsnakes, Winget).
  2. Installs terminal stack (WezTerm or Warp plus tmux/Zellij).
  3. Applies shell + terminal configs, preserving local overrides in `.local/`.
* Run weekly CI that spins up macOS runners, Ubuntu containers, and Windows WSL to validate bootstrap scripts end-to-end.

---

## 7. Upgrade cadence & monitoring

| Component | Check frequency | Automation |
| --- | --- | --- |
| WezTerm, tmux, Fish, Zellij | Monthly | Renovate/Dependabot watching GitHub releases; notify via Slack `#tooling-updates`. |
| Warp | Monthly | Subscribe to Warp release RSS; evaluate AI policy impacts before rollout. |
| Yazi | Biweekly | Monitor GitHub Releases for plugin-breaking changes. |
| WSL & RHEL images | Quarterly | Snapshot WSL Store package version; rebuild RHEL WSL images via Image Builder. |

Record upgrade decisions in the “Terminal Stack” section of the internal platform changelog.

---

## Appendix: quick commands

```bash
# Fish 4 on Ubuntu (PPA)
sudo add-apt-repository ppa:fish-shell/release-4 && sudo apt install fish

# WezTerm nightly test (Linux)
wezterm cli info

# Warp CLI disable auto agent
defaults write dev.warp.WarpAgent enableAutoAgentMode -bool false

# tmux apply shared config
tmux source-file /usr/local/share/tmux/authority.conf

# Zellij web client
zellij setup --generate-config && zellij web

# Yazi plug cleanup
ya pack -d unused-plugin

# RHEL 10 on WSL import
wsl --import RHEL10 .\WSL\RHEL10 rhel-10.0-x86_64-wsl2.tar.gz --version 2
```

Keep this playbook alongside the Git, .NET, PowerShell, Azure CLI, Node, Python, and package manager handbooks to deliver a consistent, secure terminal experience across the organisation.
