---
title: Reflection & Runtime Discovery
description: Inspect assemblies, types, and members at runtime and activate components dynamically with .NET 9.
last_reviewed: 2025-11-03
owners:
  - @prodyum/language-guild
---

# Reflection & Runtime Discovery

Reflection lets you explore assemblies, types, and members at runtime so that you can build plug-in systems, diagnostics, and tooling without compile-time coupling.citeturn0search0 The `System.Reflection` APIs expose assemblies, modules, types, and members as objects you can query, while `Type` remains the central entry point for probing metadata.citeturn0search0turn0search2

## Keywords & APIs covered

`typeof`, `Assembly`, `Module`, `Type`, `MemberInfo`, `MethodInfo`, `PropertyInfo`, `FieldInfo`, `ConstructorInfo`, `Activator`, `BindingFlags`

## Quick start

```csharp
// File: samples/ConsoleSnippets/ReflectionShowcase.cs
using System;
using System.Reflection;

namespace ConsoleSnippets;

public static class ReflectionShowcase
{
    public static void DumpCurrentAssembly()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Console.WriteLine($"Assembly: {assembly.FullName}");

        foreach (Type type in assembly.GetExportedTypes())
        {
            Console.WriteLine($"  Type: {type.FullName}");

            foreach (MethodInfo method in type.GetMethods(
                         BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                Console.WriteLine($"    Method: {method.Name} returns {method.ReturnType.Name}");
            }
        }
    }

    public static object CreatePlugin(string typeName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type? pluginType = assembly.GetType(typeName, throwOnError: true);
        return Activator.CreateInstance(pluginType!);
    }
}
```

- `Assembly.GetExecutingAssembly()` returns the currently loaded assembly so you can iterate over public types.citeturn0search0turn0search2
- `BindingFlags` filters members to avoid inherited duplicates and surface only the metadata you care about.citeturn0search2
- `Activator.CreateInstance` lets you instantiate plug-ins dynamically once you locate the type by name.citeturn0search0

> **Try it:** Add a `Main` method that calls `DumpCurrentAssembly()` and register a simple plugin class. Run `dotnet run --project samples/ConsoleSnippets`.

## Common scenarios

| Scenario | Reflection recipe |
| --- | --- |
| Build a plug-in loader | Use `AssemblyLoadContext` to load assemblies, scan for a marker interface via `Type.GetInterfaces()`, and instantiate matches with `Activator.CreateInstance`.citeturn0search0 |
| Generate diagnostics tooling | Combine `Type.GetMembers()` with `MemberInfo.CustomAttributes` to list required annotations or deprecated APIs.citeturn0search0turn0search2 |
| Explore metadata in scripting | Use `typeof(T).GetMethods()` inside REPL or scripting hosts to surface available operations dynamically.citeturn0search2 |

## Performance & safety tips

- Cache reflection results (e.g., `MethodInfo`) instead of resolving them repeatedly to avoid repeated metadata traversal.citeturn0search0
- Prefer `Type.GetMethod`/`GetProperty` with explicit `BindingFlags` and parameter arrays to avoid ambiguous matches.citeturn0search2
- When you must emit code, pivot to `System.Reflection.Emit` or source generators for repeatable performance-sensitive pipelines.citeturn0search0

## Practice

1. Add a custom attribute to a plug-in class and extend the sample to list every attribute applied to discovered types.
2. Load another assembly via `Assembly.LoadFile` and enumerate all types that inherit from your shared `IPlugin` contract.
3. Write a helper that creates a factory function (`Func<object>`) by caching a resolved constructor.

## Further reading

- [Reflection in .NET](https://learn.microsoft.com/dotnet/fundamentals/reflection/reflection)citeturn0search0
- [Viewing type information](https://learn.microsoft.com/dotnet/fundamentals/reflection/viewing-type-information)citeturn0search2
