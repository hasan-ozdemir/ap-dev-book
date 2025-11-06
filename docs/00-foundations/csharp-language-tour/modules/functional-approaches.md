title: Functional Approaches & LINQ
description: Functional programming patterns, LINQ techniques, and immutability practices in modern C#.
last_reviewed: 2025-11-03
owners:
  - @prodyum/language-guild
---

# Functional Approaches and LINQ

C# 13/14 and .NET 9 bring first-class features for functional thinking—records, pattern matching, span-friendly LINQ, and asynchronous pipelines—so MAUI and cloud services can minimise side effects while staying fast.citeturn1search0turn0search0turn3search2

## 1. Immutability with records and persistent collections

- `record class` and `record struct` types provide value-based equality plus `with` expressions, making it trivial to clone view models or DTOs without mutating existing instances.citeturn1search0
- `System.Collections.Immutable` offers thread-safe collections backed by structural sharing, which remain AOT-friendly for MAUI deployments.citeturn1search2

```csharp
public record TodoItem(Guid Id, string Title, bool Completed)
{
    public TodoItem MarkDone() => this with { Completed = true };
}
```

Persist immutable snapshots (for example cached query results) by storing records or immutable lists in `Preferences` or local databases so UI layers stay deterministic.

## 2. Modern pattern matching

- List and slice patterns let you declaratively check collection shapes, removing manual indexing logic.citeturn0search0
- Combine guards (`when`) with pattern matching to validate inbound data before it reaches application services.

```csharp
static string Describe(ReadOnlySpan<int> scores) => scores switch
{
    [>= 90, ..] => "High performer",
    [< 50, ..] => "Needs support",
    _ => "On track"
};
```

## 3. LINQ strategies at scale

| Technique | Why it matters | When to apply |
|-----------|----------------|---------------|
| **Deferred execution** | LINQ queries run only when enumerated, reducing accidental database or API calls. Materialise explicitly with `ToList()` in UI layers. | Paging results in MAUI list pages. |
| **Parallel LINQ (PLINQ)** | `AsParallel()` distributes CPU-bound work while preserving logical query syntax. | Batch analytics and report generation on background threads.citeturn2search3 |
| **Span-enabled LINQ** | .NET 9 extends `Where`, `Select`, and friends to `Span<T>`/`ReadOnlySpan<T>`, cutting allocations for text-heavy workloads.citeturn3search2 |

```csharp
ReadOnlySpan<char> pipeline = "a,b,c".AsSpan();
var upper = pipeline.Split(',')
                    .Select(static c => char.ToUpperInvariant(c[0]))
                    .ToArray();
```

Span-aware LINQ avoids intermediate string allocations, easing GC pressure in mobile scenarios.citeturn3search2

## 4. Functional composition and helpers

- Compose `Func<T>` delegates to express complex transformations from smaller units; extension methods can keep the syntax readable.
- `CommunityToolkit.HighPerformance` exposes `Span2D`, `RefEnumerable`, and other helpers that keep pipelines allocation-free.citeturn0search2turn0search4

```csharp
Func<T, TResult> Compose<T, TMid, TResult>(Func<T, TMid> first, Func<TMid, TResult> second) =>
    x => second(first(x));
```

## 5. Asynchronous pipelines

- `System.Threading.Channels` pairs with `IAsyncEnumerable<T>` to build back-pressure-aware event pipelines for telemetry, sync queues, or background jobs.citeturn0search1turn3search0
- In UI layers, expose channels via `ObservableCollection` updates on the main thread, or bridge to `IObservable<T>` for reactive presentation logic.citeturn3search0

```csharp
var channel = Channel.CreateUnbounded<TodoItem>();

_ = Task.Run(async () =>
{
    await foreach (var item in channel.Reader.ReadAllAsync(ct))
    {
        await MainThread.InvokeOnMainThreadAsync(() => Items.Add(item));
    }
});
```

## 6. Testing guidance

- Pure functions—no captured state, no IO—are trivial to test with xUnit `[Theory]` + `InlineData`.
- Validate LINQ queries with `FluentAssertions` sequence equality checks to ensure query refactors keep ordering and projection correct.
- Instrument pipelines with counters via `System.Diagnostics.Metrics` so performance regressions surface early in CI.citeturn3search2
