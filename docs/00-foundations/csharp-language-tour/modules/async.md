---
title: Asynchrony & Concurrency
description: Master async/await, ValueTask, coordination primitives, and async streams to build responsive .NET services.
last_reviewed: 2025-10-29
owners:
  - @prodyum/language-guild
---

# Asynchrony & Concurrency

Modern .NET apps rely on asynchronous code to stay responsive under load. This module shows how to apply the Task-based Asynchronous Pattern (TAP), choose between `Task` and `ValueTask`, coordinate concurrent work safely, and stream data with `IAsyncEnumerable<T>`.citeturn14search0turn14search2

## Prerequisites

- Review [`Program` structure](./program-structure.md) and the [`samples/ConsoleSnippets`](https://github.com/hasan-ozdemir/devyum/tree/main/samples/ConsoleSnippets) project.
- Optional: read [Generics & collections](./generics.md) for background on `Func<T>`/`Predicate<T>` delegates.

---

## Async/await essentials

- Add `async` and return `Task`/`Task<TResult>` for methods that perform I/O without blocking threads; the compiler generates a state machine that resumes when awaited operations finish.citeturn14search2
- Await every task you start--unobserved tasks can swallow exceptions and leak resources.citeturn14search2
- Prefer natural async methods over `Task.Run` in ASP.NET handlers; reserve `Task.Run` for CPU-bound work you deliberately move off the request thread.citeturn14search2

### Example: fast vs slow cache lookup

See `AsyncShowcase.FetchProductAsync` in the sample project for a full implementation that coalesces cache hits and falls back to the database without blocking.

```csharp
// samples/ConsoleSnippets/AsyncShowcase.cs
public static ValueTask<string> FetchProductAsync(string sku, CancellationToken token)
{
    if (Cache.TryGetValue(sku, out var cached))
    {
        return ValueTask.FromResult(cached); // completes synchronously
    }

    var gate = InFlight.GetOrAdd(sku, static _ => new SemaphoreSlim(1, 1));
    return FetchAndCacheAsync(sku, gate, token);
}

private static async ValueTask<string> FetchAndCacheAsync(
    string sku, SemaphoreSlim gate, CancellationToken token)
{
    await gate.WaitAsync(token).ConfigureAwait(false);
    try
    {
        if (Cache.TryGetValue(sku, out var cached))
        {
            return cached;
        }

        var record = await Catalog.LookupAsync(sku, token).ConfigureAwait(false);
        Cache[sku] = record;
        return record;
    }
    finally
    {
        gate.Release();
        InFlight.TryRemove(sku, out _);
    }
}
```

Why it matters: `await` yields control instead of blocking threads, letting the thread pool handle more concurrent work.citeturn14search2

---

## Choosing Task vs ValueTask

| Use case | Return type | Notes |
| --- | --- | --- |
| Always asynchronous, or need multiple awaits | `Task` / `Task<T>` | Simple, thread-safe, and reusable across awaiters.citeturn14search5 |
| Often completes synchronously (e.g., cache hits, pooled buffers) | `ValueTask` / `ValueTask<T>` | Avoids allocations but may be awaited only once; call `.AsTask()` if reuse is required.citeturn14search5 |

- Never await a `ValueTask` multiple times or concurrently--convert to a `Task` first.citeturn14search5
- Avoid blocking calls such as `.Result` or `.Wait()` on `ValueTask`; convert to `Task` and block only as a last resort.citeturn14search5

The sample's `AsyncShowcase.FetchProductAsync` returns `ValueTask<string>` to optimize cached responses while still supporting async fallbacks.

---

## Coordinating concurrent work

- Batch I/O with `Task.WhenAll` to reduce latency across parallel API calls, wrapping the await in `try/catch` to handle aggregate exceptions.citeturn15search1
- Propagate cancellation using `CancellationToken` parameters so cooperative operations stop promptly when callers disconnect.citeturn15search0
- Avoid deadlocks in libraries by using `ConfigureAwait(false)` when you do not require the caller's context.citeturn15search2

```csharp
public static async Task<IDictionary<string, decimal>> FetchRatesAsync(
    IEnumerable<string> symbols, CancellationToken token)
{
    var lookups = symbols.Select(symbol => Catalog.GetQuoteAsync(symbol, token)).ToArray();
    await Task.WhenAll(lookups).ConfigureAwait(false);
    return lookups.ToDictionary(task => task.Result.symbol, task => task.Result.price);
}
```

---

## Streaming with IAsyncEnumerable<T>

- Use `async IAsyncEnumerable<T>` plus `yield return` to push incremental results without buffering the entire sequence.citeturn14search6
- Consume sequences with `await foreach`, optionally supplying `WithCancellation(token)` to stop mid-stream.citeturn14search6
- Dispose resources via `await using` inside enumerators that manage external handles.citeturn14search6

```csharp
public static async IAsyncEnumerable<double> StreamTelemetryAsync(
    [EnumeratorCancellation] CancellationToken token = default)
{
    while (true)
    {
        token.ThrowIfCancellationRequested();
        await Task.Delay(TimeSpan.FromMilliseconds(70), token).ConfigureAwait(false);
        yield return Math.Round(Random.Shared.NextDouble() * 100, 2);
    }
}
```

The sample's `AsyncShowcase.StreamTelemetryAsync` demonstrates these patterns and writes live readings to the console.

---

## Failure modes & mitigations

| Risk | Mitigation |
| --- | --- |
| Deadlocks from resuming on captured sync contexts (e.g., legacy UI threads). | Use `ConfigureAwait(false)` in library code, and keep call chains async end-to-end.citeturn15search2 |
| Multiple awaits on a `ValueTask` leading to corrupted state. | Await exactly once or convert to `Task` via `.AsTask()`.citeturn14search5 |
| Resource leaks in async streams when consumers stop early. | Wrap enumerators in `await using` so `DisposeAsync` runs on cancellation.citeturn14search6 |

---

## Practice

1. Cache probe: Extend `FetchProductAsync` to track cache hit ratios with `Interlocked` APIs.
2. Fan-out service call: Add a method that fans out to two HTTP services, races them with `Task.WhenAny`, and cancels the loser via linked tokens.
3. Telemetry enrichment: Pipe `StreamTelemetryAsync` into LINQ `Select`/`Where` operators to flag outliers without buffering the entire stream.

---

## Further reading

- [Task asynchronous programming model (TAP) overview](https://learn.microsoft.com/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap)citeturn14search0
- [Asynchronous programming with async and await](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/)citeturn14search2
- [CA2012: Use ValueTasks correctly](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2012)citeturn14search5
- [Iterating with async enumerables in C#](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/iteration-statements#await-foreach)citeturn14search6
