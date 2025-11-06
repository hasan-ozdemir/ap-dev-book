---
title: Modern C# 13 Features
description: Adopt the newest language capabilities introduced alongside .NET 9.
last_reviewed: 2025-10-29
owners:
  - @prodyum/language-guild
---

# Modern C# 13 Features

C# 13 shipped with .NET 9 and brings quality-of-life improvements that streamline our daily development. These features increase expressiveness and reduce boilerplate in MAUI apps, backend services, and shared libraries.citeturn4view0

## 1. Params collections

```csharp
public static string JoinLabels(params ReadOnlySpan<string> labels)
{
    return string.Join(" • ", labels.ToArray());
}

var result = JoinLabels(["DevOps", "Azure", "MAUI"]);
```

- Accept a `ReadOnlySpan<T>` while still calling with array, list, or collection expressions.
- Eliminates defensive copying when building string interpolations. 

## 2. Partial properties

```csharp
public partial class EnvironmentSettings
{
    public partial string ApiBaseUrl { get; }

    partial void OnApiBaseUrlChanging(ref string value);
}

public partial class EnvironmentSettings
{
    public partial string ApiBaseUrl
    {
        get => _apiBaseUrl;
        set
        {
            OnApiBaseUrlChanging(ref value);
            _apiBaseUrl = value;
        }
    }

    private string _apiBaseUrl = "https://localhost";

    partial void OnApiBaseUrlChanging(ref string value)
    {
        if (!value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Only HTTPS endpoints are supported.");
        }
    }
}
```

- Split property logic across partial class files—useful when source generators or MAUI XAML create partials you want to extend safely. 

## 3. Lock statement enhancements

```csharp
var sync = new object();

lock (sync)
{
    // Implicit ref-scoped locking prevents accidentally escaping the lock object.
    Console.WriteLine("Safe critical section.");
}
```

- `lock` now accepts `ref` and `ref readonly` variables, reducing accidental copying. 

## 4. Collection expressions improvements

```csharp
string[] channels =
[
    "Internal",
    ..GetBetaChannelNames(),
    "Production"
];
```

- Spread operator (`..`) works across more collection types, simplifying list construction. 

## 5. Ref struct interfaces

```csharp
public interface IBufferWriter<T>
{
    void Write(scoped Span<T> target);
}

public ref struct ArrayBufferWriter<T>(Span<T> buffer) : IBufferWriter<T>
{
    public void Write(scoped Span<T> target) => target.CopyTo(buffer);
}
```

- `ref struct` can implement interfaces, unlocking better abstractions for high-performance scenarios (e.g., serializers). 

---

### Adoption guidance

- Update analyzers to C# 13 once the team is on .NET 9.
- Add examples from this module to the Todo app (e.g., using collection expressions for navigation routes).
- Review code reviews for opportunities to replace verbose patterns with these features.

## Upcoming modern C# 14 features

C# 14 arrives with .NET 10 in late 2025 preview builds and is already feature complete as of Preview 7, giving us a clear runway to experiment before General Availability.citeturn5view0 Use the following highlights to plan lab spikes and update coding standards ahead of the upgrade window.

### Extension members and richer augmentation

The new `extension` blocks let you add instance or static members, operators, and properties to types you do not own. This replaces the earlier "extension types" proposal with syntax that composes with existing static classes:citeturn5view0

```csharp
public static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        public bool IsEmpty => !source.Any();
        public static IEnumerable<T> NonEmpty() => source.Where(item => item is not null);
    }
}

if (orders.IsEmpty) return;
var sanitized = IEnumerable<string>.NonEmpty();
```

Review shared libraries for conflicting identifiers (for example, classes named `extension`) and update analyzers to surface migrations automatically.citeturn5view0

### `field`-backed properties

The `field` contextual keyword now ships as a first-class feature, letting you add validation logic without manually declaring backing storage:citeturn5view0

```csharp
public string Token
{
    get;
    set => field = value?.Trim() ?? throw new ArgumentNullException(nameof(value));
}
```

### Everyday productivity boosts

- **Lambda parameter modifiers:** Apply `ref`, `out`, or `scoped` directly to simple lambda parameters, pairing well with MAUI command handlers that pass spans or pooled buffers.citeturn5view0turn5view0
- **Span-first overload resolution:** New implicit conversions pick the `Span<T>` overload automatically; audit array covariance code paths and add regression tests around ambiguous overloads.citeturn5view0turn5view0
- **Null-conditional assignment:** Collapse guard clauses by assigning through `?.` or `?[]`, simplifying UI state updates: `viewModel?.Header = string.Empty;`.citeturn5view0

### Adoption checklist

1. Enable the C# preview language version in pilot branches and run the Roslyn feature status board to watch for late-breaking changes.
2. Update ReSharper/Rider or Visual Studio tooling to versions that understand the new syntax before pushing the preview into shared branches.citeturn5view0
3. Schedule targeted code reviews around extension member usage to ensure discoverability and avoid overloading domain models.




