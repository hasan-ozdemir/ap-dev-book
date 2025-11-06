---
title: Language Members & Contracts
description: Model robust APIs with classes, structs, interfaces, delegates, events, and advanced generics.
last_reviewed: 2025-10-29
owners:
  - @prodyum/language-guild
---

# Language Members & Contracts

This module digs into the types and members you use to express business rules in C#: classes, structs, interfaces, enums, delegates, events, and generics. Each section expands on the syntax, typical use cases, and pitfalls to avoid in mission-critical systems.

## Classes and encapsulation

- Classes are reference types that support inheritance, polymorphism, and encapsulated state; apply access modifiers (`public`, `internal`, `private`) to control visibility.citeturn13search4
- Auto-implemented and init-only properties (`{ get; init; }`) provide concise syntax while enforcing immutability in object initialisers.citeturn13search4
- Seal classes when you do not intend extensibility so downstream consumers rely on interfaces and composition instead of fragile inheritance chains.citeturn3search3

### Example

`ObjectModelShowcase.CustomerAccount` combines init-only properties, guard clauses, and an event to signal balance changes to observers.

```csharp
public sealed class CustomerAccount
{
    public CustomerAccount(string id) => Id = id;

    public string Id { get; }
    public Currency PreferredCurrency { get; init; } = Currency.FromCode("USD");
    public SubscriptionTier PreferredTier { get; init; } = SubscriptionTier.Standard;
    public decimal Balance { get; private set; }

    public event EventHandler<BalanceChangedEventArgs>? BalanceChanged;

    public void Credit(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        var previous = Balance;
        Balance += amount;
        BalanceChanged?.Invoke(this, new BalanceChangedEventArgs(this, previous, Balance));
    }
}
```

## Structs and enums

- Use `struct` for small, immutable value objects that benefit from stack allocation; mark them `readonly` to avoid defensive copies.citeturn11search3
- Enums provide named constants—specify the underlying numeric type and apply `[Flags]` when you need bitwise combinations.citeturn13search2

`ObjectModelShowcase.Currency` is a `readonly record struct` that normalises ISO currency codes, and `SubscriptionTier` uses `[Flags]` to represent combinable feature packs.

## Interfaces and contracts

- Interfaces declare contracts without implementation, enabling dependency inversion and easier mocking in tests.citeturn13search3
- Default interface members help evolve APIs gradually, but keep interfaces small to minimise breaking changes.citeturn13search3

In the sample, `IAccountNotifier` abstracts notification transports so MAUI view models can swap between console, email, or push implementations without touching domain code.

## Delegates, predicates, and events

- Delegates are type-safe references to methods; generic helpers (`Func<T>`/`Action<T>`/`Predicate<T>`) reduce boilerplate for callbacks and filters.citeturn13search6
- Events wrap delegates to protect publishers from external invocation while supporting thread-safe subscription and notification patterns.citeturn13search5

`AccountAuditor` subscribes to `CustomerAccount.BalanceChanged` and raises its own `BalanceChanged` event, letting listeners plug in `Predicate<decimal>` filters for high-value transactions.

## Methods and overloading

- Methods express behaviour; use overloads and optional parameters to provide convenience entry points that delegate to a validated core implementation.citeturn13search8
- Prefer expression-bodied members for simple calculations to keep intent obvious and reduce ceremony.citeturn13search8

## Namespaces and organization

- Namespaces isolate types, prevent naming collisions, and mirror solution boundaries.citeturn13search9
- File-scoped namespaces (`namespace Company.App;`) reduce indentation noise in modern C#.citeturn13search9

## Generics beyond collections

- Generics deliver compile-time safety and reuse; add constraints (`where T : class`, `struct`, `notnull`, `unmanaged`) to clarify requirements and unlock optimisations.citeturn13search7
- Use variance (`in`, `out`) thoughtfully on interfaces and delegates so producers and consumers cooperate across inheritance hierarchies.citeturn13search7
- Combine generics and delegates (e.g., `Predicate<T>`, `Func<T, TResult>`) to implement policy-based designs that callers can customise.citeturn13search6turn13search7

### Sample walkthrough

`ObjectModelShowcase.RunAsync` wires everything together: it subscribes to auditor events, applies a `Predicate<decimal>` to flag large balances, hydrates a generic `List<CustomerAccount>`, and invokes the notifier abstraction.

```csharp
// samples/ConsoleSnippets/ObjectModelShowcase.cs
public static async Task RunAsync()
{
    var account = new CustomerAccount("C-1024")
    {
        PreferredCurrency = Currency.FromCode("USD"),
        PreferredTier = SubscriptionTier.Premium | SubscriptionTier.Analytics
    };

    var auditor = new AccountAuditor();
    auditor.BalanceChanged += (_, e) =>
    {
        if (HighValuePredicate(e.NewBalance))
        {
            Console.WriteLine($"High-value change detected: {e.NewBalance:C}");
        }
    };

    auditor.Subscribe(account);
    account.Credit(1_250m);

    IAccountNotifier notifier = new ConsoleAccountNotifier();
    await notifier.NotifyAsync(account, CancellationToken.None);
}
```

## Practice

1. Add partial classes and file-scoped namespaces to separate generated UI code from hand-crafted domain logic.
2. Extend `AccountAuditor` with a generic decorator that throttles event notifications based on a configurable `TimeSpan`.
3. Introduce a record struct for exchange rates and update `Currency.ConvertTo` to cache delegates that transform between currencies.

---

## Further reading

- [Classes (C# Programming Guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/classes)citeturn13search4
- [Properties (C# Programming Guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties)citeturn13search4
- [Interfaces (C# Programming Guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/interfaces/)citeturn13search3
- [Delegates (C# Programming Guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/delegates/)citeturn13search6
- [Events (C# Programming Guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/events/)citeturn13search5
- [Generics (C# Programming Guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/generics/)citeturn13search7
