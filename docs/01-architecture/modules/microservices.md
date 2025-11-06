---
title: Microservices Architecture
description: Build independently deployable services around bounded contexts with resilient communication.
last_reviewed: 2025-10-31
owners:
  - @prodyum/architecture-guild
---

# Microservices Architecture

Microservices architecture structures an application as a suite of small, autonomous services aligned to bounded contexts. Each service owns its data, ships independently, and communicates through well-defined APIs or events. Azure Architecture Center highlights domain ownership, independent deployment pipelines, and full-lifecycle observability as prerequisites for this style.citeturn0search1turn3search5

---

## Core characteristics

| Characteristic | Why it matters |
| --- | --- |
| **Bounded contexts** | DDD seams keep models cohesive and prevent coupling between services.citeturn1search2turn0search1 |
| **Independent deployment** | Each service has its own CI/CD pipeline and can release without coordinating global downtime.citeturn0search1 |
| **Decentralised data** | Services persist data privately (SQL, NoSQL, Cosmos DB) and integrate via events instead of shared schemas.citeturn0search1 |
| **Observability + resilience** | Distributed tracing, structured logging, circuit breakers, and retries are required to diagnose and contain failures.citeturn3search5 |

---

## Reference building blocks

| Component | Responsibilities |
| --- | --- |
| **API gateway (YARP, Azure API Management)** | Aggregates endpoints, handles cross-cutting concerns, shields clients like MAUI apps from service topology changes.citeturn3search5 |
| **Service discovery / config** | Registers service endpoints (e.g., Azure App Configuration, Dapr sidecars).citeturn3search5 |
| **Messaging backbone** | Enables event-driven workflows via Azure Service Bus, Event Grid, or Kafka.citeturn3search0turn3search5 |
| **Service mesh (optional)** | Adds mTLS, traffic shaping, and observability for large estates (Istio, Linkerd).citeturn3search5 |
| **CI/CD pipelines** | Per-service pipelines automate build, test, security scanning, and deployment stages.citeturn0search1 |

---

## Adoption checklist

1. **Team topology:** Ensure squads can own services end-to-end, including on-call and incident response.citeturn0search1
2. **Operational maturity:** Put platform engineering in place (Kubernetes/Container Apps, logging/metrics stack, blue-green rollouts).citeturn3search5
3. **Automation:** Treat infrastructure as code, enforce automated testing, and bake quality gates into pipelines.citeturn0search1turn3search5
4. **Client strategy:** Provide MAUI or other front ends with typed clients, resilience policies, and offline caching because service boundaries are now remote.citeturn3search5

---

## Example topology

```text
MAUI Client -> API Gateway -> Order Service (HTTP)
                      |--> Inventory Service (HTTP)
Order Service --publishes--> Service Bus Topic --> Billing Service
```

- Gateway routes requests, handles auth, and aggregates results.
- Services exchange integration events instead of sharing databases.
- Observability pipeline exports logs, traces, and metrics to Azure Monitor.

---

## Patterns & practices

- **Saga / orchestration:** Coordinate long-running workflows with Durable Functions, MassTransit, or custom orchestrators to handle compensating actions.citeturn3search5
- **Strangler fig migration:** Incrementally peel capabilities from a legacy monolith, routing traffic through the gateway.citeturn0search1
- **Event sourcing + outbox:** Maintain audit trails and reliable message delivery when strong consistency is infeasible.citeturn3search5
- **Zero trust security:** Use JWT access tokens, mTLS between services, and centralised policy enforcement.citeturn3search5

---

## Common pitfalls and mitigations

| Pitfall | Mitigation |
| --- | --- |
| **Operational overhead** | Establish shared platform tooling, templates, and paved roads for deployments.citeturn3search5 |
| **Data consistency** | Prefer eventual consistency with outbox/event sourcing or synchronous orchestration only where unavoidable.citeturn3search5 |
| **Chatty interfaces** | Aggregate queries at the gateway, design coarse-grained APIs, or expose GraphQL endpoints to reduce round-trips.citeturn3search5 |
| **Team sprawl** | Keep services aligned to business capabilities; sunset unused services to reduce platform sprawl.citeturn0search1 |

---

## Further reading

- Azure Architecture Center – Microservices architecture style.citeturn0search1turn3search5
- Azure Architecture Center – Event-driven messaging reference architectures.citeturn3search0
