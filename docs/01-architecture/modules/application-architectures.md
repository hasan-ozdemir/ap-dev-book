---
title: Application Architecture Spectrum
description: Compare monoliths, layered systems, clean/onion designs, and microservices for .NET and MAUI delivery.
last_reviewed: 2025-10-31
owners:
  - @prodyum/architecture-guild
---

# Application Architecture Spectrum

Architectural style determines how .NET and .NET MAUI teams deliver features, scale workloads, and manage operations. Azure Architecture Center groups the mainstream choices into monoliths, layered systems, clean/onion architectures, and microservices; this guide applies that lens to mobile-plus-backend solutions so you can pick the right starting point and evolve responsibly.citeturn3search7turn3search5

---

## Quick comparison

| Architecture | Core idea | Best fit | Operational impact | Typical .NET/MAUI workload |
| --- | --- | --- | --- | --- |
| **Modular monolith** | One deployable application with internal modules enforcing domain boundaries.citeturn2search2 | Product teams seeking fast iteration without distributed complexity. | Centralised deployment and observability; whole system scales together. | MAUI client + ASP.NET Core backend shipped as a single container or App Service site. |
| **Layered / N-tier** | Presentation, application, and data concerns separated into logical (optionally physical) tiers.citeturn3search6turn3search0 | Enterprise line-of-business apps with clear segregation between UI, services, and persistence. | Tier-specific scaling and compliance controls; latency added between hops. | MAUI or Blazor UI talking to REST/gRPC services backed by EF Core data tier. |
| **Clean / Onion (hexagonal)** | Domain core isolated from infrastructure through adapters and ports.citeturn4search0 | Medium-to-large solutions that need testable business rules and technology independence. | Strong boundaries and testability; extra ceremony implementing adapters. | MAUI shell (MVVM/MVU) invoking application services through mediator or CQRS pipelines. |
| **Microservices** | Autonomous services around bounded contexts with independent pipelines and deployment.citeturn0search1 | Organisations with multiple teams owning distinct domains and strict scalability needs. | Requires mature DevOps, distributed tracing, and contract governance. | MAUI client consumes an API gateway; services run on AKS or Azure Container Apps. |

---

## Modular monolith

A modular monolith keeps a single deployment unit but enforces boundaries with feature folders, domain packages, and explicit contracts, allowing teams to iterate quickly before committing to distributed architecture.citeturn2search2

**Use when**

- Front-end and back-end developers share a release cadence and need rapid feedback.
- Infrastructure can scale the entire workload horizontally or via simple container replicas.
- Domain boundaries are still forming and you want to avoid premature messaging complexity.

**Implementation checklist**

```text
src/
  Contoso.Mobile (MAUI client)
  Contoso.Api (ASP.NET Core minimal API)
  Contoso.Domain (entities, value objects, domain events)
  Contoso.Application (CQRS handlers, mediators)
  Contoso.Infrastructure (EF Core, integrations, message brokers)
```

- Enforce module rules with dependency analyzers (Roslyn, ArchUnitNET).
- Use `Partial` classes and internal access modifiers to keep contracts narrow.
- Apply feature flags internally before extracting independent services.

---

## Layered / N-tier architecture

Layered systems separate presentation, business logic, and data access into tiers that can scale or deploy independently, while N-tier implementations often impose matching physical tiers (web, API, data).citeturn3search6turn3search0

**Practices**

- Treat the MAUI client as the presentation tier; keep data access out of view-models.
- Define a contract-first API layer (REST or gRPC) to encapsulate validation and orchestration.
- Insert an application/service tier to aggregate data for mobile clients and reduce round trips.
- Apply caching (OutputCache, Redis) at service tier for read-heavy calls.

**Risks**

- Tight coupling (e.g., exposing EF Core entities over the wire) erodes tier boundaries.
- Additional hops increase latency—monitor with Application Insights dependencies and optimise DTO size.

---

## Clean / Onion / Hexagonal architecture

Clean architecture places the domain model at the centre, with concentric layers wrapping application and infrastructure. Dependencies always point inward, so business rules remain technology-agnostic while adapters translate inputs/outputs.citeturn4search0

**Key principles**

- **Domain layer** defines entities, value objects, and interfaces that abstractions must implement.
- **Application layer** orchestrates use cases via mediators or CQRS handlers, depending only on domain contracts.
- **Infrastructure layer** implements adapters (database, messaging, MAUI platform services) and depends on application/domain packages.
- **Presentation layer** (MAUI) converts user actions into application commands and maps results back to UI state.

```csharp
// Application layer contract
public interface ISyncContacts
{
    Task ExecuteAsync(CancellationToken token);
}

// MAUI view model depends on abstraction, not infrastructure
public class ContactsViewModel(ISyncContacts syncContacts, IToastService toast)
{
    public async Task SyncAsync()
    {
        await syncContacts.ExecuteAsync(CancellationToken.None);
        toast.Show("Contacts synced");
    }
}
```

---

## Microservices architecture

Microservices carve a solution into autonomous services aligned to bounded contexts, each owning its data, pipeline, and deployment lifecycle. Azure’s reference architecture emphasises domain-driven design, independent releases, and robust observability for this style.citeturn0search1turn3search5

**Adoption checklist**

- **Team topology:** One team per service (Dev + Ops) with clear on-call rotation.citeturn0search1
- **Domain maturity:** Bounded contexts are stable and justify independent schemas.
- **Operations:** Platform supports container orchestration (AKS/ACA), OpenTelemetry tracing, and contract versioning.citeturn0search1turn3search5
- **Client considerations:** Aggregate backend endpoints behind API Management or YARP gateway; expose resilient clients to MAUI (retry/circuit breaker, offline cache).

**Common pitfalls**

- Distributed transactions introduce eventual consistency—use saga/outbox patterns for critical flows.
- Chatty APIs drain mobile battery; coalesce calls at the gateway or expose query-optimised GraphQL endpoints.

---

## Selecting an architecture

1. **Team structure:** Start with modular monoliths when a single team owns the product; move to microservices only when teams and domains scale independently.citeturn2search2turn0search1
2. **Domain volatility:** If business rules still churn together, layered/clean architectures inside one deployable are safer; stable boundaries favour microservices.citeturn4search0turn0search1
3. **Operational readiness:** Evaluate deployment automation, observability, and compliance requirements before adding distributed complexity.citeturn3search5
4. **Client experience:** Regardless of backend style, mobile clients need caching, retries, and offline workflows to mask network variability.

Document the decision, revisit it after major milestones, and plan seam extraction (strangler fig) before splitting workloads.

---

## Further reading

- Modern Web Apps with ASP.NET Core (modular monolith).citeturn2search2
- Azure Architecture Center – Layered Architecture.citeturn3search6
- Azure Architecture Center – N-tier architecture style.citeturn3search0
- Azure Architecture Center – Clean architecture in ASP.NET Core.citeturn4search0
- Azure Architecture Center – Microservices architecture style.citeturn0search1turn3search5



