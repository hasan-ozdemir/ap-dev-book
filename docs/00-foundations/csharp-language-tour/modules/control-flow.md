---
title: Control Flow
description: Use pattern matching, switch expressions, and advanced loops to express business rules succinctly.
last_reviewed: 2025-10-29
owners:
  - @prodyum/language-guild
---

# Control Flow

Modern C# emphasises expressive pattern matching and functional-style expressions, letting you collapse nested branch logic into concise, readable constructs.citeturn12search1turn12search2 This module shows how to combine `switch`, relational patterns, and `when` clauses effectively.

## Keywords covered

`if`, `else`, `switch`, `when`, `case`, `goto`, `for`, `foreach`, `while`, `do`, `continue`, `break`, `yield`, `await foreach`

## Example

```csharp
var sku = "premium";
var tier = sku switch
{
    "free" => PricingTier.Free,
    "pro" => PricingTier.Pro,
    "premium" or "enterprise" => PricingTier.Premium,
    _ when sku.StartsWith("beta-", StringComparison.OrdinalIgnoreCase) => PricingTier.Beta,
    _ => throw new ArgumentOutOfRangeException(nameof(sku), sku, "Unknown SKU.")
};

await foreach (var checkpoint in GenerateCheckpoints())
{
    if (checkpoint.Contains("Deploy", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Triggering deployment automation.");
        continue;
    }

    Console.WriteLine($"Checkpoint: {checkpoint}");
}
```

`GenerateCheckpoints` uses `yield return` in the sample project to stream data asynchronously without materialising the entire sequence.citeturn12search0

## Pattern matching tips

- Combine property, list, and relational patterns to simplify nested conditionals.citeturn12search1
- Use `is not null` and `is []` to validate data structures cleanly.citeturn12search1
- Leverage `switch` expressions for concise state machines and UI navigation logic in MAUI.citeturn12search2

## Practice

- Rewrite existing `if/else` chains in the Todo app using switch expressions.
- Implement guard clauses with pattern matching to validate API requests.
- Experiment with `goto case` to share logic between switch branches when maintaining legacy code.

Mastering control flow ensures your MAUI view models and Azure Functions remain readable even with complex business rules.
