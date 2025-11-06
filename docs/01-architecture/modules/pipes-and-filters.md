---
title: Pipes and Filters Pattern
description: Compose data processing pipelines from reusable filters connected by streaming pipes.
last_reviewed: 2025-10-31
owners:
  - @prodyum/architecture-guild
---

# Pipes and Filters Pattern

The pipes and filters pattern streams data through a sequence of independent filters, each performing a transformation before passing results to the next stage via pipes. Azure Architecture Center recommends this style for ETL, media processing, and DevOps workflows where reuse and composition are key.citeturn2search0

---

## Building blocks

| Element | Responsibility |
| --- | --- |
| **Filter** | Stateless component that reads input, transforms it, and emits output. |
| **Pipe** | Transport channel (queues, streams, channels) that connects filters and provides buffering/back-pressure. |
| **Scheduler/orchestrator** | Coordinates execution cadence—Azure Functions, Logic Apps, or worker services can host filters.citeturn2search0 |
| **Error handling** | Captures failed messages and routes them for retries or manual remediation.citeturn2search0 |

---

## Example .NET pipeline

```csharp
public interface IFilter<TIn, TOut>
{
    ValueTask<TOut> ExecuteAsync(TIn input, CancellationToken token = default);
}

public sealed class Pipeline<T>
{
    private readonly IReadOnlyList<IFilter<T, T>> _filters;

    public Pipeline(IEnumerable<IFilter<T, T>> filters) => _filters = filters.ToList();

    public async ValueTask<T> ExecuteAsync(T payload, CancellationToken token = default)
    {
        foreach (var filter in _filters)
        {
            payload = await filter.ExecuteAsync(payload, token);
        }
        return payload;
    }
}
```

- Host filters in worker services or Azure Functions.
- Use `System.Threading.Channels` or Azure Service Bus to implement pipes that buffer and smooth bursts.

---

## Use cases

- Data ingestion: raw telemetry → validation → enrichment → storage.citeturn2search0
- Media workflows: decode → transcode → watermark → package.
- DevOps automation: checkout → compile → test → publish containers.
- Real-time analytics: parse event → enrich → aggregate → emit dashboard metrics.

---

## Benefits and considerations

| Pros | Cons |
| --- | --- |
| High reusability and composability of filters | Pipes can proliferate; need governance for topology |
| Filters can scale independently to meet demand | Must handle back-pressure and monitoring at each stage |
| Clear separation of concerns simplifies testing | Additional latency if filters are chained synchronously |

---

## Best practices

- Keep filters stateless and idempotent; persist state outside the pipeline when necessary.citeturn2search0
- Adopt schema validation (JSON Schema, Avro) at pipeline boundaries to catch malformed payloads early.
- Implement back-pressure via bounded queues, throttling, or rate-based autoscaling.
- Emit metrics per filter (processing time, throughput, failures) and monitor with Azure Monitor or Application Insights.

---

## Further reading

- Azure Architecture Center – Pipes and Filters pattern.citeturn2search0
