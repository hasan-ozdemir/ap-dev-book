---
title: Service-Oriented Architecture (SOA)
description: Organize coarse-grained services with contract-first design, shared governance, and mediation.
last_reviewed: 2025-10-31
owners:
  - @prodyum/architecture-guild
---

# Service-Oriented Architecture (SOA)

Service-oriented architecture (SOA) groups enterprise capabilities into coarse-grained services with technology-agnostic contracts, often mediated by shared infrastructure such as API gateways or enterprise service buses. Azure Architecture Center positions SOA as a fit for organisations that require centralised governance, canonical schemas, and interoperability with legacy systems.citeturn2search1turn6search1

---

## Core principles

| Principle | Practical guidance |
| --- | --- |
| **Contract-first design** | Define schemas (OpenAPI, gRPC, XML) before implementation, publish them in a catalog so consumers remain decoupled.citeturn2search1 |
| **Loose coupling** | Interact via messages and shared contracts instead of direct database access; use Azure API Management and Service Bus to mediate calls.citeturn2search1 |
| **Autonomy** | Services own logic and data but may share infrastructure during transition; plan to isolate data stores over time.citeturn2search1 |
| **Shared policy enforcement** | Centralize authentication, authorization, logging, and throttling through gateways or ESBs to satisfy governance requirements.citeturn2search1turn6search1 |

---

## Reference architecture

```text
Clients (MAUI, Web, Partners)
        |
        v
Azure API Management / Service Bus (mediation, security, transformation)
        |
 -------------------------------------------------
| Order Service | Inventory Service | Billing Service |
 -------------------------------------------------
        |
        v
Shared integration (Event Grid, Data Lake, Reporting)
```

- API Management handles routing, throttling, authentication, and schema validation.citeturn2search1
- Service Bus topics/queues enable asynchronous orchestration between services when workflows span domains.citeturn2search1
- Canonical contracts and message enrichment occur in the mediation layer before services process requests.citeturn6search1

---

## Implementation checklist

1. **Model business capabilities:** Map services to high-level domains (Ordering, Billing, Fulfilment) and publish their contracts.citeturn2search1
2. **Select mediation patterns:** Decide when to use orchestration (Durable Functions/Logic Apps) versus choreography (events on Service Bus/Event Grid).citeturn2search1turn6search1
3. **Enforce governance:** Automate policy checks (naming, versioning, SLA) in CI/CD pipelines and maintain a service catalog or developer portal.citeturn6search1
4. **Monitor centrally:** Emit structured logs and correlation IDs so operations teams can trace cross-service flows through Azure Monitor.citeturn2search1

---

## When to choose SOA

- Heavily regulated industries that demand centralised auditing and compliance.
- Enterprises integrating heterogeneous systems (ERP, CRM, legacy SOAP) that rely on canonical schemas.citeturn2search1turn6search1
- Organisations transitioning from monoliths that need governance before adopting fine-grained microservices.

---

## Trade-offs

| Advantages | Considerations |
| --- | --- |
| Consistent contracts and governance across the estate | Shared ESB/gateway can become a bottleneck if overloaded |
| Simplifies integration with legacy platforms | Coarse-grained services may require complex change coordination |
| Clear pathway toward microservices by carving out bounded contexts | Requires disciplined schema/version management to avoid drift |

---

## Further reading

- Azure Architecture Center – Service-oriented architecture style.citeturn2search1turn6search1
