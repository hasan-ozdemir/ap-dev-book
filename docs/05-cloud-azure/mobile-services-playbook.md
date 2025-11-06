---
title: Azure Playbook for .NET MAUI Mobile Backends
description: Opinionated guidance for choosing and provisioning Azure services that power production-grade .NET MAUI applications, with Azure CLI deployments and .NET 9 integration snippets.
last_reviewed: 2025-11-03
owners:
  - @prodyum/cloud-guild
---

# Azure Playbook for .NET MAUI Mobile Backends

This playbook highlights the Azure services most frequently paired with Prodyum's .NET MAUI solutions. Each section explains when to use the service, shows repeatable **Azure CLI** deployment steps, and includes .NET 9 integration examples you can drop into mobile backends.

## Table of contents

1. [Identity & access](#1-identity-access)
2. [Serverless & container hosting](#2-serverless-container-hosting)
3. [Data & secret storage](#3-data-secret-storage)
4. [Messaging, realtime, and notifications](#4-messaging-realtime-and-notifications)
5. [Maps, location, and spatial intelligence](#5-maps-location-and-spatial-intelligence)
6. [Production readiness checklist](#6-production-readiness-checklist)

---

## 1. Identity & access

### 1.1 Microsoft Entra External ID (Azure AD B2C)

- **When to use:** adopt Microsoft Entra External ID for Customers when you need consumer or partner sign-in with social providers and adaptive MFA; it is the evolution of Azure AD B2C and keeps the same tenant model.citeturn22search2
- **Licensing watch-outs:** Premium P1 continues, while Premium P2 capabilities retire on 15 March 2026-confirm the SKU before onboarding new workloads.citeturn22search0turn22search1
- **Tenant provisioning:** Azure portal -> *Microsoft Entra ID* -> *Manage tenants* -> *Create* -> choose **Microsoft Entra ID (B2C)**, complete the basics, then link the subscription so delivery teams can deploy resources.citeturn21search3

**MAUI sign-in (WebAuthenticator)**

```csharp
var authResult = await WebAuthenticator.AuthenticateAsync(
    new Uri($"https://{tenant}.b2clogin.com/{tenant}.onmicrosoft.com/{policy}/oauth2/v2.0/authorize" +
            $"?client_id={clientId}&redirect_uri={redirectUri}&scope=openid%20offline_access"),
    new Uri(redirectUri));

SecureStorage.Default.Set("access_token", authResult?.AccessToken);
```

---

## 2. Serverless & container hosting

### 2.1 Azure App Service (managed Web API)

Use App Service when you need a managed .NET Web API with TLS, deployment slots, authentication, and autoscale without managing infrastructure. Premium v4 and AZ-aware plans add Availability Zone resilience.citeturn0search0turn1search1

```bash
az group create --name rg-maui-apis --location eastus
az deployment group create \
  --resource-group rg-maui-apis \
  --template-uri "https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/quickstarts/microsoft.web/app-service-docs-windows/azuredeploy.json" \
  --parameters webAppName=<unique-app-name> language=".NET" helloWorld=true
```
citeturn0search0

### 2.2 Azure Functions

Azure Functions is ideal for event-driven APIs, background processing, or scheduled jobs triggered by MAUI clients. Build locally with Functions Core Tools, then publish or run in Container Apps.citeturn2search0turn3search4

```bash
az group create --name rg-maui-functions --location eastus
func start                         # local development loop
dotnet publish src/Api --configuration Release
az functionapp function show \
  --name <functionAppName> \
  --resource-group rg-maui-functions \
  --function-name HttpExample \
  --query invokeUrlTemplate \
  --output tsv
```
citeturn2search0

### 2.3 Azure Container Apps

Select Container Apps when you need Dapr sidecars, gRPC, background workers, or to host your Functions in a fully managed container runtime with pay-per-second billing.citeturn23search0turn3search4

```bash
az group create --name rg-maui-aca --location eastus
az containerapp env create --name env-maui-aca --resource-group rg-maui-aca --location eastus
az containerapp create \
  --name maui-api \
  --resource-group rg-maui-aca \
  --environment env-maui-aca \
  --image ghcr.io/contoso/maui-api:1.0 \
  --ingress external --target-port 8080
az containerapp update \
  --name maui-api \
  --resource-group rg-maui-aca \
  --enable-dapr --dapr-app-id maui-api --dapr-app-port 8080
```
citeturn23search0

---

## 3. Data & secret storage

### 3.1 Azure Cosmos DB for NoSQL (serverless)

Cosmos DB serverless delivers microsecond latency for JSON documents with consumption-based billing-great for offline sync or per-user profiles.citeturn3search1turn3search2

```bash
account="maui-cosmos-$RANDOM"
az group create --name rg-maui-cosmos --location westus
az cosmosdb create \
  --resource-group rg-maui-cosmos \
  --name $account \
  --locations regionName=westus \
  --capabilities EnableServerless
```

```csharp
var cosmos = new CosmosClient(endpoint, key);
var container = cosmos.GetDatabase("maui").GetContainer("profiles");
await container.UpsertItemAsync(new { id = userId, displayName, lastSeen = DateTime.UtcNow });
```

### 3.2 Azure Storage (Blob)

Use Blob Storage for media uploads, diagnostics, and sync payloads. Issue SAS tokens from your backend so MAUI clients upload securely.citeturn9search0

```bash
az storage account create \
  --name <storageAccount> \
  --resource-group rg-maui-storage \
  --location eastus \
  --sku Standard_ZRS \
  --encryption-services blob
az storage container create \
  --name app-media \
  --account-name <storageAccount> \
  --auth-mode login
```

### 3.3 Azure Key Vault

Store API keys, Notification Hub secrets, and certificates in Key Vault; retrieve secrets from trusted backend services rather than embedding them in MAUI apps.citeturn11search0

```bash
az keyvault create --name <kvName> --resource-group rg-maui-secrets --location eastus
az keyvault secret set --vault-name <kvName> --name NotificationHubConnection --value "<connection-string>"
```

---

## 4. Messaging, realtime, and notifications

### 4.1 Azure Notification Hubs

Notification Hubs orchestrates APNs, FCM, and Windows notifications with tag-based targeting and platform-specific templates.citeturn1search0

```bash
az group create --name rg-maui-push --location eastus
az notification-hub namespace create --resource-group rg-maui-push --name mauins --location eastus --sku Free
az notification-hub create --resource-group rg-maui-push --namespace-name mauins --name mauihub --location eastus
```

### 4.2 Azure Service Bus

Use Service Bus queues for reliable command processing and topics for fan-out events across microservices.citeturn5search0

```bash
az servicebus namespace create --resource-group rg-maui-bus --name mauibusns --location eastus
az servicebus queue create --resource-group rg-maui-bus --namespace-name mauibusns --name app-commands
```

### 4.3 Azure SignalR Service

Add realtime updates-presence, chat, collaborative edits-without managing WebSocket scale-out.citeturn4search0

```bash
az signalr create --name mauisignalr --resource-group rg-maui-realtime --sku Standard_S1 --unit-count 1 --location eastus
```

```csharp
var connection = new HubConnectionBuilder()
    .WithUrl($"{signalrEndpoint}/clienthub", options => options.AccessTokenProvider = GetAccessTokenAsync)
    .WithAutomaticReconnect()
    .Build();
await connection.StartAsync();
```

### 4.4 Azure Communication Services

Add chat, voice, SMS, and email experiences without managing telephony infrastructure.citeturn6search0

```bash
az communication create \
  --name mauicomms \
  --resource-group rg-maui-comms \
  --location Global \
  --data-location UnitedStates
```

---

## 5. Maps, location, and spatial intelligence

Azure Maps delivers vector tiles, routing, and geofencing APIs that integrate with MAUI's `Map` control; Bing Maps Basic keys retire on 30 June 2025.citeturn7search0turn24search0

```bash
az group create --name rg-maui-maps --location eastus
az deployment group create \
  --resource-group rg-maui-maps \
  --template-uri "https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/quickstarts/microsoft.maps/azure-maps-create/azuredeploy.json" \
  --parameters accountName=<uniqueMapsName> sku=G2
az maps account keys list --name <uniqueMapsName> --resource-group rg-maui-maps
```

```csharp
var map = new Map
{
    MapServiceToken = azureMapsPrimaryKey,
    MapStyle = MapStyle.Road
};
```

---

## 6. Production readiness checklist

- **Governance:** enforce Azure Policy for resource tags, diagnostic logging, and backup/retention across resource groups.
- **Networking:** use VNet integration for App Service or Functions that require private data stores; restrict public ingress when possible.
- **Secrets rotation:** automate Key Vault secret rotation and renew Notification Hub or Service Bus keys via CLI (`az notification-hub authorization-rule list-keys`, `az servicebus namespace authorization-rule keys renew`).
- **Observability:** connect compute workloads to Application Insights, export metrics to Azure Monitor, and instrument MAUI clients with OpenTelemetry.
- **Cost controls:** default to serverless SKUs (Cosmos DB serverless, SignalR Standard with auto-scale) and configure budgets and alerts.
- **Incident response:** set Azure Monitor alerts for push failure rates, queue backlog depth, and API latency; document runbooks for common outages.

Use this playbook as the baseline architecture whenever you stand up a new MAUI backend, then extend it with workload-specific requirements.

