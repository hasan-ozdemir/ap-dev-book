---
title: Design Patterns with C#
description: End-to-end catalogue of Gang of Four and enterprise patterns implemented with modern .NET 9 idioms.
last_reviewed: 2025-11-03
owners:
  - @prodyum/language-guild
---

# Design Patterns with Modern C#

Design patterns capture reusable problem/solution pairs that keep large codebases extensible when the product roadmap evolves quickly. Modern C# still relies on the Gang of Four (GoF) catalogue, and Microsoft positions these patterns as a foundation for resilient application architectures.citeturn1search0turn1search1

> **How to practice:** Each pattern includes a concise `.NET 9` console snippet. Copy the block into `samples/ConsoleSnippets`, run `dotnet run`, and then extend the example for your current project.

## Creational patterns

Creational patterns encapsulate object construction so callers stay agnostic of concrete types and configuration.citeturn1search0

### Abstract Factory

Abstract Factory creates families of related UI components without exposing their concrete implementations—a perfect fit for MAUI cross-platform widget packs.citeturn1search0

```csharp
public interface IWidgetFactory
{
    IButton CreateButton();
    IMenu CreateMenu();
}

public sealed class CupertinoWidgetFactory : IWidgetFactory
{
    public IButton CreateButton() => new CupertinoButton();
    public IMenu CreateMenu() => new CupertinoMenu();
}
```

### Builder

Builder assembles complex aggregates step-by-step, letting you vary configuration (for example subscription tiers) without combinatorial constructors.citeturn1search0

```csharp
public sealed class ReceiptBuilder
{
    private readonly List<ReceiptLine> _lines = [];

    public ReceiptBuilder AddLine(string sku, decimal price) =>
        _lines.Add(new ReceiptLine(sku, price)) is var _ ? this : this;

    public Receipt Build() => new(_lines.Sum(l => l.Price), _lines.ToArray());
}
```

### Factory Method

Factory Method moves object creation into subclasses, allowing each platform to decide which concrete type to return while callers depend on the abstraction.citeturn1search0

```csharp
public abstract class NotificationFactory
{
    public abstract Notification Create();
}

public sealed class PushNotificationFactory : NotificationFactory
{
    public override Notification Create() => new PushNotification();
}
```

### Prototype

Prototype clones preconfigured instances when object setup is expensive (for example, seeded view models or HTTP clients with security headers).citeturn1search0

```csharp
public record Theme(string Name, Color Primary, Color Accent)
{
    public Theme CloneWithAccent(Color accent) => this with { Accent = accent };
}
```

### Singleton

Singleton guarantees a single instance for shared infrastructure such as telemetry pipelines. Prefer `Lazy<T>` to handle thread-safe initialization.citeturn1search0

```csharp
public sealed class TelemetryHub
{
    private static readonly Lazy<TelemetryHub> _instance =
        new(() => new TelemetryHub());

    public static TelemetryHub Instance => _instance.Value;
}
```

## Structural patterns

Structural patterns compose or wrap objects to expand functionality without rewriting core types.citeturn1search0turn1search12

### Adapter

Adapter translates an incompatible API into a domain-friendly contract—ideal when wrapping platform SDKs for dependency injection.citeturn1search0

```csharp
public interface IBlobStore
{
    Task UploadAsync(Stream payload, string path, CancellationToken ct);
}

public sealed class BlobStorageAdapter : IBlobStore
{
    private readonly BlobClient _client;
    public BlobStorageAdapter(BlobClient client) => _client = client;

    public Task UploadAsync(Stream payload, string path, CancellationToken ct) =>
        _client.UploadAsync(payload, overwrite: true, cancellationToken: ct);
}
```

### Bridge

Bridge decouples an abstraction from its implementation so they can vary independently—for example, swapping renderers while reusing business logic.citeturn1search0

```csharp
public interface IRenderer { void Draw(ControlModel model); }

public abstract class Control
{
    protected Control(IRenderer renderer) => Renderer = renderer;
    protected IRenderer Renderer { get; }
    public void Render(ControlModel model) => Renderer.Draw(model);
}
```

### Composite

Composite treats individual controls and groups uniformly, enabling recursive operations such as toggling feature flags across a nested page.citeturn1search0

