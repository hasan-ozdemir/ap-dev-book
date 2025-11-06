---
title: C# Keyword Compendium
description: Exhaustive reference for every reserved and contextual keyword available from C# 1.0 through C# 13, with runnable examples and upgrade notes.
last_reviewed: 2025-10-30
owners:
  - @prodyum/language-guild
---

# C# Keyword Compendium

Modern C# spans dozens of reserved and contextual keywords. This compendium groups them by the problem they solve, explains the intent behind each token, and provides a runnable snippet you can paste into `samples/ConsoleSnippets` to experiment. The lists and definitions mirror the official language specification and Microsoft Learn guidance current to C# 13.citeturn3view0

> **Summary of keyword categories**
>
> | Category | Keywords |
> | --- | --- |
> | Type declarations | `class`, `struct`, `interface`, `enum`, `delegate`, `record`, `event`, `module` |
> | Access & modifiers | `public`, `protected`, `internal`, `private`, `static`, `abstract`, `sealed`, `virtual`, `override`, `new`, `readonly`, `volatile`, `extern` |
> | Parameters & generics | `in`, `out`, `ref`, `params`, `where`, `notnull`, `unmanaged`, `allows`, `scoped` |
> | Flow control | `if`, `else`, `switch`, `case`, `default`, `when`, `for`, `foreach`, `while`, `do`, `break`, `continue`, `return`, `yield`, `goto` |
> | Exceptions | `try`, `catch`, `finally`, `throw` |
> | Memory & interop | `unsafe`, `fixed`, `stackalloc`, `sizeof`, `typeof`, `checked`, `unchecked`, `lock`, `using` |
> | Primitives & literals | `bool`, `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `float`, `double`, `decimal`, `string`, `object`, `void`, `true`, `false`, `null` |
> | Pattern & LINQ helpers | `is`, `as`, `and`, `or`, `not`, `from`, `where`, `select`, `join`, `group`, `into`, `orderby`, `ascending`, `descending`, `let`, `by`, `equals` |
> | Async & context | `async`, `await`, `yield`, `var`, `dynamic`, `nameof`, `global`, `file`, `record`, `with`, `required`, `init`, `extension`, `field` |

> **How to use this page**
>
> 1. Browse the sections that match the code you are writing (types, flow control, memory, LINQ, etc.).
> 2. Copy the sample into a scratch file and run it with `dotnet run --project samples/ConsoleSnippets`.
> 3. Follow the upgrade notes to understand when a keyword was introduced or broadened so you can plan migrations.

---

## 1. Reserved keywords (compiler-reserved identifiers)

These keywords are reserved by the compiler in every scope. You cannot use them as identifiers unless you escape them with `@`.citeturn3view0

### 1.1 Type declarations and inheritance

`class` — Defines a reference type that supports inheritance and polymorphism.

```csharp
public class InvoiceService
{
    public void Submit() => Console.WriteLine("Submitted");
}
```

`struct` — Declares a value type that lives on the stack by default and avoids heap allocations.

```csharp
public struct Money(decimal amount, string currency)
{
    public decimal Amount { get; } = amount;
    public string Currency { get; } = currency;
}
```

`interface` — Specifies a contract that multiple types can implement.

```csharp
public interface IRepository<T>
{
    Task<T> GetAsync(Guid id);
}
```

`enum` — Lists a set of named integral constants.

```csharp
public enum OrderStatus
{
    Draft,
    Submitted,
    Approved
}
```

`delegate` — Describes a function signature that can be stored and invoked later.

```csharp
public delegate void MetricsCaptured(ReadOnlySpan<double> samples);
```

`event` — Wraps a delegate with add/remove semantics for publisher–subscriber patterns.

```csharp
public class EventStream
{
    public event EventHandler? MessageReceived;
    public void Raise() => MessageReceived?.Invoke(this, EventArgs.Empty);
}
```

`module` — Targets metadata at the module level, typically for interop or compliance attributes.citeturn3view0

```csharp
[module: CLSCompliant(false)]
public static class NativeExports { }
```

`abstract` — Marks a class or member that cannot be instantiated directly and must be overridden.

```csharp
public abstract class ReportRenderer
{
    public abstract string Render(string source);
}
```

`sealed` — Prevents further inheritance from a class.

```csharp
public sealed class PdfReportRenderer : ReportRenderer
{
    public override string Render(string source) => $"<pdf>{source}</pdf>";
}
```

`virtual` — Allows a derived class to override a member.

```csharp
public class RestClient
{
    public virtual HttpRequestMessage BuildRequest() => new(HttpMethod.Get, "/");
}
```

`override` — Provides a specialized implementation for a virtual or abstract member.

```csharp
public class PagingClient : RestClient
{
    public override HttpRequestMessage BuildRequest() =>
        new(HttpMethod.Get, "/?page=next");
}
```

`static` — Declares members (or entire types) that belong to the type itself rather than instances.

```csharp
public static class CurrencyCodes
{
    public const string Usd = "USD";
}
```

`new` — Hides an inherited member or creates an instance.

```csharp
public class LegacyClient : RestClient
{
    public new HttpRequestMessage BuildRequest(Uri endpoint) =>
        new(HttpMethod.Get, endpoint);
}
```

### 1.2 Access modifiers and encapsulation

`public`, `protected`, `internal`, `private` control visibility and accessibility of members across assemblies and inheritance chains.

```csharp
public class ApiClient
{
    private readonly HttpClient _http;
    protected internal Uri BaseAddress { get; init; } = new("https://api.contoso.com");

