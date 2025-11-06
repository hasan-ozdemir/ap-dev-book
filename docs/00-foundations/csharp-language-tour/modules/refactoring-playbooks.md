---
title: Refactoring Playbooks
description: Practical refactoring techniques with sample .NET 9 code and guidance on when to apply them.
last_reviewed: 2025-11-03
owners:
  - @prodyum/language-guild
---

# Refactoring Playbooks

Refactoring improves design without changing behaviour. Use the catalog below to pay down technical debt iteratively while keeping production stable.citeturn9search0turn9search1

## Extract Method

Break long methods into named operations to aid readability and reuse.citeturn9search0

```csharp
public sealed class OrderService
{
    public decimal CalculateTotal(Order order) =>
        SumLines(order) + CalculateShipping(order) - ApplyDiscount(order);

    private static decimal SumLines(Order order) =>
        order.Lines.Sum(line => line.Price * line.Quantity);
    private static decimal CalculateShipping(Order order) =>
        order.RequiresExpress ? 14.95m : 4.95m;
    private static decimal ApplyDiscount(Order order) =>
        order.Customer.IsLoyal ? order.Subtotal * 0.05m : 0m;
}
```

## Introduce Parameter Object

Group related parameters into a value object to clarify intent and prevent mismatched arguments.citeturn9search0

```csharp
public readonly record struct DateRange(DateOnly Start, DateOnly End);

public Task<IReadOnlyList<Booking>> ListAsync(DateRange range, CancellationToken ct) =>
    _context.Bookings
        .Where(b => b.Date >= range.Start && b.Date <= range.End)
        .ToListAsync(ct);
```

## Replace Conditional with Polymorphism

Move branching logic into specialised strategies when `switch` statements multiply.citeturn9search0

```csharp
public abstract class PricingRule
{
    public abstract decimal Calculate(Order order);
}

public sealed class SubscriptionPricing : PricingRule
{
    public override decimal Calculate(Order order) =>
        order.Months * order.MonthlyRate;
}
```

## Extract Interface

Create an interface to capture shared behaviour and enable dependency injection or mocking.citeturn9search0

```csharp
public interface IEmailSender
{
    Task SendAsync(Message message, CancellationToken ct);
}

public sealed class SendGridEmailSender : IEmailSender { /* ... */ }
```

## Encapsulate Collection

Expose collection operations through methods instead of leaking the internal list, preventing uncontrolled mutations.citeturn9search0

```csharp
public sealed class SprintBacklog
{
    private readonly List<WorkItem> _items = [];
    public IReadOnlyCollection<WorkItem> Items => _items.AsReadOnly();
    public void Add(WorkItem item) => _items.Add(item);
    public void Remove(WorkItem item) => _items.Remove(item);
}
```

## Move Method

Place a method on the class that owns the data it operates on to improve cohesion.citeturn9search0turn9search3

```csharp
public sealed class InvoiceLine
{
    public decimal Price { get; init; }
    public decimal Quantity { get; init; }

    public decimal Total() => Price * Quantity;
}
```

## Decompose Conditional

Split complex `if` expressions into intent-revealing boolean properties.citeturn9search0

```csharp
var isHoliday = Holidays.Contains(order.Date);
var requiresSignature = order.Total > 250;

if (isHoliday && requiresSignature)
{
    SchedulePremiumCourier(order);
}
```

## Replace Magic Number with Symbolic Constant

Improve discoverability and future changes by naming domain values.citeturn9search0

```csharp
private const decimal LoyaltyDiscountRate = 0.05m;
```

## Introduce Guard Clauses

Fail fast when inputs are invalid so the happy path stays legible.citeturn9search0turn10search0

```csharp
public sealed class PaymentService
{
    public Task CaptureAsync(Payment payment)
    {
        ArgumentNullException.ThrowIfNull(payment);
        if (payment.Amount <= 0) throw new ArgumentOutOfRangeException(nameof(payment.Amount));
        return _gateway.CaptureAsync(payment);
    }
}
```

## Inline Temp / Inline Variable

Remove unnecessary temporary variables if they hide intent or prevent pattern matching.citeturn9search0

```csharp
if (user is { IsActive: true, Roles: { Count: > 0 } roles })
{
    GrantAccess(roles);
}
```

## Refactoring workflow checklist

1. Protect behaviour with tests before you change structure.citeturn9search1turn9search0
2. Make one small change at a time and run the suite (`dotnet test`) after each step.citeturn9search1turn9search2
3. Track debt items in the backlog so the team can schedule the work intentionally, not as hidden tasks.citeturn9search3

