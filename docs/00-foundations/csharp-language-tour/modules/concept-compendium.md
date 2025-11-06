---
title: C# Programming Concepts Atlas
description: Survey the core building blocks of modern C#—from object modelling and generics to async, LINQ, pattern matching, and memory safety—with runnable examples for each concept.
last_reviewed: 2025-10-30
owners:
  - @prodyum/language-guild
---

# C# Programming Concepts Atlas

C# has evolved from a simple object-oriented language in 2002 to a multi-paradigm platform that spans functional, asynchronous, and performance-critical workloads. This atlas maps the essential building blocks you will encounter in daily work and pairs each concept with a minimal .NET 9 example you can run in the `samples/ConsoleSnippets` project. The coverage mirrors Microsoft's official tour of the language and the latest feature briefs for C# 13 and C# 14 preview.citeturn10view0turn4view0turn5view0

---

## 1. Object model essentials

### 1.1 Classes and objects

Classes define reference types that encapsulate state and behavior. Constructors establish invariants and methods encapsulate work.citeturn10view0

```csharp
public class InvoiceService
{
    private readonly HttpClient _http;
    public InvoiceService(HttpClient http) => _http = http;
    public async Task SubmitAsync(Invoice invoice) =>
        await _http.PostAsJsonAsync("/invoices", invoice);
}
```

### 1.2 Records and value equality

