---
title: Migrating Custom Renderers to Modern .NET MAUI Handlers
description: Step-by-step playbook for porting Xamarin.Forms custom renderers and platform code to .NET MAUI handler patterns that run on .NET 9.
last_reviewed: 2025-10-29
owners:
  - @prodyum/maui-guild
---

# Migrating Custom Renderers to Modern .NET MAUI Handlers

Xamarin.Forms custom renderers gave you per-platform control, but .NET MAUI replaces them with lighter, faster handlers and mapper extensions. This guide walks through the full migration arc--from keeping existing renderers alive via compatibility shims, to rewriting them as first-class handlers, to advanced mapper techniques for cross-platform polish. Every step targets .NET 9 tooling so your code stays current with today's MAUI release train.citeturn1search0

---

## 1. Readiness checklist

- **Upgrade to .NET 9 workloads.** Install the latest MAUI workloads (`dotnet workload install maui`) so your solution restores `Microsoft.Maui.Controls.Compatibility 9.0.x` alongside the handler stack.citeturn2search6
- **Inventory every renderer.** List the Xamarin.Forms control type, renderer class name, and the platform(s) it targets. Note any renderer attributes like `[ExportRenderer]`.
- **Decide on migration phase.** Tag each renderer as "compatibility", "hybrid", or "full handler" so you can prioritise the highest-impact controls first.

Build a worksheet that tracks control name, current renderer, target strategy, native dependencies, and available tests.

---

## 2. Phase one - light up compatibility renderers

When deadlines are tight, keep existing renderers alive inside a MAUI app by referencing the compatibility package and registering renderers at startup. This keeps shipping velocity while you modernise the code behind the scenes.citeturn0search0

```xml
<!-- In your .csproj -->
<ItemGroup>
  <PackageReference Include="Microsoft.Maui.Controls.Compatibility"
                    Version="9.0.120" />
</ItemGroup>
```

```csharp
// MauiProgram.cs
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Hosting;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureMauiHandlers(handlers =>
            {
                // register all renderers in an assembly
                handlers.AddCompatibilityRenderers(typeof(MyLegacyRenderer).Assembly);

                // or map a specific renderer
                handlers.AddCompatibilityRenderer(typeof(LegacyCameraView),
                                                  typeof(AndroidCameraRenderer));
            });

        return builder.Build();
    }
}
```

**Tips**

- Keep the registration logic centralised so PR diffs show any remaining compatibility dependencies.
- Add smoke tests that resolve legacy renderers to confirm nothing broke during the workload upgrade.

---

## 3. Phase two - port renderers to handlers

Handlers map cross-platform controls (virtual views) to native views with property and command mappers. Start by creating a virtual view that replaces the renderer's Xamarin.Forms control.citeturn1search0turn3search3

```csharp
// Controls/RichCameraView.cs
public class RichCameraView : View
{
    public static readonly BindableProperty IsFlashEnabledProperty =
        BindableProperty.Create(nameof(IsFlashEnabled), typeof(bool), typeof(RichCameraView), false);

    public bool IsFlashEnabled
    {
        get => (bool)GetValue(IsFlashEnabledProperty);
        set => SetValue(IsFlashEnabledProperty, value);
    }
}
```

Create a handler that wires property and command mappers to native APIs:

```csharp
// Handlers/RichCameraViewHandler.cs
public partial class RichCameraViewHandler :
    ViewHandler<RichCameraView, object>
{
    public static readonly IPropertyMapper<RichCameraView, RichCameraViewHandler> PropertyMapper =
        new PropertyMapper<RichCameraView, RichCameraViewHandler>(ViewMapper)
        {
            [nameof(RichCameraView.IsFlashEnabled)] = MapIsFlashEnabled
        };

    public static readonly CommandMapper<RichCameraView, RichCameraViewHandler> CommandMapper =
        new(ViewCommandMapper);

    public RichCameraViewHandler() : base(PropertyMapper, CommandMapper)
    {
    }
}
```

Implement platform-specific partial classes with multi-targeting so each platform compiles only its native code.citeturn4search0

```csharp
// Handlers/RichCameraViewHandler.Android.cs
public partial class RichCameraViewHandler
{
    protected override TextureView CreatePlatformView()
        => new TextureView(Context);

    static void MapIsFlashEnabled(RichCameraViewHandler handler, RichCameraView view)
    {
        handler.PlatformView?.Post(() =>
        {
            handler.CameraController?.SetFlash(view.IsFlashEnabled);
        });
    }
}
```

```csharp
// Handlers/RichCameraViewHandler.iOS.cs
public partial class RichCameraViewHandler
{
    protected override UIView CreatePlatformView()
        => new CameraPreviewView(MauiContext?.Services.GetRequiredService<IAVCaptureFactory>());

    static void MapIsFlashEnabled(RichCameraViewHandler handler, RichCameraView view)
    {
        handler.PlatformView?.UpdateFlash(view.IsFlashEnabled);
    }
}
```

