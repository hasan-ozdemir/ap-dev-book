---
title: .NET MAUI Community Toolkit Playbook
description: Ship richer cross-platform experiences by composing Community Toolkit views, behaviors, and services with .NET 9+ MAUI apps.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

# .NET MAUI Community Toolkit Playbook

The .NET MAUI Community Toolkit layers production-ready controls, behaviors, markup helpers, and services on top of the core framework. Use this playbook to decide when to adopt each package, wire it into your solutions, and apply recipes that keep apps upgrade-safe.

## 1. Foundation & setup

### 1.1 When to reach for the toolkit

- Accelerate delivery with prebuilt media, camera, and drawing experiences instead of maintaining custom renderers.
- Close feature gaps surfaced during Xamarin.Forms migrations (popups, speech-to-text, legacy secure storage) while staying inside a single MAUI project structure.
- Align your support window with the official MAUI cadence; the toolkit tracks .NET releases and requires you to stay current within six months of a new major version.citeturn1search1

### 1.2 Install and initialize

Add the base package and call the builder extension during host startup:

```csharp
using CommunityToolkit.Maui;

var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseMauiCommunityToolkit(options =>
    {
        options.SetShouldSuppressExceptionsInConverters(false);
        options.SetShouldSuppressExceptionsInBehaviors(false);
        options.SetShouldSuppressExceptionsInAnimations(false);
    });
```

The optional `CommunityToolkit.Maui.Markup`, `.MediaElement`, `.Camera`, and other satellite packages expose their own `UseMauiCommunityToolkit*` helpers; add them only when the feature is needed to keep app size small.citeturn1search0

### 1.3 Package selection matrix

| Package | Primary capabilities | Typical use cases |
| --- | --- | --- |
| `CommunityToolkit.Maui` | Popups, alerts, behaviors, converters, drawing | Replace legacy renderers, add inline validation, surface transient UI |
| `CommunityToolkit.Maui.Camera` | CameraView preview, capture, zoom, flash control | ID verification flows, barcode or document scanning |
| `CommunityToolkit.Maui.MediaElement` | Cross-platform audio/video playback | Marketing carousels, training videos, offline tutorials |
| `CommunityToolkit.Maui.Markup` | Fluent C# markup helpers | Compose UI in code-behind or modules without XAML |
| `CommunityToolkit.Maui.Core` | Snackbar, toast, shared primitives | Consistent notifications and alerts |

## 2. UI building blocks and recipes

### 2.1 Popup v2 for modal flows

Popup v2 trades the old renderer-based approach for a ContentView-first model, enabling DI registration and reuse across navigation stacks. Register popups with scoped or singleton lifetime and drive them from any page:

```csharp
using CommunityToolkit.Maui.Views;

public partial class ConfirmationPopup : Popup
{
    public ConfirmationPopup() => InitializeComponent();
}

await this.ShowPopupAsync(new ConfirmationPopup(), PopupOptions.Empty, token);
```

When upgrading from Popup v1, update namespaces (`CommunityToolkit.Maui`), re-register popups via `AddSingletonPopup`, and remove custom layout hacks that v2 now handles internally.citeturn2search4

### 2.2 Camera and media experiences

CameraView delivers live preview, multi-camera switching, zoom, focus, and capture commands. Combine it with MVVM commands to encapsulate permissions and lifecycle:

```xaml
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
    x:Class="Contoso.CameraPage">
    <Grid>
        <toolkit:CameraView
            x:Name="Camera"
            SelectedCamera="{Binding SelectedCamera}"
            CaptureImageCommand="{Binding CaptureCommand}" />
    </Grid>
</ContentPage>
```

```csharp
public partial class CameraViewModel
{
    readonly CameraView camera;

    public CameraViewModel(CameraView cameraView) => camera = cameraView;

    public IAsyncRelayCommand CaptureCommand => new AsyncRelayCommand(async () =>
    {
        var file = await camera.CaptureImageAsync();
        await mediaStore.SaveAsync(file);
    });
}
```

CameraView ships via `CommunityToolkit.Maui.Camera` and inherits the CameraView API introduced in toolkit 9.x; toolkit 11 adds breaking changes that require .NET 9 SDK 9.0.101 or later.citeturn5search1turn5search3

MediaElement enables rich playback with customizable transport controls. Initialize it with `.UseMauiCommunityToolkitMediaElement()` and bind to remote or embedded sources:

```xaml
<toolkit:MediaElement
    Source="https://contoso.cdn/media/tutorial.mp4"
    ShouldAutoPlay="True"
    ShouldShowPlaybackControls="True"
    MetadataTitle="Getting Started" />
```

