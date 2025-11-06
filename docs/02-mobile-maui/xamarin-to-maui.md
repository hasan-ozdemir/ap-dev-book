---
title: Xamarin.Forms to .NET MAUI – Comparison & Migration Guide
description: Understand what changed from Xamarin.Forms to .NET MAUI and migrate your apps with confidence.
last_reviewed: 2025-10-29
owners:
  - @prodyum/maui-guild
---

# Xamarin.Forms to .NET MAUI – Comparison & Migration Guide

If you built apps with Xamarin.Forms, .NET MAUI is the next evolution. It unifies projects, streamlines platform APIs, and brings performance and tooling improvements. This guide highlights similarities, differences, and a step-by-step migration path.

---

## 1. High-level comparison

| Area | Xamarin.Forms | .NET MAUI | What changed |
| --- | --- | --- | --- |
| Project structure | Multiple head projects (Android, iOS, UWP, etc.) | Single multi-target project with `Platforms` folder | Simplifies build and sharing.citeturn0search5 |
| Rendering model | Custom renderers | Handlers + mappers | Cleaner, lightweight architecture; easier customization.citeturn0search5turn0search9 |
| Platform APIs | Xamarin.Essentials side package | Essentials integrated into MAUI | No extra package; APIs available out of the box.citeturn0search8 |
| Hot reload | Limited XAML hot reload | Improved XAML & C# hot reload | Faster iteration with shared tooling.citeturn0search1 |
| Tooling | Visual Studio (Windows, Mac) | Visual Studio (Windows) + VS Code extension (preview) | VS for Mac retired; VS Code extension fills cross-platform gap.citeturn0search7turn0search8 |
| Target frameworks | `Xamarin.Forms` packages tied to Xamarin.iOS/Android | `net9.0-android`, `net9.0-ios`, etc. | .NET unified platform, long-term support.citeturn0search9turn0search6 |

---

## 2. Feature improvements

- **Performance**: MAUI handlers reduce view inflation overhead; .NET 9 adds trimming + AOT enhancements.citeturn0search0turn0search5
- **Graphics & Media**: `Microsoft.Maui.Graphics`, `GraphicsView`, and MediaElement improvements replace custom renderers.
- **Platform integration**: MAUI Essentials exposes sensors, storage, connectivity without separate packages.
- **Tooling**: Live Visual Tree, XAML Live Preview, and improved Hot Reload accelerate UI debugging.citeturn0search1
- **Windows support**: WinUI 3 replaces Xamarin.Forms UWP, delivering fluent design integration.
- **Blazor Hybrid**: Embed web UI via `BlazorWebView`, not available in Xamarin.Forms.

---

## 3. Migration strategy

### 3.1 Preparation

1. Upgrade solution to the latest Xamarin.Forms LTS before migration.
2. Audit dependencies: ensure libraries support .NET 6+ or MAUI.
3. Review custom renderers, dependency services, and platform-specific code; plan handler equivalents.

### 3.2 Recreate the project

1. Create a MAUI project (`dotnet new maui`).
2. Copy assets (`Resources`, images, fonts). Update `MauiProgram.cs` for DI registrations.
3. Recreate `App.xaml`, resource dictionaries, and Shell navigation.

### 3.3 Convert views & controls

- Most XAML files port with minimal changes. Update namespaces (`xmlns:controls="clr-namespace:Contoso.Mobile.Controls"`).
- Replace `Device.RuntimePlatform` with `OperatingSystem.IsAndroid()`, etc.
- Convert custom renderers to handlers:

```csharp
public class GradientButtonHandler : ButtonHandler
{
    public static IPropertyMapper<Button, GradientButtonHandler> Mapper =
        new PropertyMapper<Button, GradientButtonHandler>(ButtonHandler.Mapper)
        {
            [nameof(Button.Background)] = MapBackground
        };
}
```

### 3.4 Services & dependency injection

- Xamarin.Forms `DependencyService` → MAUI DI:

```csharp
builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>();
```

### 3.5 Platform-specific code

- Move `MainActivity`, `AppDelegate`, etc., into `Platforms` folder and update MAUI templates (Handlers, permission APIs).

### 3.6 Build & test

- Run `dotnet workload restore`, `dotnet build`, `dotnet test`.
- Use MAUI migration tooling (`dotnet tool install -g upgrade-assistant`) for automated help.

---

## 4. Compatibility packages

- `Microsoft.Maui.Controls.Compatibility` retains legacy APIs such as `NavigationPage`, legacy renderers during transition.
- Use sparingly; plan to remove after migrating to handlers.

---

## 5. Sample renderer → handler conversion

**Xamarin.Forms renderer:**

```csharp
[assembly: ExportRenderer(typeof(GradientButton), typeof(GradientButtonRenderer))]
public class GradientButtonRenderer : ButtonRenderer
{
    protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
    {
        base.OnElementChanged(e);
        if (Control is not null)
        {
            Control.Background = new GradientDrawable();
        }
    }
}
```

**.NET MAUI handler:**

```csharp
public class GradientButton : Button { }

public class GradientButtonHandler : ButtonHandler
{
    public GradientButtonHandler() : base(Mapper) { }

    public static IPropertyMapper<Button, GradientButtonHandler> Mapper =
        new PropertyMapper<Button, GradientButtonHandler>(ButtonHandler.Mapper)
        {
            [nameof(Button.Background)] = MapBackground
        };

    private static void MapBackground(GradientButtonHandler handler, Button view)
    {
#if ANDROID
        var gradient = new GradientDrawable();
        handler.PlatformView.SetBackground(gradient);
#endif
    }
}

builder.ConfigureMauiHandlers(handlers =>
{
    handlers.AddHandler(typeof(GradientButton), typeof(GradientButtonHandler));
});
```

---

## 6. Testing after migration

- Run automated tests (unit/integration/UI) to catch regressions.
- Validate trimming & linking (especially when using reflection-heavy libraries).
- Smoke test on all platforms; check for splash screen/asset issues.

---

## 7. Pitfalls & tips

- Remove deprecated Xamarin.Forms packages (`Xamarin.Essentials`, `Xamarin.Forms.Visual.Material`).
- Check for resource naming differences (`@drawable/icon` vs `Resources/Images/icon.png`).
- Update CI pipelines to use `dotnet build/publish` instead of `msbuild /t:Install`.
- Monitor bundle size; adjust linking/AOT settings as needed.

---

## Checklist

- [ ] Dependencies compatible with .NET MAUI.
- [ ] Resources and XAML ported.
- [ ] Custom renderers converted or wrapped via compatibility package.
- [ ] DependencyService replaced with DI.
- [ ] Platform-specific code updated.
- [ ] Automated tests passing on all targets.

---

## Further reading

- Official Xamarin.Forms → .NET MAUI migration docs.citeturn0search6
- .NET MAUI vs Xamarin.Forms handler architecture overview.citeturn0search5turn0search9
- .NET MAUI tooling improvements (Hot Reload, Live Preview).citeturn0search7turn0search1
