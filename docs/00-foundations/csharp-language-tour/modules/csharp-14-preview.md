title: Upcoming C# 14 Features
description: Preview features and roadmap items under consideration for C# 14 applicable to .NET 9/10.
last_reviewed: 2025-11-03
owners:
  - @prodyum/language-guild
---

# Upcoming C# 14 Features

C# 14 aligns with the .NET 10 release train scheduled for fall 2025, and Preview 7 marked the language as “feature complete,” so now is the right time to prepare production codebases for the upgrade path.citeturn0search0 This guide distills the preview feature set and shows how Prodyum teams can adopt it safely ahead of general availability.

> **Update note (3 Nov 2025):** Preview builds can still change before RTM. Track the Microsoft Learn “What’s new in C# 14” article and official release notes for the final surface area.citeturn0search2

## 1. Language innovations at a glance

| Feature | Why it matters | Typical use cases |
|---------|----------------|-------------------|
| **Extension members** | Extend types you do not own with static members, properties, and operators for more expressive APIs.citeturn0search2turn0search6 | Enrich domain models without modifying sealed or third-party types. |
| **Field-backed properties (`field`)** | Inject validation or change tracking into auto-properties without adding manual backing fields.citeturn0search2turn1search0 | DTOs, view models, and MAUI `BindableObject` classes that raise `PropertyChanged`. |
| **Lambda parameter modifiers** | Apply `ref`, `in`, `out`, or `scoped` directly to implicit lambda parameters to avoid defensive copies.citeturn0search2 | High-performance pipelines that process `Span<T>` buffers. |
| **Implicit `Span<T>` conversions** | Improve interoperability between `Span<T>`, `ReadOnlySpan<T>`, and arrays with fewer casts or copies.citeturn0search2 | Parsing, serialization, and text handling in MAUI and ASP.NET back ends. |
| **`nameof` for unbound generics** | Reference generic type definitions without choosing type arguments.citeturn0search2 | Logging, source generators, and diagnostics that need stable identifiers. |
| **Partial events and constructors** | Split event and constructor logic across generated and hand-written partial classes.citeturn0search2 | Incremental generators and UI scaffolding tools. |
| **User-defined compound assignments** | Customize operators like `+=` or `-=` for your types while preserving type safety.citeturn0search2turn1search9 | Financial, vector, or domain value objects that encapsulate business rules. |

## 2. Hands-on code samples

### 2.1 Field-backed property

```csharp
public class Invoice
{
    public decimal Total { get; private set; }

    public decimal Discount
    {
        get;
        set => field = value switch
        {
            < 0m => throw new ArgumentOutOfRangeException(nameof(value)),
            > 0.5m => 0.5m,
            _ => value
        };
    }

    public void ApplyDiscount() => Total -= Total * Discount;
}
```

The `field` keyword keeps the compiler-generated backing storage while letting you add validation logic, eliminating the need for an explicit `_discount` member.citeturn1search0turn1search3

### 2.2 Extension member

```csharp
public static extension class MoneyExtensions for decimal
{
    public static decimal ToCurrency(this decimal value, string culture = "en-US") =>
        string.Create(CultureInfo.GetCultureInfo(culture), $"{value:C}");

    public static decimal MaxBudget => 1_000_000m;
}
```

Extension classes can expose static members (such as `MaxBudget`) alongside instance-style helpers, enabling richer fluent APIs without touching the original type.citeturn0search6

### 2.3 Lambda parameter modifiers

```csharp
Func<ReadOnlySpan<byte>, Span<byte>, bool> copy =
    (in source, ref destination) =>
    {
        source.CopyTo(destination);
        destination = destination[source.Length..];
        return true;
    };
```

Applying modifiers directly to lambda parameters keeps hot paths zero-copy without introducing boilerplate locals or wrapper methods.citeturn0search2

## 3. Adoption checklist

- **Project configuration:** C# 14 ships with the .NET 10 SDK; enable the preview today by setting `<LangVersion>preview</LangVersion>` in project files or pinning the SDK via `global.json`.citeturn1search0turn1search3
- **Tooling support:** Visual Studio 2022 17.14 and newer provide full syntax services for the preview, including IntelliSense and analyzer coverage.citeturn0search3turn0search4
- **Style guidance:** Update `.editorconfig` so reviewers agree where new keywords (for example `field`) should appear.

## 4. Risk watchlist

- The `field` contextual keyword shadows members literally named `field`; escape legacy members with `@field` or rename them during migration.citeturn1search1turn1search3
- Extension members can grow unwieldy—prefer small, domain-focused helpers and document the assemblies that own them.
- Guard preview-only functionality behind compiler constants (for example `#if FEATURE_CSHARP14`) until .NET 10 ships so long-term support branches remain stable.citeturn0search0

Use this module as your launchpad for pilot projects, and keep monitoring the official previews so you can adjust patterns as the language reaches RTM.citeturn0search2
