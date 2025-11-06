---
title: CQRS & Event Sourcing
description: Separate reads and writes, capture state changes as events, and build responsive systems.
last_reviewed: 2025-10-29
owners:
  - @prodyum/architecture-guild
---

# CQRS & Event Sourcing

Command Query Responsibility Segregation (CQRS) splits read and write workloads: commands mutate state, while queries return read-optimized projections. Event sourcing complements CQRS by persisting state changes as an append-only stream of events.citeturn1search0turn0search5

## CQRS basics

- **Write model:** Handles commands (`CreateOrder`, `ApproveLoan`); enforces business rules.
- **Read model:** Consumes events to build denormalized projections (SQL read model, search index).
- **Transport:** Commands often use synchronous APIs; events delivered via messaging (Service Bus, Event Grid).

## Event sourcing principles

- **Events as source of truth:** Persist each state change (`OrderCreated`, `OrderLineAdded`).
- **Rehydration:** Rebuild aggregates by replaying events; snapshots optimize performance.
- **Audit & replay:** Built-in history enables auditing and debugging.

## .NET implementation sketch

```csharp
public interface IEventStore
{
    Task AppendAsync(string streamName, long expectedVersion, IEnumerable<IDomainEvent> events);
    Task<IReadOnlyList<IDomainEvent>> ReadAsync(string streamName);
}

public async Task Handle(CreateInvoice command)
{
    var invoice = Invoice.Create(command.InvoiceId, command.CustomerId);
    await _eventStore.AppendAsync($"invoice-{command.InvoiceId}", StreamVersion.None, invoice.DomainEvents);
    await _bus.PublishAsync(invoice.DomainEvents);
}
```

- Use Azure Cosmos DB or EventStoreDB for append-only storage.
- Implement optimistic concurrency (expected version).
- Publish events to messaging infrastructure for read model updates.

## Read model projection example

```csharp
public class InvoiceReadModelHandler : IEventHandler<InvoicePaid>
{
    public async Task HandleAsync(InvoicePaid evt, CancellationToken token)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE InvoiceSummary SET Status = @Status WHERE InvoiceId = @InvoiceId",
            new { Status = "Paid", InvoiceId = evt.InvoiceId });
    }
}
```

## When to adopt CQRS + ES

- Domain has complex workflows with frequent changes to read requirements.
- Auditability and traceability are critical.
- Reads significantly outnumber writes, or you need specialized read stores (Elastic, Cosmos DB).

Avoid this pattern when CRUD suffices; CQRS/ES adds operational complexity and requires robust DevOps practices (monitoring, debugging tools).citeturn1search0

## Best practices

- **Event schema versioning:** Add event metadata (version, correlation IDs) to manage evolution.
- **Replay tooling:** Build scripts to rebuild projections; store migration events when schemas change.
- **Snapshotting:** Periodically persist aggregate snapshots to avoid long replay times.
- **Testing:** Write unit tests for aggregates and integration tests for projections.
- **Security:** Protect event streams; encrypt sensitive payloads before writing.

## Further reading

- CQRS and Event Sourcing architecture patternsciteturn1search0
- Event-driven architecture styleciteturn0search5