TextureView support on Android unlocks transparency overlays for picture-in-picture tutorials.citeturn6search0

### 2.3 Drawing and ink capture

DrawingView offers multi-line signature capture and gesture-driven sketching. Bind `Lines` to a view-model collection to persist strokes, and toggle `IsMultiLineModeEnabled` to support scribbles or white-boarding scenarios.citeturn5search0

### 2.4 Alerts, snackbars, and toasts

Use `Snackbar.Make()` for context-aware notifications that auto-dismiss, anchor near controls, and expose action buttons:

```csharp
using CommunityToolkit.Maui.Alerts;

await Snackbar.Make(
    "Settings saved",
    action: () => analytics.Track("settings_saved"),
    actionButtonText: "Undo",
    duration: TimeSpan.FromSeconds(5)).Show();
```

Customize fonts, colors, and anchor targets via `SnackbarOptions` to align with brand guidelines.citeturn3search5turn3search4

### 2.5 Fluent C# markup

`CommunityToolkit.Maui.Markup` exposes fluent helpers to build declarative layouts in C#. Use typed row/column enums, chained bindings, and gesture helpers to reduce XAML overhead in dynamic modules:

```csharp
using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

Content = new Grid
{
    RowDefinitions = Rows.Define((Row.Title, 48), (Row.Form, Star)),
    ColumnDefinitions = Columns.Define((Column.Label, 120), (Column.Input, Star)),
}.Children(
    new Label().Text("Code:").Row(Row.Form).Column(Column.Label),
    new Entry().Row(Row.Form).Column(Column.Input).Bind(Entry.TextProperty, nameof(ViewModel.Code)));
```

The fluent API supports Hot Reload, typed bindings, and automation property helpers for accessibility.citeturn2search0

### 2.6 Speech and accessibility services

`SpeechToText` exposes online and offline recognition providers. Register `OfflineSpeechToText.Default` for airplane-mode scenarios, request runtime permissions, and surface fallback toasts on denial:

```csharp
builder.Services.AddSingleton<ISpeechToText>(OfflineSpeechToText.Default);

if (!await speechToText.RequestPermissions(token))
{
    await Snackbar.Make("Microphone denied").Show();
    return;
}

await speechToText.StartListenAsync(
    new SpeechToTextOptions { Culture = CultureInfo.CurrentCulture, ShouldReportPartialResults = true },
    token);
```

Offline recognition requires Android API 33+ and iOS 13+. Toolkit 11.0.0 introduced the offline engine and updated bindings; plan device testing accordingly.citeturn3search0turn3search1turn3search2

## 3. Integration patterns

- **Dependency injection:** Register toolkit services (`AddSingletonPopup`, `AddTransient<CameraViewModel>`) so popups and views can resolve platform abstractions without static singletons.citeturn2search4turn5search1
- **Feature flags:** Wrap optional packages in `#if` directives or configuration toggles when targeting kiosk, wearable, or Lite builds to trim assemblies during publishing.
- **Telemetry:** Use toolkit events (`MediaEnded`, `CameraView.CaptureCompleted`) to emit structured analytics and measure adoption of advanced UI features.citeturn6search0turn5search1

## 4. Upgrade and quality checklist

1. **Track release notes:** Monitor each toolkit update for breaking changes (behaviors no longer inherit `BindingContext`, popup namespace moves). Apply migration steps immediately after upgrading to avoid runtime regressions.citeturn1search4turn2search4
2. **Smoke test device flows:** Validate popups, camera capture, and offline speech on the oldest supported Android/iOS versions after each SDK update; the toolkit follows .NET’s monthly servicing cadence.citeturn1search1turn5search3
3. **Linker/AOT review:** When publishing trimmed or AOT builds, annotate view-model members consumed via reflection (behaviors, markup typed bindings) to prevent missing method exceptions.
4. **UX accessibility:** Combine `AutomationPropertiesExtensions` (markup package) with Snackbar and Popup content for screen reader parity.citeturn2search0turn3search5

## 5. Further learning

- Official samples: clone `CommunityToolkit/Maui` and explore the gallery app for ready-made camera, popup, and drawing demos.citeturn1search0
- Release cadence: follow InfoQ release coverage to anticipate breaking changes and .NET 9 alignment milestones.citeturn1search4turn5search3
- Deep dives: review feature specs linked from Microsoft Learn articles to understand extension points and platform-specific behaviors before customizing controls.citeturn2search0turn6search0
