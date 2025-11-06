---
title: UI Components & Experience Design
description: Build responsive, accessible MAUI interfaces that leverage .NET 9 control enhancements and community toolkits.
last_reviewed: 2025-10-29
owners:
  - @prodyum/maui-guild
---

# UI Components & Experience Design

.NET MAUI 9 continues to mature the UI stack with new first-party controls, deeper desktop integrations, and updated tooling that shortens the inner loop for designers and developers alike.[^maui-9-ga] Use this guide to design polished, production-ready experiences across mobile and desktop.

> **Need a control-by-control reference?** See the [MAUI Control Catalog](./ui-controls/catalog.md) for per-component XAML and C# recipes.

## 1. Core control improvements

- **CollectionView & CarouselView** now use a rewritten handler on iOS and Mac Catalyst, reducing scrolling glitches and memory churn on data-heavy screens.[^maui-9-ga]
- **TitleBar (Windows)** lets you brand and populate the window chrome with shell commands; the API is currently Windows-only, with Mac Catalyst support slated for a future release.[^maui-9-ga]
- **HybridWebView** adds initialization events and `InvokeJavaScriptAsync` overloads so you can host React/Angular/Vue experiences with a strongly typed bridge back to .NET.[^maui-10-whatsnew]

## 2. Layout patterns

| Scenario | Recommended layout | Notes |
|----------|--------------------|-------|
| Master-detail navigation | `Grid` + adaptive visual states | Toggle between stacked (mobile) and side-by-side (tablet/desktop) experiences. |
| Infinite feeds | `CollectionView` with `DataTemplateSelector` | Combine with `LoadMoreCommand` and skeleton placeholders for perceived performance. |
| Dashboard | `FlexLayout` + `UniformItemsLayout` | Keeps cards responsive while honoring light/dark and high-contrast theme resources. |

> **Tip:** Keep layout primitives in partial XAML files and pair them with view-models or MVU handlers for unit-testability.

## 3. Toolkit accelerators

- **.NET MAUI Community Toolkit 11.0** brings offline speech recognition, .NET 9 support, and numerous bug fixes for media, popups, and navigation behaviors.[^toolkit-11]
- **Syncfusion Toolkit 1.0.4** adds AOT-safe trimming support so rich controls can ship in Native AOT builds without bloating package size.[^syncfusion-104]
- **Telerik UI for .NET MAUI 11.0.0** now ships unified theming (with multiple swatches) and merges trial/commercial packages, simplifying design system adoption.[^telerik-theme]

## 4. Accessibility & localization

- Apply `SemanticProperties.Description`, `Hint`, and heading levels so screen readers describe controls accurately across platforms.[^semantic]
- Telerik’s 2025 Q3 service release adds screen-reader fixes for `RadCollectionView`, including proper announcement of grouped items—validate third-party controls alongside native ones.[^telerik-accessibility]
- For RTL markets, set `FlowDirection="RightToLeft"` on views such as Syncfusion’s PDF Viewer to flip layout and gestures while reusing the same view-model logic.[^rtl-syncfusion]

## 5. Design system integration

1. Define a shared `ResourceDictionary` for color ramps, typography, spacing, and drop shadows.
2. Use `OnAppTheme` to deliver light/dark/high-contrast palettes from a single resource set.
3. Centralize glyphs in `Resources/Fonts` and expose them via `FontImageSource` or the starter template’s icon helpers bundled with the .NET 9 sample app.[^maui-blog-template]
4. Prefer `DataTemplateSelector` or compiled bindings for component variants so the same view can adapt to phone, tablet, and desktop densities.

## 6. Performance & tooling guidelines

- Lean on trimming/native AOT improvements in .NET 9 to ship smaller, faster binaries—align code with the rewritten CollectionView and new template defaults.[^maui-blog-template]
- Use XAML Live Preview in Visual Studio 2022 17.14 to inspect MAUI layouts at design time (Windows and Android emulators) without starting a debug session.[^vs-live-preview]
- Profile gesture-heavy pages with Hot Reload + Live Preview to catch layout thrash or overdraw before reaching devices.

## 7. Validation checklist

- [ ] Controls render correctly across Android, iOS, macOS, and Windows target frames.
- [ ] Accessibility scans (Narrator, VoiceOver, TalkBack) confirm descriptions, focus order, and contrast.
- [ ] Animations sustain >= 55 FPS on baseline devices (Moto G Power, iPhone SE, Surface Laptop Go).
- [ ] Third-party toolkits are pinned in `Directory.Packages.props` and release notes reviewed for breaking changes.

Adopting these practices keeps Prodyum experiences aligned with the latest MAUI platform capabilities while protecting quality across every device family.

[^maui-9-ga]: Edin Kapić, “.NET MAUI 9 Launched with Better Performance, New Controls,” InfoQ, Nov 20 2024.citeturn4search2
[^maui-10-whatsnew]: Microsoft Learn, “What’s new in .NET MAUI for .NET 10,” accessed Oct 29 2025.citeturn0search0
[^toolkit-11]: Edin Kapić, “.NET MAUI Community Toolkit Adds .NET 9, Offline Speech Recognition,” InfoQ, Feb 21 2025.citeturn4search4
[^syncfusion-104]: Syncfusion Help, “Syncfusion Toolkit for .NET MAUI Release Notes v1.0.4,” Mar 13 2025.citeturn0search9
[^telerik-theme]: Telerik, “Telerik UI for .NET MAUI 11.0.0 (2025 Q2) Release History,” May 21 2025.citeturn0search8
[^semantic]: Microsoft Learn, “SemanticProperties extensions,” updated Sept 2022, accessed Oct 29 2025.citeturn5search0
[^telerik-accessibility]: Telerik, “Telerik UI for .NET MAUI 11.0.1 Release Notes,” Jul 9 2025.citeturn0search7
[^rtl-syncfusion]: Syncfusion, “Right to left in .NET MAUI PDF Viewer (SfPdfViewer),” May 23 2025.citeturn6search2
[^maui-blog-template]: James Montemagno, “Announcing .NET 9,” .NET Blog, Nov 12 2024.citeturn4search1
[^vs-live-preview]: Rachel Kang, “Enhancements to XAML Live Preview in Visual Studio for .NET MAUI,” Visual Studio Blog, Sept 23 2025.citeturn7search0





