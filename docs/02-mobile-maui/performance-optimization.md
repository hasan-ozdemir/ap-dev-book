---
title: Performance Optimization
description: Tune .NET MAUI apps on .NET 9 for fast startup, smooth UI, and efficient resource usage.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

# Performance Optimization

.NET MAUI on .NET 9 ships with faster startup defaults, smarter trimming, and updated tooling, but sustained performance still depends on disciplined measurement and platform-aware tuning. Use this guide to set baselines, apply optimizations, and keep regressions out of production builds.

---

## 1. Understand startup & trimming in .NET 9

- Android release builds on .NET 9 enable LLVM-marshalled methods by default, delivering roughly a 15% startup improvement for non-Blazor apps in Preview 5—regenerate your profile after material feature work so the hot paths stay compiled.[^preview5]
- CollectionView and CarouselView now ship with a rewritten iOS/Mac Catalyst implementation alongside Native AOT and trimming improvements, reducing package size and startup time when Release builds are configured correctly.[^maui9]
- The linker trims unused code more aggressively; declare dynamic entry points (handlers, reflection) in a `TrimmerRootDescriptor` so your release builds stay fast without missing types.[^linking]

---

## 2. Measure first, optimize continuously

- Capture startup and UI traces using the Visual Studio performance profiler presets for .NET MAUI; compare Debug vs. Release to catch misconfigured build flags.[^optimize]
- Automate counter captures with `dotnet-counters monitor --counters System.Runtime[cpu-usage,working-set] --refresh-interval 1 <app-id>` to track CPU and GC pressure during CI device runs.[^dotnetcounters]
- Log first meaningful paint from `AppShell` using `Stopwatch` and ship telemetry so you can keep production baselines alongside lab measurements.[^optimize]

---

## 3. Memory & GC hygiene

- Reuse view models and services through dependency injection rather than recreating them on navigation; suppressing repeated allocations helps Gen0 survival stay under 10%.[^optimize]
- Prefer `ObservableCollection<T>` with incremental loading or `IAsyncEnumerable<T>` feeds over building giant lists in memory; batch updates when binding to `CollectionView`.[^journey]
- Dispose images, media streams, and platform handles promptly—wrap handlers in `using` or implement `IAsyncDisposable` when bridging native SDKs.[^optimize]

---

## 4. UI rendering & layout efficiency

- Keep visual trees shallow: replace nested `StackLayout` containers with `Grid` definitions, and push heavy composition to `DrawingView`/`GraphicsView` when you need custom rendering.[^journey]
- Enable virtualization via `CollectionView.ItemsLayout` (e.g., `LinearItemsLayout`), disable animations for data-only refreshes, and defer expensive bindings until controls appear on screen.[^optimize]
- Offload background work from UI events using `Dispatcher.DispatchAsync` or `Task.Run` so layout and animations stay responsive.[^journey]

---

## 5. Platform-specific tuning

| Platform | Optimization | Notes |
| --- | --- | --- |
| Android | Combine startup tracing with profile-guided AOT: `dotnet publish -f net9.0-android -p:AndroidEnableProfiledAot=true -c Release` | Generate a fresh profile after major feature additions so hot paths stay compiled.[^maui9]
| iOS / Mac Catalyst | Pass `--optimize=all` and enable ReadyToRun images for interpreter-disabled builds | Verify native linker warnings after every Xcode toolchain update.[^optimize]
| Windows | Prefer `Win2D` or `CanvasVirtualControl` for high-frequency drawing; disable backdrop effects on data-heavy windows | Measure with the Windows App SDK energy graph to avoid battery regressions.[^optimize]

---

## 6. Build configuration & automation

- Treat Release builds as the source of truth: set `RunAOTCompilation=true`, `UseInterpreter=false`, and validate `DebugType=portable` symbol publishing in CI.[^maui9]
- Add analyzer or unit tests that ensure every handler registered via reflection is marked in your `TrimmerRootDescriptor` file; break the build when entries go missing.[^linking]
- Record performance budgets (startup < 2 s, first interaction < 100 ms) and fail pull requests when telemetry dashboards exceed rolling averages.[^optimize]

---

## 7. Guardrails & regression testing

- Schedule weekly device lab runs that replay cold start, navigation, and scroll scenarios while exporting `dotnet-counters` metrics for trend analysis.[^dotnetcounters]
- Gate pull requests with lightweight smoke benchmarks (e.g., `BenchmarkDotNet` microbenchmarks for serializers or collection builders) so hot paths stay within budget.[^optimize]
- Capture animated GIFs from profiling sessions and attach them to knowledge base articles; visual diffs make layout regressions obvious during reviews.

---

## Checklist

- [ ] Release build profile with AOT/IL trimming validated on every platform.
- [ ] Baseline startup trace saved and tracked over time.
- [ ] Memory allocations monitored with `dotnet-counters` or Visual Studio profiler.
- [ ] UI virtualization and layout simplifications applied to heavy lists.
- [ ] Performance budgets enforced via CI automation and dashboards.

---

[^preview5]: InfoQ, ".NET 9 MAUI Preview 5: New Blazor Project Template, Android 15 Beta 2 Support," July 6, 2024. citeturn6search0
[^maui9]: Microsoft, "What's new in .NET 9," accessed November 1, 2025. citeturn4search0
[^linking]: Microsoft Learn, "Linking a .NET MAUI Android app," updated October 24, 2024. citeturn9search0
[^optimize]: Microsoft Learn, "Improve app performance - .NET MAUI," accessed November 1, 2025. citeturn4search3
[^dotnetcounters]: Microsoft Learn, "Investigate performance counters (dotnet-counters)," updated October 2, 2025. citeturn8search0
[^journey]: .NET Blog, "Performance Improvements in .NET MAUI," June 8, 2021. citeturn0search6

