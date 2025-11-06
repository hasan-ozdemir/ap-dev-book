---
title: Notification Hubs & Messaging
description: Design resilient mobile push notification services using Azure Notification Hubs and related components.
last_reviewed: 2025-10-29
owners:
  - @prodyum/cloud-center
---

# Notification Hubs & Messaging

Azure Notification Hubs provides scalable cross-platform push messaging with built-in templates, telemetry, and high-availability options for modern mobile applications.citeturn7search0 Use this guide to align provisioning, security, and monitoring with Prodyum standards.

## 1. Service selection

- Choose the **Standard** tier for production so you can use templates, increased push quotas, telemetry, and Availability Zone support.citeturn7search2
- Deploy hubs in the region closest to your primary user base (West Europe by default) and keep dependent services in-region to minimise latency.citeturn7search0
- Enable **Availability Zones** for namespaces in supported regions to increase resiliency against datacentre failures.citeturn8search2

## 2. Provisioning steps

```powershell
az group create --name todoapp-push-rg --location westeurope
az notification-hub namespace create `
  --resource-group todoapp-push-rg `
  --name todoapp-push-ns `
  --sku Standard `
  --location westeurope `
  --enable-availability-zones true
az notification-hub create `
  --resource-group todoapp-push-rg `
  --namespace todoapp-push-ns `
  --name todoapp-push-hub `
  --location westeurope
```

> The `--enable-availability-zones true` switch must be set at namespace creation time; you cannot add zone redundancy after provisioning.citeturn7search1turn8search2

## 3. Platform credentials

- **Apple (APNs):** Use token-based HTTP/2 credentials (`.p8` key, Key ID, Team ID, Bundle ID) for long-lived authentication.citeturn0search4
- **Google (FCM):** Create a service account JSON in Google Cloud Console and upload it with `az notification-hub credential gcm update`.citeturn8search4
- **Windows (WNS):** Reserve your app in Partner Center, capture the Package SID and secret, and configure them under Windows settings.citeturn0search0turn0search5

## 4. Reference architecture

```
MAUI App ─► Azure API (App Service / Functions) ─► Notification Hubs ─► Platform notification services (APNs, FCM, WNS)
```

- The MAUI client requests push tokens on startup and sends them to the backend API.
- The backend stores tokens (for example Cosmos DB or Table storage) and tags them for segmentation.
- Event-driven workflows (Functions, Logic Apps) trigger hub notifications based on business events.citeturn7search0turn0search3

## 5. Reliability and monitoring

- Stream diagnostics and platform feedback into Log Analytics to investigate delivery failures and watch `IncomingMessages`, `SuccessfulSend`, and `NotificationOutcome` metrics.citeturn7search0turn8search5
- Configure alerts for send failure spikes or throttling events so incident response teams can act quickly.citeturn8search5
- Review Microsoft advisories (for example CVE-2025-59500) and apply mitigation guidance to protect privileged access.citeturn0search6

## 6. Security considerations

- Use managed identities or Azure AD service principals for APIs that interact with Notification Hubs, keeping access keys out of configuration files.citeturn3search0turn0search2
- Rotate access policies quarterly and separate send/listen/manage keys to enforce least privilege.citeturn7search0
- Ensure inbound requests to APIs that trigger pushes are authenticated and audited (Azure AD, OAuth 2.0, or custom JWT).citeturn3search0turn3search3

## 7. Client integration tips

- Use **MAUI Essentials** to request notification permissions gracefully and handle platform-specific prompts.citeturn8search4turn0search4
- On Android, handle token refresh events (FirebaseMessagingService) and resynchronise tokens with the backend.citeturn8search4
- On Windows, enable background tasks and associate the package SID/secret so WNS can deliver raw and toast notifications reliably.citeturn0search0turn0search5
