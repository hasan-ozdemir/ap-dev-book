---
title: Program Structure & Entry Point
description: Understand top-level statements, namespaces, and the anatomy of a .NET 9 console app.
last_reviewed: 2025-10-29
owners:
  - @prodyum/language-guild
---

# Program Structure & Entry Point

Modern C# projects often use **top-level statements**, making small utilities and demos easier to read while the compiler synthesises the underlying entry point.citeturn13search0turn13search1

## Keywords covered

`namespace`, `using`, `class`, `static`, `void`, `async`, `await`, `return`, `Main`

## Example

```csharp
// File: Program.cs (samples/ConsoleSnippets)
using System.Threading.Tasks;

Console.WriteLine("Prodyum Console bootstrapped.");

await RunAsync();

static async Task RunAsync()
{
    await Task.Delay(100);
    Console.WriteLine("Top-level statements call static local methods.");
}
```

### Inside the compiler

- The compiler generates an internal `Program` class with a `Main` method behind every top-level file.citeturn13search0
- `await` at the top level translates into an async `Main` returning `Task`.citeturn13search0
- Additional `static` methods inside the file remain local functions that the compiler hoists into the generated class.citeturn13search1

## Practice

- Convert the snippet into an explicit `Program` class and compare IL (`dotnet build /bl`).
- Add command-line arguments by reading `Environment.GetCommandLineArgs()`.
- Create a separate file with a `namespace Prodyum.Tools;` and import it here.

Understanding the structure makes it easier to read generated code in MAUI projects (e.g., `App.xaml.cs`) where partial classes split responsibilities.
