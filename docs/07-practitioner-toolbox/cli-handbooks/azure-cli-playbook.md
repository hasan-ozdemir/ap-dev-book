---
title: Azure CLI Playbook
description: Cross-platform install, configuration, and automation patterns for Azure CLI 2.64+ in .NET MAUI delivery pipelines.
last_reviewed: 2025-11-02
owners:
  - @prodyum/cloud-practice
---

# Azure CLI Playbook

Azure CLI (`az`) is the cross-platform control plane we use to automate Azure resources for .NET MAUI workloads because it runs consistently on Windows, macOS, Linux, and containers.citeturn7search0
The current Standard Term Support (STS) baseline is 2.66.x, so keep local workstations and build agents on 2.66 or newer to stay within Microsoft's supported window.citeturn1search6
Pair the CLI with Azure Developer CLI (`azd`) 1.12 or later when you need project-scaffolded infrastructure and pipeline automation that shares identities and configuration.citeturn9search7

---

## 1. Install or upgrade the CLI

| Platform | Command | Notes |
| --- | --- | --- |
| Windows | `winget install --id Microsoft.AzureCLI -e` | Installs the signed package and pins updates through Winget for managed workstations.citeturn9search10 |
| macOS | `brew update && brew install azure-cli` | Homebrew keeps the CLI updated; schedule `brew upgrade azure-cli` during patch windows.citeturn9search0 |
| Linux (apt) | `curl -sL https://aka.ms/InstallAzureCLIDeb \| sudo bash` | Microsoft's script adds the repository and installs `azure-cli` via APT for future upgrades.citeturn9search6 |
| Azure Linux container | `docker run -it mcr.microsoft.com/azure-cli:azurelinux3.0` | Provides a maintained Azure Linux 3.0 environment with the latest CLI for CI runs.citeturn4search7 |

```bash
az version --output table
az config get extension.use_dynamic_install
```

Ensure the version output lists 2.66.x (or newer) so you remain inside the current STS lifecycle.citeturn1search6

---

## 2. Authenticate securely

### 2.1 Developer login

```bash
az login --tenant <tenantId> --use-device-code
az account set --subscription <subscriptionId>
```

- `az login` opens a browser when one is available; add `--use-device-code` for headless shells, remote sessions, or Codespaces.citeturn7search0
- Confirm your context with `az account list --output table` and `az account set` so subsequent commands target the intended subscription.citeturn7search0

### 2.2 Federated identities for CI/CD

- Use `azure/login@v2` with OIDC (`id-token: write`) so GitHub Actions exchange workload tokens for Azure access without persisting secrets.citeturn10view0
- On self-hosted Azure runners, set `auth-type: IDENTITY` to reuse a system- or user-assigned managed identity instead of storing credentials.citeturn10view0
- Fall back to service principals only when required; scope `az ad sp create-for-rbac --sdk-auth` to a resource group and rotate secrets regularly.citeturn10view0

```bash
az ad sp create-for-rbac   --name ap-maui-deployer   --role Contributor   --scopes /subscriptions/<subscriptionId>/resourceGroups/<rgName>   --sdk-auth
```

---

## 3. Configure the CLI for predictable automation

| Setting | Command | Why it matters |
| --- | --- | --- |
| Default context | `az config set defaults.location=westeurope defaults.group=ap-maui` | Pins implicit location and resource group to avoid mis-targeted deployments.citeturn2search8 |
| Parameter persistence | `az config param-persist on` | Stores recent arguments per workspace to speed up iterative scripts.citeturn8search8 |
| Extension policy | `az config set extension.use_dynamic_install=no` | Forces pipelines to install extensions explicitly, preventing surprise downloads at runtime.citeturn2search8 |

```bash
az extension list-available --output table
az extension add --name resource-graph
az extension remove --name <extension>
```

Audit extensions quarterly and pin the specific modules (for example `containerapp`, `webapp`, `resource-graph`) that your automation depends on.citeturn11search1

---

## 4. Core operational workflows

