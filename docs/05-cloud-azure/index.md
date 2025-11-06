---
title: Cloud Computing and Azure Services
description: Opinionated guidance for deploying and operating Prodyum workloads on Microsoft Azure.
last_reviewed: 2025-10-29
owners:
  - @prodyum/cloud-center
---

# Cloud Computing and Azure Services

Build resilient, observable, and cost-aware backends that power our cross-platform applications. This playbook curates Azure patterns and runbooks proven on delivery teams, with a focus on services we rely on every sprint.citeturn12search0turn11search3
While the guidance is born from Prodyum delivery engagements, it deliberately stays transparent so partner teams and the wider community can adapt the same Azure playbooks to their own environments.

## How to use this section

1. Start with **Azure Fundamentals** to align on governance, networking, and identity baselines.citeturn12search2
2. Use the service-specific guides (App Service, Azure Functions, Notification Hubs, App Configuration, Storage) to design and deploy production workloads.citeturn11search3turn11search4
3. Run the **Azure CLI recipes** for repeatable infrastructure tasks. Each recipe is copy-paste ready and designed to run in CI.citeturn11search6
4. Reference the **Operations** notes before go-live: zone redundancy, backup, alerts, and cost guardrails.citeturn11search3turn11search4

## Core modules

- [Azure Fundamentals](./azure-fundamentals.md): Tenant structure, landing zones, security baselines.
- [App Service Resilience](./app-services.md): Premium v4 guidance, zone redundancy, GitHub Actions integration.
- [Notification Hubs & Messaging](./notification-hubs.md): Push patterns, availability zone support, backend wiring.
- [Azure Mobile Services Playbook](./mobile-services-playbook.md): Identity, compute, data, messaging, and maps stacks for .NET MAUI.
- [Azure CLI Recipes](./azure-cli-recipes/index.md): Task-oriented scripts for common provisioning steps.

Future additions will cover Cosmos DB, Azure SQL, Application Insights, and AI services used by Prodyum delivery teams.citeturn12search5

> **Reminder:** All examples assume the `Prodyum Delivery` subscription, `westeurope` primary region, and workload-managed resource groups. Adjust naming conventions in line with the `cloud-standards.md` document (in progress).

