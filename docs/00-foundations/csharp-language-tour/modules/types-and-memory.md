---
title: Types & Memory
description: Master value vs reference types, records, ref structs, and span-friendly memory patterns.
last_reviewed: 2025-10-29
owners:
  - @prodyum/language-guild
---

# Types & Memory

Prodyum engineers regularly optimise memory for mobile apps and cloud services. Knowing how types map to the CLR is essential.

## Keywords covered

`struct`, `record`, `class`, `readonly`, `ref`, `scoped`, `in`, `out`, `unsafe`

## Example

```csharp
// Value type with readonly members for stack allocation.
public readonly struct Coordinates(double latitude, double longitude)
{
    public double Latitude { get; } = latitude;
    public double Longitude { get; } = longitude;

    public double DistanceTo(Coordinates other)
        => Math.Sqrt(Math.Pow(other.Latitude - Latitude, 2) + Math.Pow(other.Longitude - Longitude, 2));
}

// Using scoped span to prevent heap escape.
scoped Span<int> samples = stackalloc int[] { 1, 2, 3 };
ref readonly var last = ref samples[^1];
Console.WriteLine($"Last sample: {last}");
```

## Key takeaways

- `struct` types generally allocate on the stack and are copied by value, making them suitable for small, immutable data.citeturn11search3
- `readonly struct` keeps fields immutable so the JIT can elide defensive copies when passing by reference.citeturn11search3turn11search1
- `record` and `record struct` supply generated value-based equality and with-expression support that simplify DTOs.citeturn11search4
- `ref`, `in`, and `out` parameters enable by-reference access to avoid redundant copies in performance-critical code paths.citeturn11search2
- `scoped` (C# 12) blocks span-backed references from escaping the current stack frame, protecting `Span<T>` usage.citeturn11search1

## Practice

- Implement a `record` for immutable API responses and convert it to `record struct` to observe differences.
- Time allocation differences between `List<int>` and `Span<int>` operations using `BenchmarkDotNet`.
- Explore `ref struct` constraints by creating a type that wraps `Span<T>`.

These patterns show up throughout the MAUI Todo sample (view models use records and spans to format offline sync data efficiently).
