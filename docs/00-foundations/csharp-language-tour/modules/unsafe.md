---
title: Native Interop & Unsafe Code
description: Use pointers, stack allocation, and interop primitives safely when you need maximum performance in .NET 9.
last_reviewed: 2025-11-03
owners:
  - @prodyum/language-guild
---

# Native Interop & Unsafe Code

The `unsafe` context unlocks pointer arithmetic, stack allocation, and interop features that bypass some of C#'s safety rules for scenarios that require raw memory access or native API integration.citeturn1search0turn1search1 Use it sparingly: errant pointers can corrupt memory, so keep unsafe regions small, audited, and surrounded by safe abstractions.citeturn1search0turn1search4

## Keywords & APIs covered

`unsafe`, `fixed`, `stackalloc`, `Span<T>`, `ReadOnlySpan<T>`, `nint`, `nuint`, `void*`, `Marshal`

## Quick start

```csharp
// File: samples/ConsoleSnippets/UnsafeShowcase.cs
using System;
using System.Runtime.InteropServices;

namespace ConsoleSnippets;

public static class UnsafeShowcase
{
    public static unsafe int SumBytes(ReadOnlySpan<byte> bytes)
    {
        fixed (byte* ptr = bytes)
        {
            int total = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                total += *(ptr + i);
            }

            return total;
        }
    }

    public static unsafe void FillWithPattern(Span<byte> buffer, byte pattern)
    {
        if (buffer.Length == 0)
        {
            return;
        }

        fixed (byte* ptr = buffer)
        {
            byte* current = ptr;
            byte* end = ptr + buffer.Length;
            while (current < end)
            {
                *current++ = pattern;
            }
        }
    }

    public static unsafe Span<byte> AllocateOnStack(int length)
        => new Span<byte>(stackalloc byte[length]);

    public static unsafe string GetFrameworkDescription()
        => Marshal.PtrToStringUTF8(RuntimeInformation.FrameworkDescription) ?? ".NET";
}
```

- `fixed` pins managed memory long enough to obtain a stable pointer, preventing the GC from relocating the buffer mid-operation.citeturn1search1
- `stackalloc` allocates temporary buffers on the stack, useful for short-lived parsing or encoding tasks.citeturn1search1
- Native handles returned from the runtime (for example, `RuntimeInformation.FrameworkDescription`) often require conversion helpers such as `Marshal.PtrToStringUTF8`.citeturn1search1

> **Try it:** Compare `SumBytes` against `bytes.Sum(b => b)` for large arrays—switch to intrinsics if you need SIMD acceleration.

## Safety checklist

| Risk | Mitigation |
| --- | --- |
| Pointer arithmetic overruns | Validate lengths before entering unsafe blocks; prefer index bounds checks in debug builds.citeturn1search4 |
| GC relocation causing crashes | Use `fixed` or `GCHandle` to pin objects when calling native APIs.citeturn1search1 |
| Buffer lifetime issues | Keep `stackalloc` buffers within method scope and copy data out before returning spans.citeturn1search1 |

## When to use unsafe code

- Interop with legacy C APIs requiring pointer parameters or structs laid out sequentially.
- High-performance serialization, compression, or graphics pipelines where you manipulate bytes directly.
- Implementing performance-critical algorithms that benefit from vectorization or manual memory layout.

Always benchmark first—`Span<T>` and `MemoryMarshal` often provide safe alternatives without dropping into unsafe code.citeturn1search1

## Practice

1. Wrap `FillWithPattern` in a safe API that takes a `Span<byte>` and add unit tests that compare results with `Array.Fill`.
2. Add a method that uses `stackalloc` for parsing ASCII digits into integers, then upgrade it to `Span<byte>` for safety.
3. Explore `System.Runtime.CompilerServices.Unsafe` for advanced scenarios like reading generic values by reference.

## Further reading

- [Unsafe code, pointer types, and function pointers](https://learn.microsoft.com/dotnet/csharp/language-reference/unsafe-code)citeturn1search1
- [When to use unsafe code](https://learn.microsoft.com/dotnet/standard/managed-code)citeturn1search4
