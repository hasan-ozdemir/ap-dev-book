---
title: Dependency Injection & Platform Services
description: Wire up reusable services, platform bridges, and legacy adapters across Xamarin.Forms and .NET MAUI using IoC patterns and multi-targeting.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

# Dependency Injection & Platform Services

Modern .NET MAUI ships with first-class dependency injection, giving you a consistent way to compose services, register view models, and surface platform features without hard wiring dependencies into UI code.citeturn10view0 Xamarin.Forms (now out of support) still relies on `DependencyService` in many production apps, so you’ll often need to bridge both worlds while keeping everything loosely coupled and testable.citeturn8search0

---

## Why inversion of control matters in cross-platform apps

- **Loose coupling** keeps MAUI Shell pages, handlers, and view models independent from concrete services, reducing the blast radius when you swap gateways (e.g., switching from REST to GraphQL).citeturn10view0
- **Shared startup** centralises configuration via the MAUI app builder, aligning with the single-project philosophy so every platform loads the same container graph.citeturn11view0
- **Migration safety nets** let you mix host-based DI with `DependencyService` adapters so legacy Xamarin.Forms code keeps working while you carve out new services behind interfaces.citeturn8search0

---

## Building a shared service graph in .NET MAUI

Create and configure your container in `MauiProgram.CreateMauiApp()`. Register views, view models, and services with the appropriate lifetime (`AddSingleton`, `AddTransient`, `AddScoped`) before calling `Build()`, because the container becomes immutable afterwards.citeturn10view0

```csharp
// MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            .ConfigureAppServices();

        return builder.Build();
    }

    private static MauiAppBuilder ConfigureAppServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddSingleton<IWeatherService, WeatherService>();

#if WINDOWS
        builder.Services.AddSingleton<IDeviceHud, Platforms.Windows.DeviceHud>();
#elif ANDROID
        builder.Services.AddSingleton<IDeviceHud, Platforms.Android.DeviceHud>();
#elif IOS || MACCATALYST
        builder.Services.AddSingleton<IDeviceHud, Platforms.Apple.DeviceHud>();
#endif

        return builder;
    }
}
```

This pattern mirrors how Microsoft configures the eShop multi-platform sample and demonstrates registering platform-specific implementations beside shared services.citeturn10view0turn11view0

### Consuming services

Inject dependencies through constructors in pages or view models so the container resolves them automatically during navigation:

```csharp
public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
```

Shell navigation will use the registered container to instantiate `MainPage` and supply its `MainViewModel`, avoiding manual service lookups.citeturn10view0

---

## Bridging into platform-specific code

.NET MAUI single-project multi-targeting lets you split implementations across platform folders or filename conventions, combining partial classes and conditional compilation.citeturn9view0 Keep shared contracts in the root project and implement details per platform:

```csharp
// Services/ClipboardService.shared.cs
public partial class ClipboardService : IClipboardService
{
    public Task SetTextAsync(string text) => SetPlatformTextAsync(text);
    public Task<string?> GetTextAsync() => GetPlatformTextAsync();

    protected partial Task SetPlatformTextAsync(string text);
    protected partial Task<string?> GetPlatformTextAsync();
}

// Services/ClipboardService.android.cs
public partial class ClipboardService
{
    protected partial Task SetPlatformTextAsync(string text)
    {
        var clipboard = Android.App.Application.Context
            .GetSystemService(Context.ClipboardService) as ClipboardManager;
        clipboard?.SetPrimaryClip(ClipData.NewPlainText("clipboard", text));
        return Task.CompletedTask;
    }

    protected partial Task<string?> GetPlatformTextAsync()
    {
        var clipboard = Android.App.Application.Context
            .GetSystemService(Context.ClipboardService) as ClipboardManager;
        return Task.FromResult(clipboard?.PrimaryClip?.GetItemAt(0)?.Text);
    }
}
```

Register the shared service once (`builder.Services.AddSingleton<IClipboardService, ClipboardService>();`) and the correct platform file is compiled into each target.

