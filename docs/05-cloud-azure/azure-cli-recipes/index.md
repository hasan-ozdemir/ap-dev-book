---
title: Azure CLI Recipes
description: Copy-ready scripts for provisioning and managing Prodyum cloud resources.
last_reviewed: 2025-10-29
owners:
  - @prodyum/cloud-center
---

# Azure CLI Recipes

All scripts assume you have authenticated with `az login` and selected the target subscription via `az account set --subscription <SUB_ID>` before execution.citeturn3search0turn12search0 Replace the ALL_CAPS placeholders with real values prior to running in production.

## Resource group & tagging

```powershell
az group create --name APP-env-rg --location westeurope
az tag create --resource-id /subscriptions/<SUB_ID>/resourceGroups/APP-env-rg --tags Owner="jane.doe" Environment="Production" CostCenter="AP-001"
```

Create the resource group in the chosen region, then attach governance tags so ownership, environment, and cost metadata stay consistent.citeturn16search0turn16search1

## App Service with deployment slot

```powershell
az appservice plan create --name APP-prod-plan --resource-group APP-prod-rg --sku P1v4 --zone-redundant true
az webapp create --name APP-prod-appsvc-weu --resource-group APP-prod-rg --plan APP-prod-plan --runtime "DOTNETCORE:9.0"
az webapp deployment slot create --name APP-prod-appsvc-weu --resource-group APP-prod-rg --slot staging
az webapp config slot-auto-swap set --name APP-prod-appsvc-weu --resource-group APP-prod-rg --slot staging --slot swap-target production
```

The sequence provisions a Premium v4 plan, deploys the production site, creates a staging slot, and enables automatic swap after smoke tests.citeturn16search0turn10search0

## Azure Functions with Event Grid trigger

```powershell
az functionapp plan create --name APP-func-plan --resource-group APP-shared-rg --location westeurope --sku EP1 --is-linux
az storage account create --name appfuncstore --resource-group APP-shared-rg --location westeurope --sku Standard_LRS --kind StorageV2
az functionapp create --name APP-func-eg --resource-group APP-shared-rg --plan APP-func-plan --storage-account appfuncstore --functions-version 4 --runtime dotnet-isolated
```

Elastic Premium plans paired with StorageV2 accounts provide the scalable baseline required for Event Grid-triggered .NET isolated functions.citeturn17search0turn17search1

## Notification Hubs credentials

```powershell
az notification-hub credential apns update `
  --resource-group APP-push-rg `
  --namespace APP-push-ns `
  --name APP-push-hub `
  --apns-primary-key @apns_key.p8 `
  --app-id <APPLE_TEAM_ID> `
  --token-key-id <KEY_ID>
```

Use token-based APNs credentials (`.p8`, Key ID, Team ID, Bundle ID) for long-lived authentication with Notification Hubs.citeturn18search0

## Azure Monitor alert

```powershell
az monitor metrics alert create   --name APP-Availability   --resource-group APP-prod-rg   --scopes /subscriptions/<SUB_ID>/resourceGroups/APP-prod-rg/providers/Microsoft.Web/sites/APP-prod-appsvc-weu   --condition "avg Availability < 99 over 5m"   --action-groups "/subscriptions/<SUB_ID>/resourceGroups/APP-shared-rg/providers/microsoft.insights/actionGroups/ap-eng-oncall"
```

Create metric alerts to watch availability and route incidents to the appropriate action group (email, Teams, PagerDuty, etc.).citeturn19search0

Keep these scripts versioned in the repo so CI/CD workflows can reuse them consistently.