```csharp
public interface IUiNode
{
    void SetEnabled(bool enabled);
}

public sealed class CompositeNode : IUiNode
{
    private readonly List<IUiNode> _children = [];
    public void Add(IUiNode node) => _children.Add(node);
    public void SetEnabled(bool enabled)
    {
        foreach (var child in _children)
        {
            child.SetEnabled(enabled);
        }
    }
}
```

### Decorator

Decorator layers cross-cutting concerns—resilience, caching, telemetry—without modifying the wrapped service.citeturn1search0

```csharp
public sealed class TelemetryTodoService : ITodoService
{
    private readonly ITodoService _inner;
    private readonly ActivitySource _source;

    public TelemetryTodoService(ITodoService inner, ActivitySource source)
    {
        _inner = inner;
        _source = source;
    }

    public async Task<IReadOnlyList<TodoItem>> GetAsync(CancellationToken ct)
    {
        using var activity = _source.StartActivity("todos.get");
        return await _inner.GetAsync(ct);
    }
}
```

### Facade

Facade aggregates complex subsystems behind a minimal API, which keeps UI code simple and test doubles straightforward.citeturn1search0

```csharp
public sealed class CheckoutFacade
{
    public Task<Receipt> CompleteAsync(Order order, CancellationToken ct) =>
        new CheckoutWorkflow().ExecuteAsync(order, ct);
}
```

### Flyweight

Flyweight shares immutable data to reduce memory—helpful for static resources like SVG icons or localization strings.citeturn1search0

```csharp
public sealed class IconFlyweightFactory
{
    private readonly ConcurrentDictionary<string, Icon> _cache = new();

    public Icon Get(string name) => _cache.GetOrAdd(name, Load);
    private static Icon Load(string name) => Icon.FromResource(name);
}
```

### Proxy

Proxy controls access to another object, injecting lazy loading or security checks without touching the target implementation.citeturn1search0

```csharp
public sealed class SecureDocumentProxy : IDocumentStore
{
    private readonly IDocumentStore _inner;
    private readonly ICurrentUser _currentUser;

    public SecureDocumentProxy(IDocumentStore inner, ICurrentUser currentUser) =>
        (_inner, _currentUser) = (inner, currentUser);

    public Task<Stream> OpenAsync(string id, CancellationToken ct)
    {
        _currentUser.EnsureHasScope("documents.read");
        return _inner.OpenAsync(id, ct);
    }
}
```

## Behavioural patterns

Behavioural patterns manage algorithms, communication, and state transitions so collaboration logic remains explicit and testable.citeturn1search0

### Chain of Responsibility

Chain of Responsibility passes a request through a pipeline until one handler processes it, mirroring middleware and validation chains.citeturn1search0

```csharp
public abstract class Handler
{
    private Handler? _next;
    public Handler Register(Handler next) => _next = next;
    public virtual ValueTask<bool> HandleAsync(CommandContext ctx) =>
        _next?.HandleAsync(ctx) ?? ValueTask.FromResult(false);
}
```

### Command

Command encapsulates an action and parameters, unlocking undo stacks and queue-based execution.citeturn1search0

```csharp
public sealed record RenameFile(string Path, string NewName) : ICommand
{
    public ValueTask ExecuteAsync() =>
        ValueTask.FromResult(File.Move(Path, NewName));
}
```

### Interpreter

Interpreter defines a grammar and evaluation logic—useful for feature flag expressions or lightweight rule engines.citeturn1search0

```csharp
public interface IExpression
{
    bool Evaluate(IDictionary<string, bool> context);
}

public sealed class Variable : IExpression
{
    private readonly string _name;
    public Variable(string name) => _name = name;
    public bool Evaluate(IDictionary<string, bool> context) => context[_name];
}
```

### Iterator

Iterator provides a consistent way to traverse collections without exposing the underlying representation.citeturn1search0

```csharp
public sealed class ReverseEnumerable<T>(IReadOnlyList<T> source)
{
    public IEnumerable<T> Enumerate()
    {
        for (var i = source.Count - 1; i >= 0; i--)
        {
            yield return source[i];
        }
    }
}
```

### Mediator

Mediator centralises collaboration so components interact through a coordinator rather than a web of direct references.citeturn1search0

```csharp
public interface IMediator
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken ct);
}

public sealed class ChatMediator : IMediator
{
    private readonly List<IChatClient> _clients = [];
    public Task PublishAsync<TMessage>(TMessage message, CancellationToken ct)
    {
        foreach (var client in _clients) client.Receive(message);
        return Task.CompletedTask;
    }
}
```

