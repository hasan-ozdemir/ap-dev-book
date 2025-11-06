---
title: MAUI Essentials Companion
description: Master every Microsoft.Maui.Essentials API-from setup to advanced patterns-with runnable .NET 9 samples.
last_reviewed: 2025-10-29
owners:
  - @prodyum/maui-guild
---

# MAUI Essentials Companion

.NET MAUI Essentials (formerly Xamarin.Essentials) ships with the MAUI workload and provides a unified set of cross-platform APIs that surface device capabilities without forcing you into platform projects. This playbook documents every Essentials feature, shows how to wire it up in .NET 9 solutions, and shares migration notes for teams coming from Xamarin.Forms.[^platform-overview][^essentials-package]

> **Target runtime:** All samples use the `net9.0` target frameworks (Android, iOS, Mac Catalyst, Windows). When you scaffold a MAUI project on .NET 9, Essentials APIs come along for the ride via `<UseMaui>true</UseMaui>`.

---

## 1. Project setup

### 1.1 csproj configuration

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net9.0-android;net9.0-ios;net9.0-maccatalyst;net9.0-windows10.0.19041.0</TargetFrameworks>
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

- **Non-MAUI heads** (e.g., .NET for iOS/Android class libraries): set `<UseMauiEssentials>true</UseMauiEssentials>` or reference the `Microsoft.Maui.Essentials` NuGet package directly.[^use-maui-essentials]
- **Permissions**: For runtime permissions (location, camera, etc.), Essentials relies on platform manifests. Use the MAUI `Platforms` folders or multi-targeted MSBuild items to declare capabilities.

### 1.2 Dependency injection registration

Essentials APIs are static convenience classes, but you can wrap them for testability:

```csharp
builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();

public sealed class ConnectivityService : IConnectivityService
{
    public bool HasInternet => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
}
```

---

## 2. Quick navigation

