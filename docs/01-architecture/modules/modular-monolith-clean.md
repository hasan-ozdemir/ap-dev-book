---
title: Modular Monolith with Clean Architecture
description: Keep one deployable unit while enforcing clear module boundaries using Clean/Onion/Hexagonal patterns.
last_reviewed: 2025-10-31
owners:
  - @prodyum/architecture-guild
---

# Modular Monolith with Clean Architecture

A modular monolith keeps delivery simple by shipping one deployable artifact, yet borrows Clean/Onion/Hexagonal dependency rules to enforce clear module boundaries and maintain testability. Microsoft’s “modern web apps” reference implementation demonstrates how a single solution can stay modular before you split features into services.citeturn2search2turn4search0

---

## Why start with a modular monolith?

- **Team throughput:** Small-to-medium teams can iterate quickly without coordinating multiple pipelines or service discoverability.citeturn2search2
- **Domain cohesion:** Modules map to bounded contexts; each exposes application services or contracts that encapsulate internal models.citeturn4search0
- **Evolution path:** Once seams harden, you can extract a module into an independent microservice with minimal refactoring.citeturn2search2

---

## Clean architecture principles

| Rule | Description |
| --- | --- |
| **Dependency inversion** | Domain and application layers are inward-facing; infrastructure must depend on abstractions defined inside.citeturn4search0 |
| **Use cases (application layer)** | Orchestrate workflows via service or mediator layers that coordinate aggregates.citeturn4search0 |
| **Adapters** | Infrastructure (EF Core, HTTP clients, messaging) implements interfaces from the inner layers, keeping business logic technology-agnostic.citeturn4search0 |
| **Testing** | Domain and application layers are unit-test friendly; infrastructure adapters use integration tests.citeturn4search0 |

---

## Suggested solution layout

```text
src/
  Contoso.Mobile           (MAUI UI / presentation)
  Contoso.Application      (use cases, commands/queries, DTOs)
  Contoso.Domain           (entities, value objects, domain events)
  Contoso.Infrastructure   (EF Core, messaging, platform services)
  Contoso.Modules.*        (feature-specific facades if desired)
```

- `Contoso.Application` depends only on the domain; MediatR or CQRS handlers expose use cases.citeturn4search0
- Infrastructure projects reference the application/domain layers to implement interfaces (repositories, email gateways, push notifications).
- MAUI presentation consumes application abstractions via dependency injection; platform-specific code stays behind interfaces so tests can substitute fakes.citeturn4search0

---

## Enforcing module boundaries

- Define contracts per module (e.g., `Sales.Contracts`) and expose only DTOs/events that other modules require.citeturn2search2
- Use architecture tests (NetArchTest, ArchUnitNET) or Roslyn analyzers to disallow cross-module references.citeturn4search0
- Keep shared kernel packages minimal; anything broader should become a separate module or integration contract.citeturn4search0

---

## When to split modules into services

| Trigger | Action |
| --- | --- |
| Independent scaling or reliability needs emerge. | Extract module behind an API gateway or asynchronous event stream. |
| Release cadence diverges between modules. | Move module to its own repo/pipeline and expose published language. |
| Team ownership exceeds cognitive load. | Reorganize around context boundaries before service extraction. |

Before extraction, ensure you have automated tests around module seams and a playbook for routing traffic (strangler fig pattern).citeturn2search2turn4search0

---

## Further reading

- Microsoft – Modern Web Apps with ASP.NET Core (modular monolith reference).citeturn2search2
- Azure Architecture Center – Clean architecture in ASP.NET Core.citeturn4search0
