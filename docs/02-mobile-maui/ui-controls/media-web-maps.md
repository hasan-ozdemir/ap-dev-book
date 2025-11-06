---
title: MAUI Media, Web, and Maps
description: Detailed guidance for media, web, and maps controls with .NET 9-ready XAML and C# samples.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

## 6. Media, Web, and Maps

### 6.1 WebView

`WebView` renders web content with navigation hooks, JavaScript interaction, and content loading from HTML strings or remote URLs.

```xml
<WebView Source="https://contoso.com/support"
         HeightRequest="480" />
```

```csharp
var supportView = new WebView
{
    Source = "https://contoso.com/support",
    HeightRequest = 480
};
supportView.Navigating += OnWebNavigating;
```

**Tips**

- Use `EvaluateJavaScriptAsync` for two-way communication.
- Set custom handlers to enforce navigation allowlists on Android.

### 6.2 BlazorWebView

`BlazorWebView` hosts Razor components in native MAUI apps, sharing code with web projects.

```xml
<BlazorWebView HostPage="wwwroot/index.html">
    <BlazorWebView.RootComponents>
        <RootComponent Selector="#app" ComponentType="{x:Type local:Dashboard}" />
    </BlazorWebView.RootComponents>
</BlazorWebView>
```

```csharp
var blazorView = new BlazorWebView
{
    HostPage = "wwwroot/index.html"
};
blazorView.RootComponents.Add(new RootComponent
{
    Selector = "#app",
    ComponentType = typeof(Dashboard)
});
```

**Tips**

- Inject scoped services via `blazorView.Services` to share the dependency injection graph.
- Use hybrid navigation to switch between native pages and Blazor routes.

### 6.3 Map

`Map` embeds native mapping engines with pins, polygons, and geocoding helpers.

```xml
<maps:Map x:Name="StoreMap"
          MapType="Street"
          IsShowingUser="True" />
```

```csharp
var map = new Map(MapSpan.FromCenterAndRadius(new Location(44.81, 20.46), Distance.FromKilometers(5)))
{
    IsShowingUser = true,
    MapType = MapType.Street
};
map.Pins.Add(new Pin { Label = "HQ", Location = new Location(44.81, 20.46) });
```

**Tips**

- Register map services and request location permissions per platform before showing the control.
- Use `Map.MapClicked` for interactive selection experiences.

---

### 6.4 CameraView

`CameraView` (Community Toolkit) renders a live preview, captures photos, switches lenses, and exposes granular camera settings.

```xml
<ContentPage ...
             xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit">
    <toolkit:CameraView x:Name="CameraPreview"
                        Camera="Back"
                        CaptureMode="Photo"
                        FlashMode="{Binding FlashMode}"
                        IsEnabled="False" />
</ContentPage>
```

```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseMauiCommunityToolkit()
    .UseMauiCommunityToolkitCamera();

protected override async void OnAppearing()
{
    if (await Permissions.RequestAsync<Permissions.Camera>() == PermissionStatus.Granted)
    {
        CameraPreview.IsEnabled = true;
        await CameraPreview.StartCameraPreview();
    }
}

async Task CaptureAsync()
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
    await using var imageStream = await CameraPreview.CaptureImage(cts.Token);
    await _imageStore.SaveAsync(imageStream);
}
```

**Tips**

- Declare camera/microphone permissions in the platform manifests and request them at runtime before enabling the preview.
- Bind `ZoomFactor`, `SupportedResolutions`, and `FlashMode` to give end users manual controls without custom native handlers.
- Call `StopCameraPreview`/`Dispose` on navigation to release hardware for other capture surfaces (QR scanners, video calls).

### 6.5 MediaElement

`MediaElement` (Community Toolkit) provides cross-platform audio and video playback with transport controls, metadata, and background audio hooks.

```xml
<ContentPage ...
             xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit">
    <toolkit:MediaElement x:Name="VideoPlayer"
                          Source="https://contoso-cdn.net/training/intro.mp4"
                          ShouldAutoPlay="False"
                          ShouldShowPlaybackControls="True"
                          Aspect="AspectFit" />
</ContentPage>
```

```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseMauiCommunityToolkitMediaElement();

VideoPlayer.MediaEnded += (_, _) => VideoPlayer.SeekTo(TimeSpan.Zero);

protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
{
    if (VideoPlayer.CurrentState == MediaElementState.Playing)
    {
        VideoPlayer.Stop();
    }
}
```

**Tips**

- Configure ATS/Network Security settings for HTTP streams; prefer HTTPS CDNs or signed URLs for production playback.
- Use `ShouldKeepScreenOn` and `MetadataArtist`/`MetadataArtworkUrl` to integrate with OS media sessions and lock screens.
- For interactive apps, bind `CurrentState` and `Position` to view-model timers so UI updates reflect buffering and seek progress.

---

## Further reading

- [Microsoft Learn: WebView](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/webview)citeturn17search0
- [Microsoft Learn: MediaElement (Community Toolkit)](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/mediaelement)citeturn17search1
- [Microsoft Learn: Map](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/map)citeturn17search2