- [ApplicationModel essentials](#3-applicationmodel-essentials)
  - [App actions](#31-app-actions)
  - [App info](#32-app-info)
  - [Browser](#33-browser)
  - [Launcher](#34-launcher)
  - [Main thread](#35-main-thread)
  - [Permissions](#36-permissions)
  - [Version tracking](#37-version-tracking)
- [Communication & identity](#4-communication-identity)
  - [Contacts](#41-contacts)
  - [Email](#42-email)
  - [Phone dialer](#43-phone-dialer)
  - [SMS](#44-sms)
  - [Web authenticator](#45-web-authenticator)
- [Data transfer](#5-data-transfer)
  - [Clipboard](#51-clipboard)
  - [Share](#52-share)
- [Device & power](#6-device-power)
  - [Battery](#61-battery)
  - [Connectivity](#62-connectivity)
  - [Device info](#63-device-info)
  - [Device display](#64-device-display)
  - [App theme resources](#65-app-theme-resources)
  - [Flashlight](#66-flashlight)
  - [Haptic feedback](#67-haptic-feedback)
  - [Vibration](#68-vibration)
- [Sensors](#7-sensors-suite)
  - [Shake detection](#71-shake-detection)
- [Location stack](#8-location-stack)
  - [Geolocation](#81-geolocation)
  - [Geocoding](#82-geocoding)
  - [Maps](#83-maps)
- [Media & voice](#9-media-voice)
  - [Media picker](#91-media-picker)
  - [Screenshot](#92-screenshot)
  - [Text-to-speech](#93-text-to-speech)
  - [Unit converters](#94-unit-converters)
- [Storage & preferences](#10-storage-preferences)
  - [File picker](#101-file-picker)
  - [File system helpers](#102-file-system-helpers)
  - [Preferences](#103-preferences)
  - [Secure storage](#104-secure-storage)
- [Converters & platform helpers](#11-converters-platform-helpers)
  - [Color converters](#111-color-converters)
  - [Platform extensions](#112-platform-extensions)
- [Testing Essentials features](#12-testing-essentials-features)
- [Pre-Essentials landscape (Xamarin.Forms era)](#13-pre-essentials-landscape-xamarinforms-era)
- [Troubleshooting on .NET 9](#14-troubleshooting-on-net-9)
- [From zero to hero learning path](#15-from-zero-to-hero-learning-path)
- [Reference checklist](#16-reference-checklist)
- [References](#17-references)

## 3. ApplicationModel essentials

### 3.1 App actions[^app-actions]

- **Why**: Provide deep links and quick shortcuts from the launcher without platform-specific registries.
- **Code**:
  ```csharp
  await AppActions.SetAsync(new[]
  {
      new AppAction("open_favorites", "Open Favorites", icon: "icon_favorite.png"),
      new AppAction("new_note", "New Note")
  });

  AppActions.AppActionInvoked += async (_, args) =>
  {
      if (args.AppActionId == "new_note")
      {
          await Shell.Current.GoToAsync("//notes/create");
      }
  };
  ```
- **Tips**: Keep action identifiers stable for analytics, and package monochrome icons for iOS to avoid runtime warnings.

### 3.2 App info[^app-info]

- **Why**: Surface app metadata (name, version, requested theme) in diagnostics and UI.
- **Code**:
  ```csharp
  var info = AppInfo.Current;
  VersionLabel.Text = $"{info.Name} v{info.VersionString} ({info.BuildString})";
  var isDark = info.RequestedTheme == AppTheme.Dark;
  ```
- **Tips**: Use `info.MinimumOsVersion` in telemetry to validate store submissions.

### 3.3 Browser[^browser]

- **Why**: Launch trusted URLs in either an in-app browser or the system browser with minimal ceremony.
- **Code**:
  ```csharp
  await Browser.Default.OpenAsync(
      new Uri("https://contoso.com/support"),
      BrowserLaunchMode.SystemPreferred);
  ```
- **Tips**: Prefer `SystemPreferred` to leverage Android custom tabs and SFSafariViewController on iOS for shared cookies.

### 3.4 Launcher[^launcher]

- **Why**: Hand off files, URIs, or custom schemes to other apps.
- **Code**:
  ```csharp
  var file = Path.Combine(FileSystem.CacheDirectory, "report.txt");
  await File.WriteAllTextAsync(file, reportMarkdown);

  await Launcher.Default.OpenAsync(new OpenFileRequest(
      "Share report",
      new ReadOnlyFile(file)));
  ```
- **Tips**: When launching custom schemes, call `await Launcher.CanOpenAsync(uri)` to avoid user-facing failures.

### 3.5 Main thread[^main-thread]

- **Why**: Marshal UI work back to the main thread from background operations.
- **Code**:
  ```csharp
  await MainThread.InvokeOnMainThreadAsync(() =>
  {
      StatusLabel.Text = "Sync complete";
      SyncIndicator.IsRunning = false;
  });
  ```
- **Tips**: Wrap Essentials access that requires UI thread (clipboard, haptics) inside `InvokeOnMainThreadAsync` to avoid `InvalidOperationException`s.

### 3.6 Permissions[^permissions]

- **Why**: Check and request runtime permissions in a consistent, testable way.
- **Code**:
  ```csharp
  var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
  if (status != PermissionStatus.Granted)
  {
      status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
  }

  if (status == PermissionStatus.Granted)
  {
      await _locationService.RefreshAsync();
  }
  ```
- **Tips**: Always explain why a permission is needed before invoking `RequestAsync`, and handle the `Denied`/`Restricted` states gracefully. On Android 14+ the system photo picker enforces partial media access, so prefer `MediaPicker` workflows over legacy storage permissions for photos and video to remain compliant.[^android-photo-picker]

### 3.7 Version tracking[^version-tracking]

- **Why**: Detect first-run scenarios, upgrades, and downgrades without building your own preference store.
- **Code**:
  ```csharp
  VersionTracking.Track();

  if (VersionTracking.IsFirstLaunchEver)
  {
      await _onboardingService.RunAsync();
  }

  if (VersionTracking.IsFirstLaunchForCurrentVersion)
  {
      await _releaseNotesService.ShowAsync();
  }
  ```
- **Tips**: Migrate the Xamarin.Essentials container to the MAUI container during upgrades to keep historical data intact.
## 4. Communication & identity

### 4.1 Contacts[^contacts]

- **Why**: Bring the system contact picker into your app without writing platform-specific UI.
- **Code**:
  ```csharp
  var contact = await Contacts.Default.PickContactAsync();
  if (contact is not null)
  {
      SelectedContact.Text = $"{contact.NameGiven} {contact.NameFamily}";
  }
  ```
- **Tips**: Request read permissions up front on Android 14+, and fall back to manual entry if the picker returns `null`.

### 4.2 Email[^email]

- **Why**: Compose messages in the platform mail client with attachments.
- **Code**:
  ```csharp
  var message = new EmailMessage
  {
      Subject = "Quarterly status",
      Body = _reportBuilder.BuildBody(),
      BodyFormat = EmailBodyFormat.Html,
  };
  message.To.Add("sponsor@contoso.com");

  await Email.Default.ComposeAsync(message);
  ```
- **Tips**: Skip attachments larger than 10 MB or compress them before calling `ComposeAsync` to avoid failures on iOS.

### 4.3 Phone dialer[^phone-dialer]

- **Why**: Launch the native dialer or initiate a call with one line of code.
- **Code**:
  ```csharp
  if (PhoneDialer.Default.IsSupported)
  {
      PhoneDialer.Default.Open("+15551234567");
  }
  ```
- **Tips**: Validate phone numbers against E.164 formatting before dialing to ensure cross-country compatibility.

### 4.4 SMS[^sms]

- **Why**: Seed a message thread with predefined recipients and text.
- **Code**:
  ```csharp
  var message = new SmsMessage
  {
      Body = "Need assistance with order 12345.",
      Recipients = { "+15551234567" }
  };

  await Sms.Default.ComposeAsync(message);
  ```
- **Tips**: Handle the case where the user cancels the composer; Essentials does not indicate delivery success.

### 4.5 Web authenticator[^webauth]

- **Why**: Complete OAuth/OpenID Connect flows using the system browser for security and SSO.
- **Code**:
  ```csharp
  var result = await WebAuthenticator.Default.AuthenticateAsync(new WebAuthenticatorOptions
  {
      Url = new Uri(_authConfig.AuthorizeUrl),
      CallbackUrl = new Uri("mauiapp://auth-callback")
  });

  var accessToken = result?.AccessToken;
  ```
- **Tips**: Pair Essentials with `SecureStorage` to persist tokens and refresh them silently in background jobs.
## 5. Data transfer

### 5.1 Clipboard[^clipboard]

- **Why**: Exchange small bits of text or URIs between apps.
- **Code**:
  ```csharp
  await Clipboard.Default.SetTextAsync(_jsonSerializer.Serialize(snapshot));

  if (Clipboard.Default.HasText)
  {
      var text = await Clipboard.Default.GetTextAsync();
      ClipboardPreview.Text = text;
  }
  ```
- **Tips**: On iOS 16+, set `UIPasteboard.WithLocalOnly()` if you store sensitive data to prevent cross-device sync.

### 5.2 Share[^share]

- **Why**: Hand off files or rich text to the system share sheet.
- **Code**:
  ```csharp
  var exportPath = Path.Combine(FileSystem.CacheDirectory, "invoice.pdf");
  await _pdfExporter.WriteAsync(invoice, exportPath);

  await Share.Default.RequestAsync(new ShareFileRequest
  {
      Title = "Send invoice",
      File = new ShareFile(exportPath)
  });
  ```
- **Tips**: Combine `ShareTextRequest` with `ShareFileRequest` when sharing both text and documents to messaging apps.
## 6. Device & power

### 6.1 Battery[^battery]

- **Why**: React to charging state or low-battery conditions to defer expensive work.
- **Code**:
  ```csharp
  Battery.Default.BatteryInfoChanged += (_, args) =>
  {
      BatteryLevel.Text = $"{args.ChargeLevel:P0}";
      BatteryState.Text = args.State.ToString();
  };
  ```
- **Tips**: On Android 14+ combine Essentials events with `JobScheduler` constraints to pause background sync under low power.

### 6.2 Connectivity[^connectivity]

- **Why**: Detect online/offline transitions and the active connection profiles.
- **Code**:
  ```csharp
  var access = Connectivity.Current.NetworkAccess;
  var profiles = Connectivity.Current.ConnectionProfiles;

  Connectivity.ConnectivityChanged += (_, args) =>
  {
      ConnectivityStatus.Text = args.NetworkAccess.ToString();
  };
  ```
- **Tips**: Treat `NetworkAccess.ConstrainedInternet` as offline for large uploads; throttle retries with exponential backoff.

### 6.3 Device info[^device-info]
- **Why**: Adapt UX based on device idiom, platform, and manufacturer details.
- **Code**:
  ```csharp
  var metrics = new
  {
      DeviceInfo.Platform,
      DeviceInfo.Idiom,
      DeviceInfo.Model,
      DeviceInfo.Manufacturer
  };
  ```
- **Tips**: Log anonymized device info alongside crash reports to spot vendor-specific issues.

### 6.4 Device display[^device-display]
- **Why**: Respond to orientation or pixel density changes.
- **Code**:
  ```csharp
  var display = DeviceDisplay.Current.MainDisplayInfo;
  ResolutionLabel.Text = $"{display.Width}x{display.Height} @ {display.Density:F1}x";
  ```
- **Tips**: Subscribe to `DeviceDisplay.MainDisplayInfoChanged` for responsive layouts on foldables.

### 6.5 App theme resources[^apptheme]

- **Why**: Coordinate color schemes with the system theme.
- **Code**:
  ```csharp
  var theme = Application.Current.RequestedTheme;
  _analytics.Track("requested_theme", theme.ToString());
  ```
- **Tips**: Bind `Application.Current.UserAppTheme` to a `Preferences` value so users can override system settings while keeping a fallback.

### 6.6 Flashlight[^flashlight]

- **Why**: Toggle the device torch for scanning or emergency modes.
- **Code**:
  ```csharp
  await Flashlight.Default.TurnOnAsync();
  ```
- **Tips**: Wrap calls in try/catch; hardware without a torch will throw `FeatureNotSupportedException`.

### 6.7 Haptic feedback[^haptics]

- **Why**: Provide tactile responses to critical interactions.
- **Code**:
  ```csharp
  HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
  ```
- **Tips**: Pair haptics with audio cues and disable them in accessibility “reduce motion” modes.

### 6.8 Vibration[^vibration]

- **Why**: Alert users for non-visual notifications.
- **Code**:
  ```csharp
  Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(300));
  ```
- **Tips**: Stop vibrations explicitly via `Vibration.Default.Cancel()` when the alert is acknowledged.
## 7. Sensors suite

Essentials exposes hardware sensors behind simple start/stop APIs so you can add motion, orientation, and ambient awareness without platform renderers.[^accelerometer][^barometer][^compass][^gyro][^magnetometer][^orientation]

| Sensor | Key API | Typical use |
| --- | --- | --- |
| Accelerometer | `Accelerometer.Default.Start` | Gesture detection, shake-to-refresh |
| Barometer | `Barometer.Default.Start` | Altitude estimation, weather hints |
| Compass | `Compass.Default.Start` | Outdoor navigation, AR alignment |
| Gyroscope | `Gyroscope.Default.Start` | Motion tracking, gaming |
| Magnetometer | `Magnetometer.Default.Start` | Heading calibration, anomaly detection |
| Orientation | `OrientationSensor.Default.Start` | Mixed reality overlays, camera stabilization |

```csharp
Accelerometer.Default.ReadingChanged += (_, args) =>
{
    AccelLabel.Text = $"{args.Acceleration.X:F2}, {args.Acceleration.Y:F2}, {args.Acceleration.Z:F2}";
};
Accelerometer.Default.Start(SensorSpeed.Game);

Barometer.Default.ReadingChanged += (_, args) =>
{
    PressureLabel.Text = $"{args.PressureInHectopascals:F1} hPa";
};
Barometer.Default.Start(SensorSpeed.Default);

Compass.Default.ReadingChanged += (_, args) =>
{
    CompassLabel.Text = $"{args.Reading.HeadingMagneticNorth:F0}°";
};
Compass.Default.Start(SensorSpeed.Default);

Gyroscope.Default.ReadingChanged += (_, args) =>
{
    GyroLabel.Text = $"{args.AngularVelocity.X:F2}";
};
Gyroscope.Default.Start(SensorSpeed.Default);

Magnetometer.Default.ReadingChanged += (_, args) =>
{
    MagnetLabel.Text = $"{args.MagneticField.X:F2}";
};
Magnetometer.Default.Start(SensorSpeed.Default);

OrientationSensor.Default.ReadingChanged += (_, args) =>
{
    OrientationLabel.Text = args.Orientation.ToEulerAngles().ToString();
};
OrientationSensor.Default.Start(SensorSpeed.UI);
```

> **Tip:** Always call the corresponding `Stop` method in `OnDisappearing`/`Dispose` to conserve battery and release hardware locks on Android.

### 7.1 Shake detection[^detect-shake]

- **Why**: Offer rapid responses to shake gestures (e.g., undo, feedback, panic) without polling accelerometer values yourself.
- **Code**:
  ```csharp
  Accelerometer.Default.ShakeDetected += (_, _) =>
  {
      MainThread.BeginInvokeOnMainThread(() =>
      {
          _shakeTelemetry.Track("shake-detected");
          UndoLastAction();
      });
  };

  Accelerometer.Default.Start(SensorSpeed.Game);
  ```
- **Tips**: Use throttling (e.g., `Debounce` or a `DateTime` guard) to avoid repeated triggers on a single shake, and stop listening while modal dialogs are open to prevent accidental submissions.
## 8. Location stack

### 8.1 Geolocation[^geolocation]

- **Why**: Obtain the device latitude/longitude with accuracy and timeout control.
- **Code**:
  ```csharp
  var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
  {
      DesiredAccuracy = GeolocationAccuracy.High,
      Timeout = TimeSpan.FromSeconds(10)
  });

  if (location is not null)
  {
      await _mapService.PlotAsync(location.Latitude, location.Longitude);
  }
  ```
- **Tips**: Use `GetLastKnownLocationAsync` first to deliver instant results while waiting for a fresh reading.

### 8.2 Geocoding[^geocoding]

- **Why**: Translate addresses to coordinates and vice versa.
- **Code**:
  ```csharp
  var placemarks = await Geocoding.Default.GetPlacemarksAsync("1600 Amphitheatre Parkway, Mountain View, CA");
  var placemark = placemarks?.FirstOrDefault();
  ```
- **Tips**: Cache geocoding results—providers rate-limit repeated requests within 24 hours.

### 8.3 Maps[^maps]

- **Why**: Launch the system map app to provide turn-by-turn navigation from your MAUI UI.
- **Code**:
  ```csharp
  await Map.Default.OpenAsync(44.8196, 20.4600, new MapLaunchOptions
  {
      Name = "Prodyum HQ",
      NavigationMode = NavigationMode.Driving
  });
  ```
- **Tips**: Provide both coordinates and a name; Android uses the label when coordinates fall outside recognized roads.
## 9. Media & voice

### 9.1 Media picker[^mediapicker]

- **Why**: Allow users to capture or select photos and videos.
- **Code**:
  ```csharp
  var photo = await MediaPicker.Default.CapturePhotoAsync();
  if (photo is not null)
  {
      await using var stream = await photo.OpenReadAsync();
      await _imageStore.SaveAsync(stream);
  }
  ```
- **Tips**: Set `MediaPickerOptions` with `Title` to provide localized guidance per platform. Android 13+ routes `Pick*` calls through the system Photo Picker, giving users partial access controls and multi-select without your app requesting broad media permissions.[^android-photo-picker]

### 9.2 Screenshot[^screenshot]

- **Why**: Capture the current view hierarchy for diagnostics or sharing.
- **Code**:
  ```csharp
  var screenshot = await Screenshot.Default.CaptureAsync();
  await using var stream = await screenshot.OpenReadAsync();
  await _blobStorage.UploadAsync(stream, "screenshots/session-42.png");
  ```
- **Tips**: Avoid capturing sensitive content; warn users before uploading screenshots to support systems.

### 9.3 Text-to-speech[^tts]

- **Why**: Produce spoken feedback for accessibility or assistant experiences.
- **Code**:
  ```csharp
  await TextToSpeech.Default.SpeakAsync("Your pickup is ready.", new SpeechOptions
  {
      Pitch = 0.9f,
      Volume = 0.8f
  });
  ```
- **Tips**: Query `TextToSpeech.Default.GetLocalesAsync()` to respect the user’s preferred language.

### 9.4 Unit converters[^unit-converters]

- **Why**: Convert between measurement units without reimplementing formulas.
- **Code**:
  ```csharp
  var miles = UnitConverters.KilometersToMiles(5.0);
  var celsius = UnitConverters.FahrenheitToCelsius(70);
  ```
- **Tips**: Combine with `Preferences` so users can pick their preferred units once.
## 10. Storage & preferences

### 10.1 File picker[^filepicker]

- **Why**: Let users choose files from the OS picker.
- **Code**:
  ```csharp
  var file = await FilePicker.Default.PickAsync(new PickOptions
  {
      PickerTitle = "Select the import sheet",
      FileTypes = FilePickerFileType.Spreadsheet
  });

  if (file is not null)
  {
      await using var stream = await file.OpenReadAsync();
      await _importer.RunAsync(stream);
  }
  ```
- **Tips**: Provide filtered `FilePickerFileType` to reduce surprises—Android returns `content://` URIs when multiple apps can serve the MIME type.

### 10.2 File system helpers[^filesystem]

- **Why**: Read and write app data in a cross-platform location.
- **Code**:
  ```csharp
  var path = Path.Combine(FileSystem.AppDataDirectory, "settings.json");
  await File.WriteAllTextAsync(path, JsonSerializer.Serialize(settings));
  ```
- **Tips**: Use `FileSystem.CacheDirectory` for temporary data that can be purged by the OS under pressure.

### 10.3 Preferences[^preferences]

- **Why**: Store small pieces of state like flags or tokens.
- **Code**:
  ```csharp
  Preferences.Default.Set("theme", "dark");
  var theme = Preferences.Default.Get("theme", "light");
  ```
- **Tips**: Namescope keys (e.g., `"featureA:welcomeComplete"`) to avoid collisions across features.

### 10.4 Secure storage[^secure-storage]

- **Why**: Persist secrets (tokens, keys) encrypted with platform keychains.
- **Code**:
  ```csharp
  await SecureStorage.Default.SetAsync("auth_token", accessToken);
  var token = await SecureStorage.Default.GetAsync("auth_token");
  ```
- **Tips**: Handle `FeatureNotSupportedException` on devices without secure enclave hardware and fallback to brokered authentication.
## 11. Converters & platform helpers

### 11.1 Color converters[^color-converters]

- **Why**: Translate brand palettes and telemetry values between hex, RGB, and HSL without writing custom parsers.
- **Code**:
  ```csharp
  var accent = ColorConverters.FromHex("#512BD4");
  var (h, s, l) = ColorConverters.ToHsl(accent);

  var contrast = l < 0.5
      ? ColorConverters.FromHsl(h, s * 0.8, Math.Min(l + 0.35, 1))
      : Colors.Black;

  ThemePreview.BackgroundColor = accent;
  ThemePreview.TextColor = contrast;
  ```
- **Tips**: Cache converted colors in a `Dictionary<string, Color>` when rendering large lists (e.g., analytics dashboards) to avoid repeated conversions.

### 11.2 Platform extensions[^platform-extensions]

- **Why**: Reach native platform types (Android `Activity`, iOS `UIViewController`, Windows `Window`) from shared code when Essentials abstractions are not enough.
- **Code**:
  ```csharp
#if ANDROID
using Microsoft.Maui.ApplicationModel;

var metrics = Platform.CurrentActivity?
    .WindowManager?
    .CurrentWindowMetrics;
#elif IOS || MACCATALYST
using UIKit;
using Microsoft.Maui.ApplicationModel;

var controller = Platform.GetCurrentUIViewController();
controller?.PresentViewController(new UIAlertController
{
    Title = "Platform bridge",
    Message = "Invoked from Essentials Platform extensions."
}, true, null);
#endif
  ```
- **Tips**: Keep platform conditionals isolated in partial classes so shared code stays testable; prefer Essentials APIs first and fall back to platform extensions only for advanced scenarios (e.g., window metrics, status bar tweaks).

## 12. Testing Essentials features

- **Unit tests**: Wrap static APIs behind interfaces; inject the wrappers. Use dependency injection to replace implementations with fakes in tests.
- **UI tests**: Leverage Appium or Playwright for MAUI. For sensors, add simulator overrides (e.g., `VirtualLocationManager` on iOS simulators).
- **Permissions**: Run automated test suites that simulate denied permissions by toggling platform simulator settings.

## 13. Pre-Essentials landscape (Xamarin.Forms era)

Before Xamarin.Essentials (2018), Xamarin.Forms developers accessed device functionality through:

- **DependencyService**: Define an interface in shared code and register platform implementations via `[assembly: Dependency]`.
- **Plugins**: Community NuGet packages (e.g., `Plugin.Geolocator`, `Plugin.Connectivity`) wrapping platform APIs.
- **Custom renderers/effects**: Create a Forms control, then implement per-platform subclasses to reach native APIs.
- **MessagingCenter**: Publish/subscribe between shared and platform code to bridge events.

### 13.1 Legacy example: Geolocation via DependencyService

```csharp
// Shared project
public interface ILocationProvider
{
    Task<Location> GetLocationAsync();
}

// Android implementation
[assembly: Dependency(typeof(DroidLocationProvider))]
public sealed class DroidLocationProvider : ILocationProvider
{
    public async Task<Location> GetLocationAsync()
    {
        var fused = LocationServices.GetFusedLocationProviderClient(MainActivity.Instance);
        var androidLocation = await fused.GetLastLocationAsync();
        return new Location(androidLocation.Latitude, androidLocation.Longitude);
    }
}
```

### 13.2 Pain points Essentials eliminated

| Scenario | Pre-Essentials workaround | Essentials replacement | Migration tip |
| --- | --- | --- | --- |
| Connectivity checks | `Plugin.Connectivity` + manual event bridging | `Connectivity.Current.NetworkAccess` | Replace plugin usage directly; event semantics align. |
| Secure storage | `Keychain`/`Keystore` bindings per platform | `SecureStorage.Default` | Remove custom renderers; re-use existing encryption seeds. |
| Accelerometer | `SensorManager` (Android) + `CMMotionManager` (iOS) | `Accelerometer.Default` | Essentials normalizes units (g-force). |
| Maps launch | Platform intents/URL schemes | `Map.OpenAsync` | Validate package IDs; on Android 11+ add query intents. |

### 13.3 Migration checklist

1. **Audit NuGet dependencies**: Replace plugins duplicated by Essentials (battery, clipboard, share, etc.).
2. **Refactor DependencyService usage**: Inject Essentials wrappers via MAUI DI for easier testing.
3. **Handle permission flows**: Remove manual permission prompts if Essentials already requests them; keep custom prompts if needed for UX.
4. **Update callbacks**: Essentials uses async APIs; adapt old synchronous calls to `await`.
5. **Platform initializer**: Delete obsolete `Platform.Init` calls in `MainActivity`/`AppDelegate`; MAUI templates wire Essentials automatically.

## 14. Troubleshooting on .NET 9

- **Xcode 16 & iOS 18**: Ensure macOS build agents run Xcode 16.1+; install corresponding `dotnet workload install maui` patch updates.
- **Android 15 (API 35)**: Debugging on older Samsung devices may require enabling developer previews or sticking to .NET 8; consider multislot CI builds.
- **Blazor hybrid + iOS 16**: Blazor WebView in .NET 9 targets modern Safari engines; older iOS browsers may fail to execute ES2023 features. Offer fallback UI or require minimum OS updates.
- **Community Toolkit alignment**: Update to .NET MAUI Community Toolkit 11.x for .NET 9 compatibility, especially when using `MediaElement`, `CameraView`, or `Snackbar`.

## 15. From zero to hero learning path

1. **Essentials API tour**: Follow a workshop building an all-in-one "Device Lab" page showcasing sensors, connectivity, media, and storage.
2. **Offline-first scenario**: Combine `Connectivity`, `Preferences`, and `SecureStorage` to implement resilient sync flows.
3. **Enterprise integration**: Use `WebAuthenticator` together with `SecureStorage` and `HttpClient` to call protected ASP.NET Core APIs.
4. **Platform enhancements**: Leverage Essentials in background services (e.g., `Geolocation` + `TextToSpeech` for fitness apps) using MAUI `IHostedService`.

## 16. Reference checklist

- App actions, App info, Browser, Launcher, Main thread, Permissions, Version tracking
- Contacts, Email, Phone dialer, SMS, Web authenticator
- Clipboard, Share
- Battery, Connectivity, Device info, Device display, App theme, Flashlight, Haptic feedback, Vibration
- Accelerometer, Barometer, Compass, Gyroscope, Magnetometer, Orientation sensor
- Geolocation, Geocoding, Maps
- Media picker, Screenshot, Text-to-speech, Unit converters
- File picker, File system helpers, Preferences, Secure storage
- Color converters, Platform extensions

## 17. References

[^platform-overview]: Microsoft Learn, "Platform integration features in .NET MAUI," accessed November 1, 2025. citeturn8search0
[^essentials-package]: Microsoft Learn, ".NET MAUI Essentials overview," accessed November 1, 2025. citeturn0search3
[^use-maui-essentials]: Microsoft Learn, "Incrementally adopt .NET MAUI features into your app," accessed November 1, 2025. citeturn1search0
[^app-actions]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^app-info]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^browser]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^launcher]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^main-thread]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^permissions]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^version-tracking]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^contacts]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^email]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^phone-dialer]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^sms]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^webauth]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^clipboard]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^share]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^battery]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^connectivity]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^device-info]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^device-display]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^apptheme]: Microsoft Learn, "Apply dynamic app theme resources," accessed November 1, 2025. citeturn7search0
[^flashlight]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^haptics]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^vibration]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^accelerometer]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^detect-shake]: Microsoft Learn, "Detect shake gestures with Accelerometer.ShakeDetected," accessed November 1, 2025. citeturn12search0
[^barometer]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^compass]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^gyro]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^magnetometer]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^orientation]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^geolocation]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^geocoding]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^maps]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^mediapicker]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^android-photo-picker]: Android Developers, "Photo Picker overview," updated October 24, 2024. citeturn6search1
[^screenshot]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^tts]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^unit-converters]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^filepicker]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^filesystem]: Microsoft Learn, "File system helpers - .NET MAUI," accessed November 1, 2025. citeturn22search0
[^preferences]: Microsoft Learn, "Application settings management in .NET MAUI," accessed November 1, 2025. citeturn0search1
[^secure-storage]: Microsoft Learn, "Secure storage - .NET MAUI," accessed November 1, 2025. citeturn20search0
[^color-converters]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
[^platform-extensions]: Microsoft Learn, ".NET MAUI platform integration sample," April 15, 2025. citeturn6view0