### 4.1 Publish MAUI back-end APIs with managed identity

```bash
az group create --name ap-maui-api-rg --location westeurope
az appservice plan create --name ap-maui-plan --resource-group ap-maui-api-rg --sku P1v3 --is-linux
az webapp create --resource-group ap-maui-api-rg --plan ap-maui-plan --name ap-maui-api --runtime "DOTNET|9.0"
az webapp identity assign --resource-group ap-maui-api-rg --name ap-maui-api
```

Assign a system-managed identity before configuring Key Vault references or Service Connector bindings so secrets never sit in configuration files.citeturn2search1

### 4.2 Orchestrate Azure Container Apps revisions

```bash
az containerapp env create   --resource-group ap-maui-api-rg   --name ap-maui-env   --location westeurope

az containerapp create   --resource-group ap-maui-api-rg   --name ap-maui-catalyst   --environment ap-maui-env   --image ghcr.io/prodyum/maui-backend:2025-10-31   --target-port 8080   --ingress external   --min-replicas 1   --max-replicas 5   --revision-suffix blue

az containerapp revision set-mode   --resource-group ap-maui-api-rg   --name ap-maui-catalyst   --mode multiple

az containerapp revision label add   --resource-group ap-maui-api-rg   --name ap-maui-catalyst   --revision ap-maui-catalyst--blue   --label production
```

Use multiple revision mode, labels, and traffic routing to run blue/green swaps without downtime, then promote the validated revision to production.citeturn1search1turn1search2

### 4.3 Deploy infrastructure safely with Bicep

```bash
az deployment group what-if   --resource-group ap-maui-api-rg   --template-file infra/main.bicep   --parameters buildVersion=2025.10.31

az deployment group create   --resource-group ap-maui-api-rg   --template-file infra/main.bicep   --parameters buildVersion=2025.10.31   --confirm-with-what-if
```

Preview changes with `what-if` and promote only after reviewing the diff output in pull requests.citeturn12search0

---

## 5. Embed CLI usage in automation

- **GitHub Actions:** Combine `azure/login@v2` with workload identity federation and run scripts with the Azure CLI or Azure Developer CLI actions.citeturn10view0
- **Azure DevOps:** Use the `AzureCLI@2` task and connect it to a managed identity-backed service connection or workload identity federation to avoid static secrets.citeturn10view0
- **Azure Developer CLI (`azd`):** `azd auth login` reuses browser, device-code, or federated identities while `azd up` deploys infrastructure and applications together.citeturn9search7

---

## Troubleshooting quick wins

| Scenario | Recommended action |
| --- | --- |
| Conditional Access or headless shells block browser login | Use `az login --use-device-code` to complete MFA in a trusted browser window.citeturn7search0 |
| Scripts repeatedly prompt for resource group or location | Enable parameter persistence with `az config param-persist on` and clear entries when switching projects.citeturn8search8 |
| Pipelines fail because extensions are missing | Audit required extensions and install them explicitly during provisioning.citeturn11search1 |
| Unsure whether the installed CLI is supported | Run `az version` and ensure you are on 2.66.x STS or newer.citeturn1search6 |
| Need a reproducible CLI environment for debugging | Launch `mcr.microsoft.com/azure-cli:azurelinux3.0` locally or in CI for a clean Azure Linux shell.citeturn4search7 |

---

## Quick reference

```bash
# Install (Windows)
winget install --id Microsoft.AzureCLI -e

# Authenticate (device code)
az login --tenant <tenantId> --use-device-code

# Set workspace defaults
az config set defaults.group=ap-maui defaults.location=westeurope

# Preview and deploy Bicep
az deployment group create -g ap-maui-api-rg -f infra/main.bicep --confirm-with-what-if

# Manage Container Apps revisions
az containerapp revision label list --resource-group ap-maui-api-rg --name ap-maui-catalyst

# Audit extensions
az extension list --output table
```

Use this playbook alongside the Git, .NET, PowerShell, and other CLI guides in the portal when you refresh developer machines or build agents.citeturn7search0
