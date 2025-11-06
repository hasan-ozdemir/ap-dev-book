---
title: Event-Driven Architecture
description: Use events to decouple producers and consumers for responsive, scalable solutions.
last_reviewed: 2025-10-31
owners:
  - @prodyum/architecture-guild
---

# Event-Driven Architecture

Event-driven architecture (EDA) integrates systems with asynchronous events so producers publish state changes and consumers react independently, improving scalability and resilience.citeturn0search5

---

## Core building blocks

| Component | What it does |
| --- | --- |
| **Event producers** | Emit domain facts (e.g., `OrderPlaced`, `DeviceTelemetry`). |
| **Event broker** | Routes, filters, and delivers events via platforms such as Azure Event Grid, Event Hubs, or Service Bus.citeturn0search5 |
| **Event consumers** | Process events, update state, or trigger downstream workflows without tight coupling.citeturn0search5 |
| **Event schema** | Defines payload contracts; adopt versioning and schema registries to keep consumers compatible.citeturn0search5 |

---

## Pattern variants

- **Simple event processing:** Stateless handlers (Azure Functions, Logic Apps) respond to each event.
- **Complex event processing:** Detect patterns across multiple streams using Azure Stream Analytics or Spark Structured Streaming.citeturn0search5
- **Event sourcing:** Persist the sequence of events as the source of truth; rebuild read models by replaying the log.citeturn1search0
- **CQRS with events:** Split commands and queries, propagating changes through event streams for read-model updates.citeturn1search0

---

## Example workflow

```text
Checkout Service --OrderPlaced--> Event Grid Topic
           |                                   |
           v                                   v
    Billing Function                 Inventory Service
           |
           v
   SignalR Notifications --> .NET MAUI client
```

- Order events flow through Event Grid so consumers scale independently; the hot path pushes real-time notifications to clients, while background services update inventory asynchronously.

---

## Design guidelines

- **Idempotency:** Make consumers tolerant of duplicate deliveries; use event version and deduplication keys.citeturn0search5
- **Schema evolution:** Publish backward-compatible changes or add new topics to avoid breaking existing subscribers.citeturn0search5
- **Dead-letter handling:** Configure Service Bus or Event Hubs dead-letter entities to capture failed messages for reprocessing.citeturn0search5
- **Observability:** Forward correlation IDs (`traceparent`) and metrics into Azure Monitor or Application Insights to debug event flows.citeturn0search5
- **Security:** Protect event ingress/egress with Azure AD, SAS tokens, VNet integration, and private endpoints.citeturn0search5

---

## .NET implementation tips

- Produce and consume events with SDKs such as `Azure.Messaging.EventGrid`, `Azure.Messaging.EventHubs`, or `Azure.Messaging.ServiceBus`.citeturn0search5
- Use background workers (`IHostedService`, .NET Worker Service template) for long-running consumers.
- Apply the outbox pattern so database updates and event publishing remain atomic (persist events, then relay them reliably).citeturn1search0

---

## Trade-offs

| Advantages | Considerations |
| --- | --- |
| Loose coupling, independent scaling | Eventual consistency and increased operational complexity |
| Reactive experiences for mobile/web clients | Requires robust monitoring, tracing, and replay tooling |
| Natural fit for microservices and IoT workloads | Governance needed to manage event schema and topic sprawl |

---

## Further reading

- Azure Architecture Center – Event-driven architecture style.citeturn0search5
- Azure Architecture Center – Event sourcing and CQRS patterns.citeturn1search0