Register the new handler once during startup:citeturn3search4

```csharp
builder.ConfigureMauiHandlers(handlers =>
{
    handlers.AddHandler(typeof(RichCameraView), typeof(RichCameraViewHandler));
});
```

**Validation checklist**

- Add UI tests per platform that toggle `IsFlashEnabled` and verify native logs.
- Instrument the handler with `ILogger<RichCameraViewHandler>` so telemetry picks up platform drift early.

---

## 4. Phase three - enhance with mapper extensions

Handlers can be globally or locally customised without subclassing every control. Use mapper extension methods (`PrependToMapping`, `AppendToMapping`, `ModifyMapping`) to tweak native components after the core mappings run.citeturn3search7

```csharp
RichCameraViewHandler.Mapper.AppendToMapping("Accessibility", (handler, view) =>
{
#if ANDROID
    handler.PlatformView.ContentDescription = "Live camera preview";
#elif IOS || MACCATALYST
    handler.PlatformView.IsAccessibilityElement = true;
    handler.PlatformView.AccessibilityTraits = UIAccessibilityTrait.Image;
#elif WINDOWS
    handler.PlatformView.SetValue(AutomationProperties.NameProperty, "Live camera preview");
#endif
});
```

For control-specific tweaks, subclass the virtual view and check `is MyRichCameraView` inside the mapper so global controls stay untouched.

---

## 5. Reusing native code by embedding MAUI views

If you invested heavily in native platform views, embed MAUI controls inside native shells (or vice versa) to reuse the same handler-based implementation across projects. This keeps your MAUI handler as the single source of truth while native apps host it via embedding APIs.citeturn0search0

Key steps:

1. Create a MAUI class library that exposes the custom control and handler.
2. Add `<UseMaui>true</UseMaui>` and `<MauiEnablePlatformUsings>true</MauiEnablePlatformUsings>` to the native head projects.
3. Initialise MAUI inside native view controllers/windows and host the handler-driven control with `ToPlatform`.

---

## 6. Hybrid strategy: adapter services + handlers

For features still using `DependencyService`, create adapters that forward to the new handler-driven implementation. This lets you phase out static lookups gradually while locking down the DI surface introduced in MAUI.

```csharp
public sealed class LegacyCameraAdapter : ICameraService
{
    private readonly Lazy<RichCameraView> _view;

    public LegacyCameraAdapter(Lazy<RichCameraView> view) => _view = view;

    public Task ToggleFlashAsync(bool enabled)
    {
        _view.Value.IsFlashEnabled = enabled;
        return Task.CompletedTask;
    }
}
```

Register `LegacyCameraAdapter` in the DI container so both handler-based pages and legacy views hit the same logic.

---

## 7. Testing & rollout playbook

| Step | Goal | Tools |
| --- | --- | --- |
| **Container smoke tests** | Ensure handlers register and instantiate | `ActivatorUtilities.CreateInstance`, MAUI head unit tests |
| **Platform UI tests** | Validate native behaviour parity | Appium, Playwright for .NET, platform UITest frameworks |
| **Performance regression** | Confirm handler is faster than renderer | `dotnet-counters`, Android Profiler, Xcode Instruments |
| **Accessibility audit** | Verify mapper tweaks meet WCAG | Accessibility Inspector, Windows Narrator |

Roll out handler replacements incrementally--feature flag by page or control type, and monitor telemetry per platform release.

---

## 8. Troubleshooting quick wins

- **Renderer still executes?** Remove `[assembly: ExportRenderer]` attributes so compatibility shims stop wiring in legacy classes.citeturn0search0
- **Handler compile errors** often stem from missing multi-targeting rules; ensure `.Android.cs` files are excluded from iOS builds and vice versa.citeturn4search0
- **Missing native APIs** on Windows? Confirm the handler partial class lives under `Platforms/Windows` so WinUI namespaces are available.citeturn4search0
- **Runtime `FileNotFoundException` for compatibility assemblies** signals the package isn't restored for every target--double-check NuGet locks and workload versions.

---

## Practice lab

1. **Warm-up:** Register an existing gradient button renderer via `AddCompatibilityRenderer`, then capture before/after screenshots on Android and iOS.
2. **Core migration:** Port the gradient button to a handler with property mappers and platform partial classes. Run UI tests to compare colours, ripple, and haptics.
3. **Advanced polish:** Append an accessibility mapper that adds content descriptions and haptics per platform, then measure startup benchmarks to confirm the handler initialises faster than the renderer.

Document migration progress in the team's completion checklist to keep visibility high.

---

## Further reading

- Migrate a custom renderer to a .NET MAUI handler.
- Customize .NET MAUI controls with handlers.
- Create custom controls with handlers (property & command mappers).
- Reuse existing custom renderers alongside .NET MAUI.
- Target multiple platforms from a MAUI single project.
