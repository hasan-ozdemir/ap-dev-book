---
title: Object-Oriented Building Blocks
description: Apply encapsulation, inheritance, interfaces, and design patterns in modern C#.
last_reviewed: 2025-10-29
owners:
  - @prodyum/language-guild
---

# Object-Oriented Building Blocks

Prodyum uses clean architecture principles, so you must be comfortable modelling domains using C#'s object-oriented features.

## Keywords covered

`class`, `interface`, `abstract`, `sealed`, `virtual`, `override`, `new`, `partial`, `record class`, `init`

## Example

```csharp
public interface ISyncCommand
{
    Task ExecuteAsync(CancellationToken token);
}

public abstract class SyncCommandBase : ISyncCommand
{
    public async Task ExecuteAsync(CancellationToken token)
    {
        await AuditAsync("started", token);
        await ExecuteInternalAsync(token);
        await AuditAsync("completed", token);
    }

    protected abstract Task ExecuteInternalAsync(CancellationToken token);

    private static Task AuditAsync(string state, CancellationToken token)
        => Task.Run(() => Console.WriteLine($"[{DateTimeOffset.UtcNow:u}] {state}"), token);
}

public sealed class PushNotificationSync : SyncCommandBase
{
    protected override Task ExecuteInternalAsync(CancellationToken token)
        => Task.Delay(50, token);
}
```

## Design principles

- Prefer `sealed` classes by default; open for extension only when you have a deliberate inheritance strategy.citeturn3search3
- Use `interface` definitions to decouple MAUI view models from infrastructure implementations and enable flexible dependency injection.citeturn13search3
- `record class` with `init` setters enables immutable configuration while preserving object initialiser syntax.citeturn11search4
- Split large classes via `partial` definitions to isolate generated MAUI code from hand-authored logic.citeturn13search10

## Practice

- Implement an interface-based strategy pattern for syncing tasks (e.g., local storage vs cloud).
- Convert existing POCOs into `record class` types and observe equality semantics.
- Create partial classes to separate generated MAUI code from hand-written logic.

These building blocks underpin clean architecture diagrams in the Architecture section of this portal.