Records provide built-in value semantics, non-destructive mutation via `with`, and succinct data modelling (C# 9).citeturn6view0

```csharp
public record Invoice(Guid Id, decimal Total)
{
    public string Status { get; init; } = "Draft";
}

var approved = invoice with { Status = "Approved" };
```

### 1.3 Structs and readonly data

Structs create value types that avoid heap allocations. Use `readonly struct` for immutable, stack-friendly primitives.citeturn10view0

```csharp
public readonly struct Money(decimal amount, string currency)
{
    public decimal Amount { get; } = amount;
    public string Currency { get; } = currency;
}
```

### 1.4 Interfaces and abstraction

Interfaces expose contracts without implementation, supporting dependency inversion and unit testing.citeturn10view0

```csharp
public interface IRepository<T>
{
    Task<T> GetAsync(Guid id);
}

public class CosmosRepository<T> : IRepository<T>
{
    public Task<T> GetAsync(Guid id) => Task.FromResult(default(T)!);
}
```

### 1.5 Inheritance and polymorphism

Use `virtual`, `override`, and `sealed` to compose behavior across class hierarchies.citeturn10view0

```csharp
public class RestClient
{
    public virtual HttpRequestMessage BuildRequest() => new(HttpMethod.Get, "/");
}

public sealed class PagingClient : RestClient
{
    public override HttpRequestMessage BuildRequest() =>
        new(HttpMethod.Get, "/?page=next");
}
```

---

## 2. Members, encapsulation, and extension

### 2.1 Properties, indexers, and init-only setters

Properties expose state with validation. `init` setters enforce immutability during object creation.citeturn9view0turn10view0

```csharp
public class Settings
{
    public required string Environment { get; init; }
    public string Theme { get; private set; } = "Light";
    public string this[string key] => $"{Environment}:{key}";
}
```

### 2.2 Events and delegates

Delegates store callable references; events wrap them with publishing semantics.citeturn10view0

```csharp
public delegate Task PipelineStepAsync(HttpContext context);

public class Pipeline
{
    public event EventHandler<HttpContext>? Executed;

    public async Task RunAsync(HttpContext context, PipelineStepAsync step)
    {
        await step(context);
        Executed?.Invoke(this, context);
    }
}
```

### 2.3 Operator overloads

Operator overloads create domain-specific syntax (use sparingly for clarity).citeturn10view0

```csharp
public readonly struct Percentage(decimal value)
{
    private readonly decimal _value = value;
    public static Percentage operator +(Percentage left, Percentage right) =>
        new(left._value + right._value);
}
```

### 2.4 Extension methods and members

Extension methods add functionality without modifying the original type. C# 14 extension members generalise the idea.citeturn10view0turn5view0

```csharp
public static class HttpClientExtensions
{
    public static Task<T?> GetJsonAsync<T>(this HttpClient http, string path) =>
        http.GetFromJsonAsync<T>(path);
}
```

### 2.5 Partial types and source generation

Partial types let tooling (source generators, MAUI XAML) and humans share class definitions safely.citeturn10view0

```csharp
public partial class GeneratedViewModel
{
    partial void OnPropertyChanged(string propertyName);
}
```

---

## 3. Generics and reusable code

### 3.1 Generic types and methods

Generics enable type-safe reuse without boxing or casting.citeturn10view0

```csharp
public class Result<T>
{
    public required T Value { get; init; }
    public string? Error { get; init; }
}
```

### 3.2 Constraints, covariance, and contravariance

Constraints limit valid type arguments; variance annotations (`out`, `in`) handle inheritance relationships for interfaces and delegates.citeturn10view0

```csharp
public interface IRepository<out T>
{
    Task<T> GetAsync(Guid id);
}

public class Factory<T> where T : new()
{
    public T Create() => new();
}
```

### 3.3 Generic math and static virtual members (C# 11)

Static virtual interfaces unlock generic algorithms over numeric types.citeturn4view0

```csharp
public static class MathExtensions<T> where T : INumber<T>
{
    public static T Average(T left, T right) => (left + right) / T.CreateChecked(2);
}
```

---

## 4. Collections, LINQ, and data pipelines

### 4.1 Enumerables and iterators

Implement `IEnumerable<T>` to support `foreach` and deferred execution.citeturn10view0

```csharp
public static IEnumerable<int> EvenUpTo(int max)
{
    for (int i = 0; i <= max; i++)
    {
        if (i % 2 == 0) yield return i;
    }
}
```

### 4.2 LINQ query expressions

LINQ offers declarative data transformations that compose across in-memory, database, and streaming sources.citeturn10view0citeturn10view0

```csharp
var topRegions =
    from order in orders
    where order.Total > 100m
    orderby order.Total descending
    group order by order.Region into regionGroup
    select new { Region = regionGroup.Key, Total = regionGroup.Sum(o => o.Total) };
```

### 4.3 Asynchronous streams

`IAsyncEnumerable<T>` supports streaming data with `await foreach` (C# 8).citeturn9view0turn10view0

```csharp
await foreach (var reading in sensor.ReadingsAsync())
{
    Console.WriteLine(reading);
}
```

### 4.4 Span, Memory, and pipelines

`Span<T>` and `Memory<T>` enable slicing without allocations—critical for parsing and IO pipelines.citeturn12view0turn7view0

```csharp
Span<byte> buffer = stackalloc byte[256];
var header = buffer[..8];
header.Clear();
```

---

## 5. Pattern matching and functional style

### 5.1 Relational, logical, and list patterns

Modern `switch` expressions combine relational, logical, and list patterns for concise branching (C# 11).citeturn4view0

```csharp
string Describe<T>(IReadOnlyList<T> values) => values switch
{
    [] => "Empty",
    [var single] => $"Single: {single}",
    [_, .., _] => $"Count {values.Count}",
    _ => "Unknown"
};
```

### 5.2 Property and positional patterns

Patterns deconstruct complex objects inline.citeturn4view0

```csharp
if (order is { Customer: { Status: "Gold" }, Total: > 100m })
{
    Console.WriteLine("Apply loyalty perks");
}
```

---

## 6. Async, parallelism, and reactive flows

### 6.1 Task-based async model

`Task` and `ValueTask` represent asynchronous operations. Chain them with `await` for linear code flow.citeturn10view0

```csharp
public async Task<Invoice> LoadAndCacheAsync(Guid id)
{
    if (cache.TryGetValue(id, out var cached)) return cached;
    var invoice = await api.GetFromJsonAsync<Invoice>($"/invoices/{id}");
    cache[id] = invoice!;
    return invoice!;
}
```

### 6.2 Parallel workloads

`Parallel.ForEachAsync` orchestrates degree-of-parallelism with async delegates (.NET 6+).citeturn13view0citeturn9view0turn10view0

```csharp
await Parallel.ForEachAsync(invoices, parallelOptions, async (invoice, token) =>
{
    await processor.ProcessAsync(invoice, token);
});
```

### 6.3 Reactive streams

Combine `Channel<T>` or `IObservable<T>` for push-based processing.citeturn0search8

```csharp
var channel = Channel.CreateUnbounded<int>();
_ = Task.Run(async () =>
{
    await foreach (var item in channel.Reader.ReadAllAsync())
    {
        Console.WriteLine(item);
    }
});
await channel.Writer.WriteAsync(42);
```

---

## 7. Error handling, diagnostics, and contracts

### 7.1 Exceptions and filters

Use exception filters for targeted remediation without deep nesting.citeturn10view0

```csharp
try
{
    await client.SubmitAsync(invoice);
}
catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.TooManyRequests)
{
    telemetry.Track("throttled");
}
```

### 7.2 Guard clauses and pattern-based validation

Combine pattern matching with guards to fail fast.citeturn4view0

```csharp
public static void EnsurePositive(decimal amount)
{
    if (amount is <= 0)
    {
        throw new ArgumentOutOfRangeException(nameof(amount));
    }
}
```

### 7.3 Diagnostics and logging

`ILogger` abstractions integrate with structured logging sinks (Serilog, Application Insights).citeturn14view0

```csharp
public class CheckoutHandler(ILogger<CheckoutHandler> logger)
{
    public void Handle(Order order) =>
        logger.LogInformation("Processing {@OrderId} for {@Total}", order.Id, order.Total);
}
```

---

## 8. Reflection, attributes, and source generators

### 8.1 Attributes

Decorate types with metadata for runtime inspection.citeturn10view0

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class FeatureFlagAttribute(string flag) : Attribute
{
    public string Flag { get; } = flag;
}

[FeatureFlag("Invoices")]
public class InvoiceService { }
```

### 8.2 Reflection APIs

Fetch and act on metadata dynamically.citeturn10view0

```csharp
var flags = typeof(InvoiceService).GetCustomAttributes<FeatureFlagAttribute>();
foreach (var flag in flags)
{
    Console.WriteLine(flag.Flag);
}
```

### 8.3 Source generation hooks

`IIncrementalGenerator` enables compile-time code generation that feeds partial classes.citeturn4view0

```csharp
public class EndpointGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Inspect syntax and emit sources.
    }
}
```

---

## 9. Interop, unsafe code, and low-level constructs

### 9.1 P/Invoke and extern methods

`extern` + `[DllImport]` bridge managed and native libraries.citeturn8view0

```csharp
[DllImport("kernel32.dll")]
public static extern bool Beep(uint frequency, uint duration);
```

### 9.2 Unsafe code blocks

Use `unsafe`, pointers, and `stackalloc` when you control memory manually (guarded by reviews and analyzers).citeturn8view0

```csharp
unsafe Span<int> CreateStackBuffer()
{
    int* buffer = stackalloc int[4];
    return new Span<int>(buffer, 4);
}
```

### 9.3 Function pointers

Function pointers remove delegate overhead in high-frequency paths.citeturn8view0

```csharp
unsafe delegate* unmanaged<int, int, int> Adder;
```

---

## 10. Top-level programs, scripting, and tooling

### 10.1 Top-level statements

You can omit `Program` and `Main` for quick scripts (C# 9).citeturn4view0

```csharp
Console.WriteLine("Hello from a top-level program");
await Task.Delay(10);
```

### 10.2 File-scoped namespaces

Reduce indentation and noise in source files.citeturn10view0

```csharp
namespace Contoso.Payments;
```

### 10.3 Pattern-based configuration with `switch` expressions

Use `switch` expressions for configuration loading and DI registries.citeturn4view0

```csharp
var regionEndpoint = region switch
{
    "US" => "https://us.api.contoso.com",
    "EU" => "https://eu.api.contoso.com",
    _ => throw new NotSupportedException(region)
};
```

---

## 11. Putting it all together

Combine these concepts to build production-grade MAUI, web, and service workloads:

1. Model data with records or structs where value semantics matter.
2. Compose services with interfaces, dependency injection, and async flows.
3. Transform data via LINQ or `IAsyncEnumerable<T>` pipelines.
4. Optimise hotspots with spans, pooling, and function pointers.
5. Illuminate behaviour using structured logging, diagnostics, and source-generated observability helpers.

Updating this atlas regularly keeps the team aligned on language capabilities as C# 14 and .NET 10 arrive.






