### Conditional handlers and builders

Combine `#if` directives with handler mappers when you must touch native views directly (e.g., removing an Android underline on an `Entry`).citeturn11view0 Keep these conditionals in dedicated extensions to avoid scattering platform flags across UI code.

---

## Working with legacy Xamarin.Forms DependencyService

Many existing codebases still call `DependencyService.Get<T>()` for capabilities like sensors or app settings. The original guidance breaks down into four steps: define a shared interface, implement it per platform, register with `[assembly: Dependency]`, and resolve from shared code.citeturn8search0

```csharp
// Shared project
public interface IGeoFenceService
{
    Task<bool> TryRegisterFenceAsync(string id, Location center, double radiusMeters);
}

// Platforms/Android
[assembly: Dependency(typeof(GeoFenceService))]
public sealed class GeoFenceService : IGeoFenceService
{
    public Task<bool> TryRegisterFenceAsync(string id, Location center, double radiusMeters)
    {
        // Use AndroidX GeofencingClient here…
    }
}
```

### Adapting DependencyService to MAUI DI

When migrating, wrap existing `DependencyService` calls inside an adapter registered with the MAUI container:

```csharp
public sealed class GeoFenceAdapter : IGeoFenceService
{
    private readonly IGeoFenceService _legacyService =
        Xamarin.Forms.DependencyService.Get<IGeoFenceService>();

    public Task<bool> TryRegisterFenceAsync(string id, Location center, double radiusMeters)
        => _legacyService.TryRegisterFenceAsync(id, center, radiusMeters);
}
```

Register `GeoFenceAdapter` in `MauiProgram`, then update consumers to rely solely on the injected interface. This keeps unit tests container-aware and constrains legacy code to a single boundary.

---

## Loosely coupled patterns beyond DI

- **Messaging & events:** Prefer typed event aggregators (e.g., `WeakReferenceMessenger`) over static `MessagingCenter` calls to avoid hidden dependencies. Resolve them through DI so publishers and subscribers can be swapped in tests.[^messenger]
- **Feature flags & configuration:** Keep environment-specific switches in a dedicated options class (`builder.Services.Configure<AppOptions>(config);`) instead of scattering `#if DEBUG` blocks.
- **Resilience:** Wrap platform handlers in retry or timeout policies (Polly integrates cleanly via DI) so failures on one platform do not cascade into others.

---

## Risk controls & testing checklist

1. **Container smoke test:** Add a unit test (or analyzer) that reels through `builder.Services` and attempts to resolve every registered type to catch missing dependencies early.
2. **Platform validation:** In CI, build Android, iOS, and Windows targets so platform-specific partial files stay in sync; multi-targeting errors surface at compile time.[^multi-target]
3. **Legacy audit:** Track all direct `DependencyService.Get<T>()` usages. For each, decide whether to (a) migrate to DI, (b) wrap with an adapter, or (c) retire the feature.
4. **Runtime diagnostics:** Inject logging abstractions (`ILogger<T>`) so handlers and services can record platform-specific failures without relying on static `Console.WriteLine`.

---

## Practice prompts

- Convert a Xamarin.Forms `DependencyService` implementation to a MAUI-registered adapter and measure the impact on unit test coverage.
- Implement a platform-specific telemetry pipeline using partial classes, with MAUI Essentials on Android/iOS and Windows Event Tracing on Windows.
- Add a health check page that resolves every singleton service and reports registration status at runtime.

---

## Further reading

- [Dependency injection in .NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/dependency-injection).
- [Target multiple platforms from a single project](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/single-project).
- [Xamarin.Forms DependencyService reference](https://learn.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/dependency-service/introduction).
- [WeakReferenceMessenger guidance (.NET Community Toolkit)](https://learn.microsoft.com/en-us/windows/communitytoolkit/mvvm/messenger).

[^multi-target]: Microsoft Learn, "Target multiple platforms from a single project," accessed October 30, 2025.
[^messenger]: Microsoft Learn, ".NET Community Toolkit messenger guidance," accessed October 30, 2025.

