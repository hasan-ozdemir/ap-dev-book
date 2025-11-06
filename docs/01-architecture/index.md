---
title: Architecture Playbooks
description: Compare, select, and implement proven software architectures for Prodyum solutions.
last_reviewed: 2025-10-29
owners:
  - @prodyum/architecture-guild
---

# Architecture Playbooks

Prodyum teams deliver MAUI clients, APIs, analytics platforms, and integration services. Choosing the right architecture is critical for scalability, maintainability, and velocity. This section summarizes the most widely adopted software architecture styles--layered systems, modular monoliths, microservices, event-driven, serverless, data-intensive platforms--and explains when and how to apply them in the .NET ecosystem.citeturn0search0turn0search5
Each playbook captures Prodyum battle-tested practices yet remains open for partners and community engineers who need pragmatic architecture guidance for .NET and Azure.

Use these guides to:

- Evaluate trade-offs before committing to an architecture.
- Map business concerns to technical strategies.
- Reference .NET-specific implementation tips, Azure services, and code snippets.

## Architecture catalog

### Foundational styles

- [Application Architecture Spectrum](./modules/application-architectures.md)
- [N-tier & Layered Architecture](./modules/n-tier-architecture.md)
- [Modular Monolith & Clean Architecture](./modules/modular-monolith-clean.md)
- [Domain-Driven Design (DDD)](./modules/domain-driven-design.md)

### Distributed & cloud-native

- [Microservices Architecture](./modules/microservices.md)
- [Service-Oriented Architecture (SOA)](./modules/service-oriented-architecture.md)
- [Event-Driven Architecture](./modules/event-driven.md)
- [CQRS & Event Sourcing](./modules/cqrs-event-sourcing.md)
- [Serverless Architecture](./modules/serverless.md)
- [Space-Based Architecture (SBA)](./modules/space-based-architecture.md)

### Extensibility & integration

- [Microkernel Architecture](./modules/microkernel.md)
- [Pipes and Filters Architecture](./modules/pipes-and-filters.md)

### Data & analytics

- [Big Data & Analytics Architecture](./modules/big-data-analytics.md)

> **Tip:** Each playbook links to Azure services, DevOps guidance, and tactical patterns (e.g., saga orchestration, outbox pattern) to help you implement and operate the architecture in production.

Need help deciding? Start with the [Architecture Spectrum](./modules/application-architectures.md) to understand core styles, then dive into the specific playbooks that match your product strategy.
