---
title: Space-Based Architecture
description: Scale transaction-heavy workloads with in-memory data grids and replicated processing units.
last_reviewed: 2025-10-31
owners:
  - @prodyum/architecture-guild
---

# Space-Based Architecture

Space-based architecture (SBA) distributes application state across an in-memory data grid and runs identical processing units in parallel, avoiding contention on a single database during load spikes. Azure Architecture Center recommends this style for high-volume, latency-sensitive workloads such as retail flash sales or financial trading.citeturn2search2turn0search2

---

## When it shines

- **Burst traffic:** Flash sales, gaming, IoT telemetry, or trading platforms where centralized databases become a bottleneck.citeturn2search2
- **Low-latency reads:** Keeping hot data in memory provides sub-millisecond access for pricing or matching engines.citeturn0search2
- **Elastic scaling:** Identical processing units scale out on Kubernetes, Azure Container Apps, or App Service without complex coordination.citeturn2search2

---

## Architecture building blocks

| Component | Role |
| --- | --- |
| **Processing unit (PU)** | Deployable unit with application logic plus embedded cache/grid node.citeturn2search2 |
| **In-memory data grid** | Distributed cache (Azure Cache for Redis Enterprise, Apache Ignite, NCache) that partitions or replicates data.citeturn0search2turn2search2 |
| **Messaging backbone** | Queues/topics (Event Hubs, Service Bus, Kafka) feeding work into processing units.citeturn2search2 |
| **Persistence layer** | Background jobs persist snapshots or audit trails to durable storage (Cosmos DB, SQL, Data Lake).citeturn2search2 |

```text
Event Streams -> Event Hubs -> Processing Units A/B/C -> Redis Cluster
                                      |
                                      v
                                Durable storage (Cosmos DB, SQL)
```

---

## Implementation guidelines

1. **Partition data** by tenant or shard key to minimise cross-node communication; replicate critical aggregates synchronously.citeturn0search2
2. **Warm new instances** by pre-loading cache segments or replaying recent events before putting nodes into rotation.citeturn2search2
3. **Use idempotent consumers** when ingesting events so retries do not create duplicate state.citeturn2search2
4. **Persist snapshots** on a schedule and support replay to rebuild in-memory state after failures.citeturn0search2

---

## Operational guardrails

- Monitor cache hit ratios, memory consumption, and queue depth to trigger autoscaling.citeturn2search2
- Apply chaos testing (node restarts, network partitions) to validate replication and quorum settings.
- Set eviction policies (LRU/LFU) plus alerting for memory pressure; offload cold data to durable stores.citeturn0search2
- Instrument processing units with OpenTelemetry so downstream systems can trace request lifecycles.

---

## Risks and mitigations

| Risk | Mitigation |
| --- | --- |
| **Memory limits** | Size grids appropriately, evict cold data, and persist snapshots frequently.citeturn0search2 |
| **Operational complexity** | Automate deployment, scaling, and monitoring of both processing units and cache clusters.citeturn2search2 |
| **Consistency lag** | Use synchronous replication for critical data, design UX to tolerate eventual consistency elsewhere.citeturn2search2 |

---

## Further reading

- Azure Architecture Center – Space-based architecture style.citeturn2search2turn0search2
