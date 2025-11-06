---
title: C# Language Evolution (1.0 → 14.0)
description: Explore major language features from C# 1.0 through C# 14 with concise examples.
last_reviewed: 2025-10-29
owners:
  - @prodyum/language-guild
---

# C# Language Evolution (1.0 → 14.0)

Since 2002, C# has grown from a simple object-oriented language into a modern toolchain for cloud, mobile, and AI. This guide lists major features introduced in each release from C# 1.0 to C# 14 (preview), with short explanations and code snippets.

---

## 1. Timeline at a glance

| Version | Year | Highlights |
| --- | --- | --- |
| C# 1.0 | 2002 | Classes, interfaces, structs, events, delegates.citeturn0search12 |
| C# 2.0 | 2005 | Generics, nullable types, iterators, anonymous methods. |
| C# 3.0 | 2007 | LINQ, lambda expressions, extension methods, auto-properties, object/collection initializers, anonymous types. |
| C# 4.0 | 2010 | Dynamic binding, named/optional arguments, COM interop enhancements. |
| C# 5.0 | 2012 | `async`/`await`. |
| C# 6.0 | 2015 | String interpolation, expression-bodied members, null-conditional operator. |
| C# 7.0/7.1/7.2/7.3 | 2017–2018 | Tuples, pattern matching, local functions, ref structs. |
| C# 8.0 | 2019 | Nullable reference types, switch expressions, async streams, ranges. |
| C# 9.0 | 2020 | Records, init-only setters, top-level statements. |
| C# 10.0 | 2021 | Global usings, file-scoped namespaces, record structs. |
| C# 11.0 | 2022 | Raw string literals, required members, list patterns. |
| C# 12.0 | 2023 | Primary constructors for non-records, collection expressions, interceptors (preview). |
| C# 13.0 | 2024 | Lock enhancements, expansion of params, partial properties (preview).citeturn0search13 |
| C# 14.0 | 2025 | Inline arrays, static virtual interface members improvements (preview).citeturn0search14 |

---

## 2. Feature highlights with snippets

### C# 1.0 – Delegates & events

```csharp
public delegate void Notify(string message);

public class Publisher
{
    public event Notify? OnNotify;
    public void Raise() => OnNotify?.Invoke("Hello C# 1.0");
}
```

### C# 2.0 – Generics & nullable types

```csharp
List<int?> scores = new() { 42, null, 99 };
```

### C# 3.0 – LINQ & lambdas

```csharp
var evens = scores.Where(s => s.HasValue && s % 2 == 0);
```

### C# 4.0 – Dynamic

```csharp
dynamic bag = new ExpandoObject();
bag.Name = "Ada";
Console.WriteLine(bag.Name);
```

### C# 5.0 – async/await

```csharp
public async Task<string> GetAsync(Uri uri)
{
    using var client = new HttpClient();
    return await client.GetStringAsync(uri);
}
```

### C# 6.0 – String interpolation & null-conditional

```csharp
var name = person?.Name;
Console.WriteLine($"Hello {name ?? "guest"}");
```

### C# 7.x – Tuples & pattern matching

```csharp
var result = (Sum: 5, Difference: 1);
if (result is (5, _))
{
    Console.WriteLine("Tuple pattern matched!");
}
```

### C# 8.0 – Nullable reference types

```csharp
string? maybe = GetValue();
if (maybe is not null)
{
    Console.WriteLine(maybe.Length);
}
```

### C# 9.0 – Records

```csharp
public record Person(string FirstName, string LastName);
```

### C# 10.0 – Global usings & file-scoped namespaces

```csharp
global using System.Net.Http;

namespace Contoso.App;
```

### C# 11.0 – Raw string literals

```csharp
var json = """
{
  "name": "Ada",
  "city": "London"
}
""";
```

### C# 12.0 – Primary constructors for classes

```csharp
public class Order(string id)
{
    public string Id { get; } = id;
}
```

### C# 13.0 – Lock enhancements (preview)

```csharp
lock (resource, LockRecursionPolicy.SupportsRecursion)
{
    // ...
}
```

### C# 14.0 – Inline arrays (preview)

```csharp
public struct Matrix
{
    private InlineArray<9, double> _values;
}
```

---

## 3. Adoption tips

- Use `langversion` in `csproj` to opt into previews:

```xml
<PropertyGroup>
  <LangVersion>preview</LangVersion>
</PropertyGroup>
```

- Follow C# language design notes on GitHub for upcoming proposals.
- When enabling nullable reference types (C# 8), set `nullable enable` and update code accordingly.

---

## 4. Checklist for teams

- [ ] Track C# version requirements per project (.NET SDK ties to language versions).
- [ ] Document which features are in use to onboard new developers.
- [ ] Run analyzers (`dotnet format analyzers`) to enforce modern patterns.
- [ ] Review preview features before adopting in production.

---

## Further reading

- Official C# feature timeline.citeturn0search12
- .NET 9 / C# 13 preview announcements.citeturn0search13
- C# 14 incubations and design notes.citeturn0search14
