---
title: MAUI Composition Patterns
description: Compose multiple controls into reusable UX patterns with .NET 9-ready XAML and C# examples.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

## Putting It Together

Use the category guides in this folder as building blocks, then layer in layout, styling, and dependency injection patterns to deliver cohesive screens that feel native on every platform.

### Dashboard cards

- Combine `Grid` + `Border` + `CollectionView` to surface KPIs with responsive breakpoints.
- Inject services (analytics, notifications) via constructor injection so cards stay testable.
- Attach `SemanticProperties` descriptions to keep cards screen-reader friendly.

### Wizard & form flows

- Pair `AppShell` routes with `ContentPage` sections to drive multi-step onboarding.
- Reuse `Entry`, `Picker`, `DatePicker`, and `RadioButton` templates through `ControlTemplate` or `DataTemplate` resources.
- Bind validation results to `VisualStateManager` so inputs highlight errors without imperative code.

### Media & capture experiences

- Embed `CameraView` or `MediaElement` alongside `CollectionView` thumbnails to deliver capture-review-upload workflows.
- Use `ToolbarItem` and `MenuBarItem` for export/share actions, and bind commands to your view-model.
- Record playback and capture telemetry via `ActivitySource` to investigate performance regressions.

### Navigation shells

- Configure `Shell` with dashboard, search, and settings tabs; use `FlyoutItem` for desktop flyout menus.
- Apply consistent theming via `ResourceDictionary` and `AppThemeColor` so light/dark palettes stay in sync.
- Register routes for detail pages and deep links with `Routing.RegisterRoute`.

---

## Further reading

- [Microsoft Learn: .NET MAUI control gallery sample](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/)citeturn9search1
- [Microsoft Learn: .NET MAUI controls overview](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/)citeturn10search0
- [InfoQ: .NET MAUI Community Toolkit Adds .NET 9 Support](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/)citeturn18search0

