---
title: Azure Fundamentals
description: Establish governance, networking, identity, and cost baselines before provisioning workloads.
last_reviewed: 2025-10-29
owners:
  - @prodyum/cloud-center
---

# Azure Fundamentals

Prodyum takes a “landing zone first” approach so every workload inherits the governance, security, and platform guardrails defined in the Azure landing zone architecture and its quarterly policy refresh cadence.citeturn1search0turn4search10 Use this checklist before you provision or modernise application services.

## 1. Tenant and subscription structure

- **Root management group:** `ap-root` keeps global policy assignments (MFA, Defender for Cloud, platform guardrails) consolidated.citeturn1search0
- **Business unit groups:** `ap-delivery` (customer apps) and `ap-internal` (internal tooling) map to the standard landing-zone hierarchy so policies cascade reliably.citeturn1search0
- **Subscription vending:** Issue dedicated workload subscriptions when regulatory or cost-isolation requirements demand it; otherwise reuse the shared `Prodyum Delivery` subscription and migrate legacy tenants into the landing-zone tree.citeturn1search2turn1search8

> Apply the built-in **Require a tag** Azure Policy (or equivalent initiatives) at management group or subscription scope so every resource is born compliant with tagging standards.citeturn2search0

## 2. Resource naming and tagging

- **Naming pattern:** `{app}-{tier}-{resourceType}-{region}` (for example `todoapi-prod-appsvc-weu`) aligns with Cloud Adoption Framework naming guidance and keeps resources discoverable.citeturn0search2
- **Required tags:** Capture `Owner`, `Environment`, `CostCenter`, and `DataClassification` so governance, billing, and compliance reporting stay consistent.citeturn0search3
- **Policy-backed enforcement:** Use Azure Policy with Modify or Deny effects to add or require tags and remediate drift automatically.citeturn2search0

## 3. Identity and access management

- Prefer **Microsoft Entra** workload identities and registered service principals ahead of the March 31 2026 retirement of service-principal-less authentication.citeturn3search7
- Use **Privileged Identity Management (PIM)** for just-in-time elevation of platform roles across management groups and subscriptions.citeturn1search6
- Keep Conditional Access current (for example, move from “Require approved client app” to “Require app protection policy” before the 2026 retirement) and enforce MFA on privileged groups and automation identities.citeturn3search10

## 4. Networking blueprint

- Adopt hub-and-spoke or Virtual WAN topologies so shared services and landing-zone spokes remain isolated yet centrally governed.citeturn4search0turn4search1
- Force outbound traffic through Azure Firewall, private endpoints, and centralised DNS to maintain inspection and logging.citeturn4search8turn4search9
- For App Service or Functions, use **regional VNet integration** (and `WEBSITE_VNET_ROUTE_ALL` where needed) to reach private back-end services under NSG and UDR control.citeturn6search2turn6search6

## 5. Security baselines

- Enable Microsoft Defender for Cloud workload plans (App Service, SQL, Storage, AI) so the Microsoft Cloud Security Benchmark controls remain enforced and surfaced in posture reporting.citeturn5search5turn5search8
- Require HTTPS-only endpoints, modern TLS minimums, and managed certificates via platform policy assignments.citeturn5search5turn5search6
- Integrate Defender for Cloud recommendations with Sentinel or SIEM tooling to monitor and automate response workflows.citeturn5search8

## 6. Cost management

- Configure Cost Management budgets and action groups so product owners receive alerts as spend approaches thresholds.citeturn7search4turn7search0
- Review Azure Advisor and Cost Management insights monthly to right-size compute, networking, and reservation commitments.citeturn5search3turn7search2
- Tag long-lived environments (`Production`, `Staging`) separately from ephemeral ones (`Preview`, `QA`) to streamline automation and cost reporting.citeturn0search3

## 7. Observability contracts

- Stream application and platform telemetry through Azure Monitor (OpenTelemetry distro, Log Analytics, Microsoft Sentinel) for a unified operational view.citeturn5search5turn5search8
- Configure action groups (email, Teams, PagerDuty, automation) so alerts route to the right responders and runbooks.citeturn7search0turn7search2
- Define SLOs for availability and latency, then align alert thresholds, dashboards, and compliance reviews with those objectives.citeturn5search1turn5search6

Once these foundations are in place, continue with service-specific guides such as [App Service Resilience](./app-services.md) and [Notification Hubs & Messaging](./notification-hubs.md) to deepen Azure landing-zone adoption.citeturn1search0turn5search5