### Memento

Memento captures and restores an object's state—ideal for undo stacks or persisting wizard steps between sessions.citeturn1search0

```csharp
public sealed class EditorMemento(string text)
{
    public string Text { get; } = text;
}

public sealed class TextEditor
{
    private readonly Stack<EditorMemento> _history = new();
    public string Text { get; private set; } = string.Empty;
    public void SetText(string text)
    {
        _history.Push(new EditorMemento(Text));
        Text = text;
    }
    public void Undo() => Text = _history.Pop().Text;
}
```

### Observer

Observer keeps subscribers informed about state changes without tight coupling—think MAUI event streams or analytics hooks.citeturn1search0

```csharp
public sealed class PriceFeed : IObservable<decimal>
{
    private readonly List<IObserver<decimal>> _subscribers = [];

    public IDisposable Subscribe(IObserver<decimal> observer)
    {
        _subscribers.Add(observer);
        return new Unsubscriber(_subscribers, observer);
    }

    public void Publish(decimal value)
    {
        foreach (var subscriber in _subscribers) subscriber.OnNext(value);
    }
}
```

### State

State encapsulates state-specific behaviour inside dedicated classes so transitions stay explicit.citeturn1search0

```csharp
public interface ICheckoutState
{
    ValueTask<ICheckoutState> NextAsync(OrderContext context);
}
```

### Strategy

Strategy defines a family of algorithms and makes them interchangeable—handy for pricing rules or device-specific layout policies.citeturn1search0

```csharp
public interface ITaxStrategy
{
    decimal Calculate(Order order);
}

public sealed class EuVatStrategy : ITaxStrategy
{
    public decimal Calculate(Order order) => order.Subtotal * 0.21m;
}
```

### Template Method

Template Method outlines invariant workflow steps while letting subclasses override specific stages—common in pipeline orchestration.citeturn1search0

```csharp
public abstract class DocumentGenerator
{
    public byte[] Create()
    {
        var data = FetchData();
        var formatted = Format(data);
        return Export(formatted);
    }

    protected abstract object FetchData();
    protected abstract object Format(object data);
    protected abstract byte[] Export(object formatted);
}
```

### Visitor

Visitor adds new operations to object structures without modifying the element classes, keeping rendering and validation logic modular.citeturn1search0

```csharp
public interface IReportVisitor
{
    void Visit(TotalRevenue report);
    void Visit(ChurnReport report);
}
```

## Additional enterprise patterns

Beyond the GoF catalogue, .NET teams rely on architectural and enterprise patterns that align with Domain-Driven Design (DDD) guidance.citeturn1search12turn0search5

### Dependency Injection

Dependency Injection externalises object creation so high-level policies depend on abstractions. MAUI's `MauiProgram` uses the generic host container to register services at startup.citeturn6search0turn0search0

```csharp
builder.Services.AddSingleton<IWeatherService, WeatherService>();
```

### Repository

Repository mediates between domain and data layers, exposing collection-like operations while hiding storage details.citeturn6search3

```csharp
public interface IOrderRepository
{
    Task<Order?> FindAsync(Guid id, CancellationToken ct);
    Task SaveAsync(Order order, CancellationToken ct);
}
```

### Unit of Work

Unit of Work groups repository changes into a single transaction so commits stay atomic.citeturn6search3

```csharp
public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken ct);
}
```

### CQRS

Command Query Responsibility Segregation splits write and read models to scale independently—a proven pattern for enterprise-grade mobile backends.citeturn6search1

```csharp
public sealed record PlaceOrder(Guid Id, decimal Total) : ICommand;
public sealed record OrderDetails(Guid Id, decimal Total) : IQuery<OrderReadModel>;
```

### Specification

Specification composes business rules into reusable predicates that you can apply to repositories or LINQ queries.citeturn6search3

```csharp
public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();
}
```

## Choosing the right pattern

1. Start with the problem statement—prefer the pattern that minimises coupling and maximises clarity.citeturn1search0
2. Prefer composition over inheritance; most patterns encourage plugging behaviours together rather than subclassing.citeturn1search12
3. Document the decision and trade-offs in an Architecture Decision Record (ADR) so future teams understand the context.citeturn6search2



