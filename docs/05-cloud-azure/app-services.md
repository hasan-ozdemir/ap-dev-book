---
title: App Service Resilience & Deployment
description: Build, secure, and operate Azure App Service workloads with Prodyum best practices.
last_reviewed: 2025-10-29
owners:
  - @prodyum/cloud-center
---

# App Service Resilience & Deployment

Azure App Service is our standard platform for HTTP workloads that back MAUI front ends because Microsoft operates the infrastructure, applies platform patches, and keeps compliance guardrails current while we focus on code.citeturn1search0 This playbook captures Prodyum guidance to run those apps resiliently in 2025.

## 1. Choose the right SKU

| SKU | When to use | Notes |
| --- | --- | --- |
| Premium v3 (P1v3 / P2v3) | Shared staging, performance testing | Faster processors, SSD storage, and higher memory-to-core ratios deliver production-like behaviour for non-production workloads without overspending.citeturn11search0 |
| Premium v4 | Primary production workloads | Uses new Dadsv6/Eadsv6 hardware, NVMe temporary storage, and extended SKU options for Windows and Linux apps, plus built-in availability-zone support.citeturn2search1turn2search4 |
| App Service Environment v3 | Regulated or network-isolated deployments | Single-tenant App Service footprint inside your virtual network with up to 200 instances for workloads that require isolation.citeturn7search1turn11search3 |

**Upgrade path:** Premium v2/v3 and ASEv3 plans upgraded to Premium v4 can now enable Availability Zones with as few as two instances while retaining the 99.99% SLA.citeturn2search5turn11search2

## 2. Deploy with GitHub Actions

Prodyum standardises on GitHub Actions for App Service deployments. Use the workflow below as a baseline.citeturn3search0

```yaml
name: api-deploy

on:
  push:
    branches: [ main ]
  workflow_dispatch:

env:
  AZURE_WEBAPP_NAME: todoapi-prod-appsvc-weu
  AZURE_RESOURCE_GROUP: todoapi-prod-rg
  DOTNET_VERSION: '9.0.x'

jobs:
  build-and-deploy:
    runs-on: ubuntu-22.04

    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
      - run: dotnet restore
      - run: dotnet publish src/TodoApi/TodoApi.csproj -c Release -o publish
      - name: OpenID Connect login
        uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZURE_CLIENT_ID }}
          tenant-id: ${{ secrets.AZURE_TENANT_ID }}
          subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
      - uses: azure/webapps-deploy@v3
        with:
          app-name: ${{ env.AZURE_WEBAPP_NAME }}
          package: publish
```

- Prefer OpenID Connect for `azure/login@v2`; configure federated credentials instead of storing publish profiles or long-lived secrets.citeturn4search0
- If you must use service principals, keep the scope narrow (for example via `az ad sp create-for-rbac`) and store credentials as GitHub secrets.citeturn3search0
- Use deployment slots for blue/green releases, warm up the staging slot, and swap only after validation.citeturn10search0

## 3. Harden the platform

- Enable Availability Zones when the plan supports it so two instances survive a zone failure while maintaining the 99.99% SLA.citeturn2search5turn11search2
- Assign a system- or user-assigned managed identity to every app and use it for downstream access instead of storing secrets.citeturn3search0
- Enforce **HTTPS Only**, modern TLS, and FTPS policies via Azure Policy or landing-zone guardrails to block configuration drift.citeturn12search1turn12search5
- Front internet-facing apps with Azure Front Door or Application Gateway when you need global routing, WAF protection, or caching.citeturn1search1

## 4. Operational excellence

- **Health probes:** Expose `/healthz` (or similar) and enable App Service Health Check to remove or recycle unhealthy instances automatically.citeturn6search0
- **Autoscale rules:** Use Azure Monitor autoscale (or the automatic scaling feature) to add capacity on demand and respect per-app limits.citeturn8search1turn8search5
- **Backups:** Configure automatic or custom backups, route them through virtual networks when required, and test restores quarterly.citeturn9search0
- **Monitoring:** Enable Application Insights and App Service diagnostics to capture runtime telemetry, performance counters, and troubleshooting snapshots.citeturn6search4

## 5. Troubleshooting playbook

| Symptom | Checks | Resolution |
| --- | --- | --- |
| Slow cold starts | Inspect `WEBSITE_DYNAMIC_CACHE`, review warmup path configuration | Pre-warm via deployment slots; enable Always On |
| 500 errors during deployment | Review slot swap log and compare app settings | Use slot-setting flags, enable `autoSwapSlotName` |
| TLS handshake failures | Confirm custom domain binding and certificate expiry | Regenerate managed certificate or re-upload reissued certciteturn5search0 |

## 6. Azure CLI deployment recipe

```powershell
az group create --name todoapi-prod-rg --location westeurope
az appservice plan create `
  --name todoapi-prod-plan `
  --resource-group todoapi-prod-rg `
  --sku P1v4 `
  --zone-redundant true
az webapp create `
  --name todoapi-prod-appsvc-weu `
  --resource-group todoapi-prod-rg `
  --plan todoapi-prod-plan `
  --runtime "DOTNETCORE:9.0"
az webapp config appsettings set `
  --name todoapi-prod-appsvc-weu `
  --resource-group todoapi-prod-rg `
  --settings "ASPNETCORE_ENVIRONMENT=Production"
```

For Linux containers, replace the runtime parameter with `--deployment-container-image-name <acr>.azurecr.io/todoapi:latest`, and scale the plan (`az appservice plan update --sku`) before provisioning when moving between Premium tiers.citeturn3search0