    internal ApiClient(HttpClient http) => _http = http;
}
```

`protected` combines with `internal` to allow access from derived types and the same assembly.

### 1.3 Object references

`this` — Refers to the current instance inside instance members.

```csharp
public class Settings
{
    public string Theme { get; set; } = "Light";
    public void ApplyDark() => this.Theme = "Dark";
}
```

`base` — Accesses members on the base class from a derived type.

```csharp
public class AuditClient : RestClient
{
    public override HttpRequestMessage BuildRequest()
    {
        var request = base.BuildRequest();
        request.Headers.Add("X-Audit", "true");
        return request;
    }
}
```

### 1.4 Member semantics

`const` — Declares a compile-time constant.

```csharp
public const int MaxRetries = 5;
```

`readonly` — Ensures a field is assigned only in its declaration or constructor.

```csharp
public readonly struct Identifier(Guid value)
{
    public Guid Value { get; } = value;
}
```

`volatile` — Instructs the runtime not to cache a field because multiple threads may mutate it.

```csharp
private volatile bool _isReady;
```

`extern` — Links a method to an external implementation, often in native code.

```csharp
[DllImport("kernel32.dll")]
private static extern bool Beep(uint frequency, uint duration);
```

### 1.5 Built-in types and literals

The following reserved keywords map to CLR primitive types: `bool`, `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `float`, `double`, `decimal`, `string`, `object`, and `void`. They appear as variable declarations or return types.

```csharp
bool enabled = true;
byte mask = 0b_1010_0001;
sbyte delta = -1;
short port = 8080;
ushort length = 512;
int count = 42;
uint inventory = 1_000u;
long population = 8_045_000_000L;
ulong checksum = 0xFFFF_FFFF_FFFF_FFFFUL;
char grade = 'A';
float ratio = 0.75f;
double pi = Math.PI;
decimal price = 19.99m;
string message = "Hello";
object boxed = price;

void Log() => Console.WriteLine(message);
```

`true`, `false`, and `null` are literal keywords for Boolean truth values and the absence of a reference.

```csharp
bool succeeded = false;
string? alias = null;
if (!succeeded)
{
    alias ??= "fallback";
}
```

### 1.6 Casting, operators, and type information

`as` — Performs a safe reference conversion or returns `null`.

```csharp
var customer = repository.Get("42") as Customer;
```

`is` — Tests type compatibility and supports pattern matching.

```csharp
if (entity is Order order && order.Total > 100m)
{
    Console.WriteLine(order.Id);
}
```

`operator`, `implicit`, `explicit` — Define custom conversions or operators.

```csharp
public readonly struct Percentage(decimal value)
{
    private readonly decimal _value = value;

    public static implicit operator decimal(Percentage percent) => percent._value;
    public static explicit operator Percentage(decimal value) => new(value / 100m);
}
```

`typeof` — Gets a `System.Type` at runtime.

```csharp
var type = typeof(InvoiceService);
```

`sizeof` — Returns the unmanaged size of a type (requires `unsafe` for managed types).

```csharp
int bytes = sizeof(int);
```

`checked` / `unchecked` — Control overflow checking for arithmetic operations.

```csharp
int overflow = checked(int.MaxValue - 1 + 2);
int wrapped = unchecked(int.MaxValue + 1);
```

### 1.7 Parameter passing

`ref` — Passes an argument by reference for reading and writing.

```csharp
void Increment(ref int value) => value++;
```

`out` — Returns values via an argument that the callee must assign.

```csharp
bool TryParseAmount(string text, out decimal amount) =>
    decimal.TryParse(text, out amount);
```

`in` — Passes an argument by reference for read-only access.

```csharp
void Print(in ReadOnlySpan<char> name) => Console.WriteLine(name);
```

`params` — Accepts a variable-length list of arguments.

```csharp
decimal Sum(params decimal[] values) => values.Sum();
```

### 1.8 Control flow (branching and looping)

`if` / `else` — Conditional execution.

