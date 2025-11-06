---
title: Generics & Collections
description: Use generic types, constraints, and advanced collection patterns to build reusable libraries.
last_reviewed: 2025-10-29
owners:
  - @prodyum/language-guild
---

# Generics & Collections

Generics power reusable components across our MAUI apps and Azure services, enabling compile-time safety and performance.citeturn13search7 Learn how constraints, variance, and collection types work together.

## Keywords covered

`generic`, `where`, `new()`, `struct`, `class`, `notnull`, `unmanaged`, `delegate`, `ref struct`, `in`, `out`, `var`

## Example

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetAsync(Guid id, CancellationToken token = default);
    Task SaveAsync(T entity, CancellationToken token = default);
}

public sealed class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly Dictionary<Guid, T> _store = [];

    public Task<T?> GetAsync(Guid id, CancellationToken token = default)
        => Task.FromResult(_store.TryGetValue(id, out var value) ? value : null);

    public Task SaveAsync(T entity, CancellationToken token = default)
    {
        var id = entity.GetType().GetProperty("Id")?.GetValue(entity) as Guid? ?? Guid.NewGuid();
        _store[id] = entity;
        return Task.CompletedTask;
    }
}
```

## Tips

- Use **interface variance** (`in`, `out`) for producers/consumers (e.g., `IComparer<in T>`).citeturn13search7
- `where T : notnull` prevents null assignments, helpful for dictionary keys.citeturn13search7
- Combine `IEnumerable<T>` and `Span<T>` to optimize hot paths while maintaining friendly APIs.citeturn14search1

## Practice

- Implement a `Result<T>` discriminated union using `record struct` and pattern matching.
- Add caching decorators using `Func<T>` delegates to demonstrate higher-order functions.
- Explore `IAsyncEnumerable<T>` to stream paginated API results without loading everything in memory.

Generics are central to clean architecture boundaries--repositories, services, and value objects rely on them heavily.citeturn13search7
