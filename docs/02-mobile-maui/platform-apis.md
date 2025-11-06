---
title: Platform Integration & Device APIs
description: Call iOS, Android, Windows, and Mac Catalyst SDK features from .NET MAUI apps using handlers, partial classes, and Essentials.
last_reviewed: 2025-11-03
owners:
  - @prodyum/maui-guild
---

# Platform Integration & Device APIs

.NET MAUI exposes multiple strategies to call native SDK features—Essentials abstractions, platform-specific folders, conditional compilation, and handler customisations—so you retain 100% access to iOS, Android, Mac Catalyst, and Windows APIs from shared C# code.citeturn1search0turn1search6turn1search8 With Xamarin.Essentials now integrated into MAUI, most device sensors and OS capabilities are available via namespaces such as `Microsoft.Maui.Devices`, `Microsoft.Maui.ApplicationModel`, and `Microsoft.Maui.Storage`.citeturn1search1turn1search7

## Patterns for platform code

| Pattern | Use it when | Notes |
| --- | --- | --- |
| **Essentials APIs** | Feature exists in Essentials (geolocation, secure storage, app info). | Cross-platform, prefers permission prompts via shared helpers.citeturn1search1turn1search7 |
| **Conditional compilation** (`#if ANDROID`) | Small tweaks or property updates per platform. | Keeps code in shared project; lean on `DeviceInfo.Current.Platform` when runtime checks suffice.citeturn1search6 |
| **Partial classes/partial methods** | You need native API surface but want shared façade. | Define partial methods in shared file, implement per platform file under `Platforms/{Platform}`.citeturn1search6 |
| **Handler mappers/custom handlers** | You must alter native view behaviour or substitute controls. | Handlers map virtual views to native views; use `Mapper.AppendToMapping` to access the native view.citeturn1search8turn1search10 |
| **Dependency injection (service per platform)** | Complex services (background sync, native libraries). | Register platform implementation in `MauiProgram` within `#if` blocks or platform projects.citeturn1search0turn1search6 |

## Example: vibration service with conditional compilation

```csharp
// File: Platforms/Shared/VibrationService.cs
namespace Contoso.Mobile.Platform;

public interface IVibrationService
{
    void Pulse(TimeSpan duration);
}

public partial class VibrationService : IVibrationService
{
    public partial void Pulse(TimeSpan duration);
}

// File: Platforms/Android/VibrationService.Android.cs
using Android.Content;
using Android.OS;

namespace Contoso.Mobile.Platform;

public partial class VibrationService
{
    public partial void Pulse(TimeSpan duration)
    {
        var vibrator = (Vibrator?)Application.Context.GetSystemService(Context.VibratorService);
        if (vibrator?.HasVibrator == true)
        {
            var effect = VibrationEffect.CreateOneShot((long)duration.TotalMilliseconds, VibrationEffect.DefaultAmplitude);
            vibrator.Vibrate(effect);
        }
    }
}

// File: Platforms/iOS/VibrationService.iOS.cs
using AudioToolbox;

namespace Contoso.Mobile.Platform;

public partial class VibrationService
{
    public partial void Pulse(TimeSpan duration)
    {
        SystemSound.Vibrate.PlaySystemSound();
    }
}

// MauiProgram.cs
builder.Services.AddSingleton<IVibrationService, VibrationService>();
```

- Shared partial classes provide a single DI registration while platform files target native APIs.citeturn1search6
- Registering the service in `MauiProgram` integrates with the MAUI dependency container, replacing legacy `DependencyService`.citeturn1search0turn1search6

## Example: handler mapper tweak

```csharp
// File: MauiProgram.cs
using Microsoft.Maui.Controls.Handlers.Compatibility;

builder.ConfigureMauiHandlers(handlers =>
{
    handlers.AddHandler(typeof(Button), typeof(ButtonHandler));
});

ButtonHandler.Mapper.AppendToMapping("PlatformCorners", (handler, view) =>
{
#if ANDROID
    handler.PlatformView.StateListAnimator = null;
#endif

#if IOS || MACCATALYST
    handler.PlatformView.Layer.CornerRadius = 12;
    handler.PlatformView.ClipsToBounds = true;
#endif
});
```

- Handler mappers expose the underlying native view during mapping so you can adjust per platform without leaving the shared project.citeturn1search8turn1search10
- Removing the Android `StateListAnimator` avoids the ripple elevation effect, while iOS tweaks adjust rounded corners consistently.

## Essentials namespaces at a glance

| Namespace | Purpose | Sample API |
| --- | --- | --- |
| `Microsoft.Maui.ApplicationModel` | App lifecycle, permissions, versioning. | `AppInfo.RequestedTheme`, `VersionTracking.Track()`.citeturn1search1turn1search7 |
| `Microsoft.Maui.Devices` | Device identity, sensors, idiom detection. | `DeviceInfo.Current`, `Vibration.Default`.citeturn1search6turn1search7 |
| `Microsoft.Maui.Media` | Camera, microphone, screen capture. | `MediaPicker.Default.CapturePhotoAsync()`.citeturn1search7 |
| `Microsoft.Maui.Storage` | File pickers, secure storage, preferences. | `SecureStorage.Default.SetAsync(key, value)`.citeturn1search7 |

## Platform API checklist

- Document every platform hook (permissions, entitlements, manifest entries) in the Platform SDK guides to avoid missing store requirements.
- Add linker keep attributes (`[assembly: LinkerSafe]`) when platform-specific types are only referenced via reflection or DI.
- Wrap native interop in resilience (exception handling, logging) so failure on one OS does not crash the app.
- Keep device lab validation scripts (e.g., App Center Test, BrowserStack) updated for each platform-specific feature before release gating.

---

Continue with the platform-specific deep dives:

- [iOS Platform SDK Playbook](./platform-sdk/ios-sdk-guide.md)
- [Android Platform SDK Playbook](./platform-sdk/android-sdk-guide.md)
- [Windows Platform SDK Playbook](./platform-sdk/windows-sdk-guide.md)