```csharp
if (count == 0)
{
    Console.WriteLine("Nothing to process");
}
else
{
    Console.WriteLine("Processing started");
}
```

`switch`, `case`, `default` — Pattern-based branching across discrete values.

```csharp
switch (order.Status)
{
    case OrderStatus.Draft:
        Console.WriteLine("Still editing");
        break;
    case OrderStatus.Submitted:
        Console.WriteLine("Awaiting approval");
        break;
    default:
        Console.WriteLine("Completed");
        break;
}
```

`for` — Iterates with explicit initialiser, condition, and iterator.

```csharp
for (int i = 0; i < 3; i++)
{
    Console.WriteLine(i);
}
```

`foreach` — Enumerates `IEnumerable` sequences.

```csharp
foreach (var item in Enumerable.Range(0, 3))
{
    Console.WriteLine(item);
}
```

`while` / `do` — Loop while a condition holds, with `do` running at least once.

```csharp
int attempts = 0;
do
{
    attempts++;
} while (attempts < 3);
```

`break` — Exits the nearest loop or switch.

```csharp
foreach (var order in orders)
{
    if (order.IsHighRisk)
    {
        break;
    }
}
```

`continue` — Skips to the next loop iteration.

```csharp
foreach (var order in orders)
{
    if (order.IsCancelled) continue;
    Console.WriteLine(order.Id);
}
```

`goto` — Jumps to a labeled statement (use sparingly).

```csharp
int value = 2;
switch (value)
{
    case 2:
        goto case 4;
    case 4:
        Console.WriteLine("Reached 4");
        break;
}
```

`return` — Exits a method and optionally yields a value.

```csharp
int Add(int left, int right) => left + right;
```

`yield` — Produces items lazily from an iterator method.

```csharp
IEnumerable<int> Even()
{
    for (int i = 0; i < 6; i++)
    {
        if (i % 2 == 0) yield return i;
    }
}
```

### 1.9 Exception handling

`try`, `catch`, `finally`, `throw` manage exceptional control flows.

```csharp
try
{
    await repository.SaveAsync(entity);
}
catch (HttpRequestException ex)
{
    throw new InvalidOperationException("Save failed", ex);
}
finally
{
    Console.WriteLine("Attempt complete");
}
```

### 1.10 Memory, interop, and threading

`unsafe` — Enables pointer arithmetic and other low-level operations.

```csharp
unsafe Span<int> MakeBuffer()
{
    int* buffer = stackalloc int[4];
    return new Span<int>(buffer, 4);
}
```

`fixed` — Pins memory to prevent the GC from moving it.

```csharp
unsafe void Copy(int[] source, int[] destination)
{
    fixed (int* src = source, dest = destination)
    {
        Buffer.MemoryCopy(src, dest, destination.Length * sizeof(int), source.Length * sizeof(int));
    }
}
```

`stackalloc` — Allocates memory on the stack for performance-critical scenarios.

```csharp
Span<byte> bytes = stackalloc byte[8];
```

`lock` — Serialises access to a critical section.

```csharp
private readonly object _sync = new();
void Store(string key, object value)
{
    lock (_sync)
    {
        cache[key] = value;
    }
}
```

### 1.11 Namespaces and imports

`namespace` — Defines a logical grouping of types.

```csharp
namespace Contoso.Services.Billing;
```

`using` — Imports namespaces or disposes resources with `IDisposable`.

```csharp
using Contoso.Services.Billing;

using var scope = provider.CreateScope();
```

---

## 2. Contextual keywords (context-sensitive identifiers)

Contextual keywords act as keywords only in specific locations; otherwise, you can use them as identifiers.citeturn3view0

### 2.1 Async and concurrency

