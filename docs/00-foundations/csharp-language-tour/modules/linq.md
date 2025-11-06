---
title: LINQ Fundamentals
description: Query in-memory and remote data with declarative LINQ syntax and fluent operators in .NET 9.
last_reviewed: 2025-11-03
owners:
  - @prodyum/language-guild
---

# LINQ Fundamentals

Language Integrated Query (LINQ) brings query expressions directly into C#, letting you filter, project, and aggregate data in strongly typed code without string-based DSLs.citeturn0search1 The compiler rewrites query comprehension syntax (for example, `from`, `where`, `select`) into calls to the standard query operators in `System.Linq`, so the method and query forms are interchangeable.citeturn0search1turn0search0

## Keywords & operators covered

`from`, `where`, `select`, `group`, `orderby`, `join`, `let`, `into`, `selectMany`, `Distinct`, `Any`, `All`, `FirstOrDefault`, `Aggregate`

## Quick start

```csharp
// File: samples/ConsoleSnippets/LinqShowcase.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleSnippets;

public static class LinqShowcase
{
    private static readonly IReadOnlyList<Order> Orders =
    [
        new(1001, "SEA", 129.95m, new [] { "MAUI T-Shirt", "Sticker Pack" }),
        new(1002, "LAX", 349.50m, new [] { "Bluetooth Speaker" }),
        new(1003, "SEA", 89.00m, new [] { "Wireless Charger", "USB-C Cable" })
    ];

    public static IEnumerable<(string Destination, decimal Revenue)> TopDestinations(decimal minimum)
        => from order in Orders
           where order.Total >= minimum
           group order by order.Destination into bucket
           orderby bucket.Key
           select (bucket.Key, bucket.Sum(o => o.Total));

    public static IEnumerable<string> LineItemsContaining(string token)
        => Orders.SelectMany(o => o.LineItems)
                 .Where(item => item.Contains(token, StringComparison.OrdinalIgnoreCase))
                 .Distinct();

    private sealed record Order(int Id, string Destination, decimal Total, string[] LineItems);
}
```

- Query syntax is translated into chained standard query operators at compile time.citeturn0search1turn0search0
- Deferred execution means the `IEnumerable<T>` returned by `TopDestinations` runs only when enumerated, enabling streaming of large datasets.citeturn0search0
- Method syntax (`SelectMany`, `Distinct`) works seamlessly alongside comprehension clauses when no direct keyword exists.citeturn0search0

> **Try it:** Call `TopDestinations(100m)` from `Program.cs`, iterate the results, and then append `.Take(1)` to see how deferred execution affects filtering after grouping.

## Operator categories

| Category | Purpose | Examples |
| --- | --- | --- |
| Filtering | Restrict sequences to qualifying elements. | `Where`, `OfType`, query `where` clause.citeturn0search0 |
| Projection | Transform elements into new shapes (anonymous types, DTOs). | `Select`, `SelectMany`, query `select`.citeturn0search0 |
| Sorting | Define ordering with optional secondary keys. | `OrderBy`, `ThenBy`, `orderby`.citeturn0search0 |
| Grouping | Bucket results by key for aggregation. | `GroupBy`, `group ... by ... into`.citeturn0search0 |
| Joining | Correlate related sequences. | `Join`, `GroupJoin`, `join ... on ... equals ...`.citeturn0search0 |
| Aggregation | Reduce to scalar metrics. | `Count`, `Sum`, `Average`, `Aggregate`.citeturn0search0 |

## Execution model

- **Deferred execution:** Sequence-returning operators build expression pipelines that execute only during enumeration, so add terminal operators (for example, `ToList()`) to capture snapshots.citeturn0search0
- **Immediate execution:** Operators that return scalars (`Count`, `Sum`, `Any`) execute immediately to produce results.citeturn0search0
- **Providers:** LINQ to Objects runs in-memory, while `IQueryable<T>` providers (such as EF Core) translate expression trees into domain-specific query languages—be mindful of unsupported constructs.citeturn0search1

## Best practices

- Keep query projections small and deterministic, trimming heavy logic into post-processing steps to avoid inefficient SQL generation with `IQueryable<T>`.citeturn0search1
- Prefer method syntax when combining operators without query keywords (for example, `Zip`, `Chunk`, `ExceptBy`).citeturn0search0
- Encapsulate complex queries into reusable methods returning `IEnumerable<T>`/`IQueryable<T>` to centralize transformations.

## Practice

1. Extend `TopDestinations` to return the top three destinations by revenue using `OrderByDescending` and `Take`.
2. Add a method that groups line items by category and projects an anonymous type showing quantity per category.
3. Build a LINQ to HTTP example using `HttpClient` and `JsonDocument`—query API results with LINQ to Objects after deserializing JSON.

## Further reading

- [Language Integrated Query (LINQ)](https://learn.microsoft.com/dotnet/csharp/linq/)citeturn0search1
- [Standard query operators overview](https://learn.microsoft.com/dotnet/csharp/linq/standard-query-operators/)citeturn0search0
