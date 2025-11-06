---
title: Domain-Driven Design (DDD)
description: Model complex domains with bounded contexts, ubiquitous language, and tactical patterns for aggregates.
last_reviewed: 2025-10-31
owners:
  - @prodyum/architecture-guild
---

# Domain-Driven Design (DDD)

Domain-Driven Design (DDD) combines strategic concepts—bounded contexts, context maps, ubiquitous language—with tactical building blocks such as entities, value objects, aggregates, and domain services so that complex business domains remain cohesive and maintainable.citeturn1search2turn1search1

---

## Strategic design

| Concept | Purpose |
| --- | --- |
| **Bounded context** | Defines where a domain model and ubiquitous language apply, allowing teams to evolve independently.citeturn1search2 |
| **Context map** | Documents relationships (partnership, customer/supplier, conformist, anti-corruption layer) between bounded contexts so integration contracts are explicit.citeturn1search2 |
| **Ubiquitous language** | Shared vocabulary that domain experts and developers use consistently in code, meetings, and documentation.citeturn1search2 |

### Establishing bounded contexts

1. Workshop subdomains (Ordering, Billing, Fulfillment) with domain experts, then draw bounded contexts around cohesive capabilities.citeturn1search2
2. Choose integration styles per context relationship—REST, messaging, published language, or anti-corruption layer—based on the context map.citeturn1search0turn1search2
3. Assign team ownership, repository structure, and deployment boundaries to those contexts to reduce cognitive load.citeturn1search2

---

## Tactical design

| Building block | Description |
| --- | --- |
| **Entity** | Mutable object tracked by identity across requests (e.g., `Order`).citeturn1search1 |
| **Value object** | Immutable concept defined purely by value (e.g., `Money`, `Address`).citeturn1search1 |
| **Aggregate** | Consistency boundary that enforces invariants and emits domain events to coordinate with other contexts.citeturn1search1 |
| **Domain service** | Stateless operation that coordinates multiple aggregates when logic does not fit inside one aggregate.citeturn1search1 |
| **Repository** | Abstraction for persisting aggregates while hiding infrastructure details.citeturn1search1 |

```csharp
public class Invoice : AggregateRoot
{
    private readonly List<InvoiceLine> _lines = [];

    public InvoiceId Id { get; }
    public Money Total => _lines.Sum(line => line.Amount);

    public void AddLine(Product product, Money amount)
    {
        var line = new InvoiceLine(product.Id, amount);
        _lines.Add(line);
        AddDomainEvent(new InvoiceLineAdded(Id, line));
    }
}
```

*Aggregates raise domain events so integration boundaries stay decoupled.*citeturn1search1

---

## Integrating contexts

- **Anti-corruption layer (ACL):** Translate external or legacy models into your ubiquitous language to protect core concepts from leaking abstractions.citeturn1search0
- **Event-driven integration:** Publish domain events (Azure Service Bus, Event Grid) to connect contexts asynchronously while preserving autonomy.citeturn1search2
- **Shared kernel:** Share only the minimal, truly common elements between contexts; broader sharing signals the need for a different context relationship.citeturn1search2

---

## Testing strategy

- Unit-test aggregates by asserting state changes or emitted domain events for a given command scenario.citeturn1search1
- Integration-test repositories against realistic infrastructure (SQL, Cosmos DB, Testcontainers) to validate mapping and transactional boundaries.
- Contract-test ACLs and published interfaces so upstream schema or API changes surface before corrupting your domain model.citeturn1search0

---

## Tooling and practices

- Use Roslyn analyzers or architecture tests to prevent accidental cross-context dependencies.citeturn1search1
- Capture context maps and ubiquitous language in ADRs or C4 diagrams so new engineers ramp quickly.citeturn1search2
- Automate code generation (e.g., source generators for value objects) to enforce immutability and validation rules.

---

## When to adopt DDD

- Business rules shift often and misunderstandings are costly.citeturn1search2
- Multiple teams collaborate on intersecting processes and need clear seams for ownership.citeturn1search2
- You plan to evolve toward microservices; DDD’s bounded contexts and aggregates provide natural seams for extraction.citeturn1search1turn1search2

---

## Further reading

- Azure Architecture Center – Strategic and Tactical Domain-Driven Design overview.citeturn1search2turn1search1