`async` / `await` — Mark asynchronous methods and suspend execution until awaited operations complete (introduced in C# 5).citeturn3view0

```csharp
public async Task<int> LoadCountAsync()
{
    await Task.Delay(100);
    return await repository.CountAsync();
}
```

`when` — Adds guard clauses to pattern matching and exception filters.

```csharp
catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.TooManyRequests)
{
    logger.LogWarning("Throttled");
}
```

`scoped` — Limits the lifetime of references passed into members (C# 11).citeturn3view0

```csharp
ref struct Buffer
{
    public void Fill(scoped Span<byte> target) => target.Clear();
}
```

`allows` — Specifies function pointer annotations such as `SuppressGCTransition` (C# 11).citeturn8view0

`managed`, `unmanaged` — Define function pointer calling conventions or constrain generics to unmanaged types.citeturn8view0

### 2.2 Records and object initialisation

`record`, `with`, `required`, `init`, `data` (deprecated) describe immutable data carriers (C# 9–11).citeturn6view0

```csharp
public record Customer(Guid Id)
{
    public required string Name { get; init; }
}

var updated = existing with { Name = "Updated" };
```

`field` — Provides direct access to the compiler-generated backing field inside property accessors (previewed for C# 14).citeturn5view0

```csharp
public class Message
{
    public string Title
    {
        get;
        set => field = value?.Trim() ?? "Untitled";
    }
}
```

`extension` — Introduces extension members inside `extension` blocks (C# 14 preview).citeturn5view0

```csharp
public static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        public bool IsEmpty => !source.Any();
    }
}
```

### 2.3 Partial types and members

`partial` (type) and `partial` (member) allow splitting declarations across files.citeturn3view0

```csharp
public partial class InvoiceService
{
    partial void OnInvoiceCreated(Invoice invoice);
}

public partial class InvoiceService
{
    partial void OnInvoiceCreated(Invoice invoice) => Console.WriteLine(invoice.Id);
}
```

`file` — Limits a type to the current source file, avoiding accidental exports.

```csharp
file class Validators
{
    public static bool IsEmail(string text) => text.Contains("@", StringComparison.Ordinal);
}
```

### 2.4 Properties, events, and lambdas

`get`, `set`, `init`, `value`, `add`, `remove` define accessors and event wiring.

```csharp
public class Settings
{
    public string Theme { get; set; } = "Light";
    public event EventHandler? Updated
    {
        add => _updated += value;
        remove => _updated -= value;
    }
    private event EventHandler? _updated;
}
```

`delegate` (anonymous), `var`, `dynamic`, `lambda` keywords enable flexible typing.

```csharp
Func<int, int> square = static value => value * value;
dynamic context = new System.Dynamic.ExpandoObject();
```

### 2.5 Pattern matching

`and`, `or`, `not` combine patterns, while `is` (contextual when used in patterns) and `when` refine matches (C# 9–11).citeturn3view0

```csharp
if (shape is Rectangle { Width: > 0 } and { Height: > 0 } rect &&
    rect.Width is not 0 and not double.NaN)
{
    Console.WriteLine("Valid rectangle");
}
```

### 2.6 Query expressions (LINQ)

`from`, `where`, `select`, `group`, `into`, `orderby`, `ascending`, `descending`, `join`, `on`, `equals`, `by`, `let` mirror SQL-like pipelines.

```csharp
var totals =
    from order in orders
    join customer in customers on order.CustomerId equals customer.Id
    where order.Total > 100m
    let discounted = order.Total * 0.9m
    orderby order.Created descending, order.Id ascending
    group order by customer.Region into regionGroup
    select new { Region = regionGroup.Key, Total = regionGroup.Sum(o => o.Total), Discounted = discounted };
```

### 2.7 Namespaces and aliases

`global` — References the root namespace; `alias` (in `using alias = ...`) creates shortcuts to types or namespaces.

```csharp
using Json = global::System.Text.Json.JsonSerializer;
```

### 2.8 Miscellaneous context keywords

- `nameof` returns the simple name of a symbol.

```csharp
Console.WriteLine(nameof(InvoiceService.Submit));
```

- `var` enables implicit typing.

```csharp
var builder = new StringBuilder();
```

- `value` inside a property setter references the incoming value.
- `nint` / `nuint` represent native-sized integers (C# 9).citeturn3view0

```csharp
nint pointer = nint.Zero;
nuint length = 1024;
```

- `args` exposes command-line arguments in top-level programs.

```csharp
Console.WriteLine($"Arg count: {args.Length}");
```

- `required` ensures object initialisers set critical members (C# 11).citeturn6view0

- `unmanaged`, `notnull`, `where` (constraint) add generic constraints.citeturn3view0

```csharp
public interface IBuffer<T> where T : unmanaged, notnull
{
    void Write(T value);
}
```

- `select`, `await`, `yield`, `async`, `var`, `dynamic` remain contextual because they can still be used as identifiers outside their grammatical positions.

---

## 3. Upgrade checklist

1. **Stay current on compiler tooling.** Visual Studio and the .NET SDK must support the newest contextual keywords—especially preview features like `extension` and `field`—before you adopt them in shared code.citeturn5view0
2. **Document language version requirements.** Annotate project files with `<LangVersion>` so newer keywords (e.g., `required`, `scoped`) do not break older CI agents.
3. **Run analyzers after upgrades.** Roslyn analyzers flag obsolete patterns (`nameof`, pattern matching keywords) and ensure team-wide consistency.

---

## 4. Further reading

- Microsoft Learn: [C# keywords](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/)citeturn3view0
- Microsoft Learn: [Function pointers](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/function-pointers)citeturn8view0
- Microsoft Learn: [Required members](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required)citeturn6view0
- Microsoft Learn: [C# 14 preview features](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14)citeturn5view0









