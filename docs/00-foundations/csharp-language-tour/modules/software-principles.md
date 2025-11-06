---
title: Software Principles in C#
description: Pragmatic implementation of SOLID, DRY, KISS, GRASP, and allied heuristics with .NET 9 code samples.
last_reviewed: 2025-11-03
owners:
  - @prodyum/language-guild
---

# Software Principles & Clean Code

Senior MAUI and cloud engineers balance delivery speed with long-term maintainability. The principles below align with Microsoft's clean architecture recommendations and industry-proven heuristics so you can reason about change safely.citeturn8search0turn1search12

## SOLID principles

SOLID keeps classes cohesive, behaviours extendable, and dependencies testable.citeturn8search0

### Single Responsibility Principle (SRP)

A type should change for one reason. Split UI state from persistence to keep diffs surgical.citeturn8search0

```csharp
public sealed class TodoViewModel
{
    private readonly ITodoService _service;
    public ObservableCollection<TodoItem> Items { get; } = [];

    public TodoViewModel(ITodoService service) => _service = service;
    public async Task LoadAsync(CancellationToken ct) =>
        Items.ReplaceWith(await _service.GetAsync(ct));
}
```

### Open/Closed Principle (OCP)

Extend behaviour without modifying released code. Decorators or partial classes add new capabilities while respecting existing contracts.citeturn8search0

```csharp
public sealed class CachingTodoService : ITodoService
{
    private readonly ITodoService _inner;
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    public CachingTodoService(ITodoService inner) => _inner = inner;
    public async Task<IReadOnlyList<TodoItem>> GetAsync(CancellationToken ct)
    {
        if (_cache.TryGetValue("todos", out IReadOnlyList<TodoItem> cached))
        {
            return cached;
        }

        var items = await _inner.GetAsync(ct);
        _cache.Set("todos", items, TimeSpan.FromMinutes(5));
        return items;
    }
}
```

### Liskov Substitution Principle (LSP)

Derived types must honour the base type's contract so callers can swap implementations without surprises.citeturn8search0

```csharp
public abstract class StorageClient
{
    public abstract Task UploadAsync(string path, Stream payload, CancellationToken ct);
}

public sealed class BlobStorageClient : StorageClient
{
    public override Task UploadAsync(string path, Stream payload, CancellationToken ct) =>
        new BlobClient(path).UploadAsync(payload, overwrite: true, cancellationToken: ct);
}
```

### Interface Segregation Principle (ISP)

Prefer small, role-based interfaces so consumers depend only on what they actually use.citeturn8search0

```csharp
public interface IClipboardService
{
    Task SetTextAsync(string text);
}

public interface IShareService
{
    Task ShareAsync(string title, string text);
}
```

### Dependency Inversion Principle (DIP)

High-level modules depend on abstractions, and concrete implementations live behind DI registrations.citeturn8search0

```csharp
builder.Services.AddScoped<ITelemetrySink, OpenTelemetrySink>();
```

## DRY (Don't Repeat Yourself)

Consolidate behaviour once, expose it through shared modules, and compositionally reuse it.citeturn2search12

```csharp
public static class HttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddJsonDefaults(this IHttpClientBuilder builder) =>
        builder.ConfigureHttpClient(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        });
}
```

## KISS (Keep It Simple, Stupid)

Prefer straightforward solutions that the team can reason about quickly; complexity must earn its place.citeturn2search16

```csharp
public sealed class OtpValidator
{
    public bool IsValid(string otp) => otp.Length == 6 && otp.All(char.IsDigit);
}
```

## YAGNI (You Aren't Gonna Need It)

Only implement features when a real requirement exists; speculative abstractions slow shipping and increase bugs.citeturn3search0

```csharp
// Defer building a plug-in system; start with the single strategy you need.
public sealed class FlatShippingCost : IShippingCalculator
{
    public decimal Calculate(Order order) => 4.95m;
}
```

## Convention over Configuration (CoC)

Leverage predictable defaults so new features align automatically with tooling conventions (naming, folder layout).citeturn2search11

```csharp
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
});
```

## Composition over Inheritance

Compose behaviours by injecting collaborators instead of building deep inheritance trees; maintainers can swap dependencies without touching base classes.citeturn1search12

```csharp
public sealed class ExportJob
{
    private readonly IDataSource _source;
    private readonly IReportWriter _writer;
    public ExportJob(IDataSource source, IReportWriter writer) =>
        (_source, _writer) = (source, writer);
}
```

## Law of Demeter (LoD)

Limit method calls to immediate collaborators—avoid train-wreck expressions that expose internal structure and create brittle coupling.citeturn2search14

```csharp
// Violates LoD: customer.Account.BillingAddress.Country.Code
// Prefer injecting a formatter that knows how to access nested data.
public sealed class AddressPrinter
{
    public string Format(BillingAddress address) => $"{address.City}, {address.Country}";
}
```

## GRASP principles

General Responsibility Assignment Software Principles help teams assign behaviour where it naturally belongs (Controller, Information Expert, High Cohesion).citeturn4search12

```csharp
public sealed class CheckoutController
{
    private readonly OrderService _orderService;
    public CheckoutController(OrderService orderService) => _orderService = orderService;
    public Task<IActionResult> PlaceAsync(PlaceOrder request) =>
        _orderService.CreateAsync(request);
}
```

## Principle alignment checklist

- [ ] Is each class or function tied to one cohesive responsibility (SRP)?citeturn8search0
- [ ] Did you avoid duplicating logic that could live in shared helpers (DRY)?citeturn2search12
- [ ] Is the simplest solution that meets requirements implemented (KISS/YAGNI)?citeturn2search16turn3search0
- [ ] Are dependencies injected via interfaces rather than hard-coded constructions (DIP/Composition over Inheritance)?citeturn8search0turn1search12
- [ ] Do controllers mediate workflow responsibilities instead of pushing logic into views (GRASP Controller)?citeturn4search12

Use this checklist during pull requests to keep the codebase healthy as new features land.




