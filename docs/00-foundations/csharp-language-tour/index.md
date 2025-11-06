---
title: C# Language Tour
description: Learn every C# keyword and feature using runnable .NET 9 console snippets.
last_reviewed: 2025-10-29
owners:
  - @prodyum/language-guild
---

# C# Language Tour

The Prodyum playbook expects every engineer to read, write, and review modern C# fluently. This tour breaks the language into digestible modules, each backed by runnable code in [`samples/ConsoleSnippets`](https://github.com/hasan-ozdemir/devyum/tree/main/samples/ConsoleSnippets).citeturn0search0

The same roadmap is openly available so partner teams, clients, and the broader developer community can benchmark themselves against Prodyum expectations and collaborate with us faster.

## How to study

1. Clone the repo and run `dotnet run --project samples/ConsoleSnippets`.
2. Explore the modules below in order. Each keyword includes a short explanation, gotchas, and a minimal example you can adapt.
3. Use the "Practice" prompts to build muscle memory (e.g., extend the snippet, add tests).

> **Tip:** Run `dotnet watch --project samples/ConsoleSnippets/ConsoleSnippets.csproj run` while editing snippets to see changes instantly.

## Modules

- [Program structure & entry point](./modules/program-structure.md)
- [Types & memory](./modules/types-and-memory.md)
- [Control flow](./modules/control-flow.md)
- [Object-oriented building blocks](./modules/oop-basics.md)
- [Language members & contracts](./modules/language-members.md)
- [Generics & collections](./modules/generics.md)
- [Reflection & runtime discovery](./modules/reflection.md)
- [LINQ fundamentals](./modules/linq.md)
- [Asynchrony & concurrency](./modules/async.md)
- [Native interop & unsafe code](./modules/unsafe.md)
- [Modern C# 13 additions](./modules/csharp-13.md)
- [Upcoming modern C# 14 features](./modules/csharp-14-preview.md)
- [C# Keyword Compendium](./modules/keyword-compendium.md)
- [C# Programming Concepts Atlas](./modules/concept-compendium.md)
- [Design patterns in practice](./modules/design-patterns.md)
- [Software principles & clean code](./modules/software-principles.md)
- [Refactoring playbooks](./modules/refactoring-playbooks.md)

Each module documents every keyword relevant to the topic (for example, `class`, `struct`, `record`, `partial`, `ref`, `readonly`). We track coverage in the [Language Completion Checklist](./modules/completion-checklist.md).

## Why C# 13?

C# 13, released alongside .NET 9, introduces `params` spans and collections, partial properties, refined `lock` behaviour, new string escape sequences, and support for `ref struct` interfaces, among other quality-of-life improvements.citeturn0search2

Keeping pace with these features lets Prodyum teams write safer, more expressive code and stay aligned with the broader .NET ecosystem's expectations for senior engineers.citeturn0search2

---

Ready to start? Jump into [Program structure & entry point](./modules/program-structure.md).


