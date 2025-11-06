---
title: Android Platform SDK Playbook
description: Leverage AndroidX, Google services, and platform APIs from .NET MAUI with production-ready guidance.
last_reviewed: 2025-11-03
owners:
  - @prodyum/maui-guild
---

# Android Platform SDK Playbook

.NET MAUI ships a first-class Android experience built on top of AndroidX, Gradle tooling, and Google Play services. Understanding project multi-targeting, manifest configuration, and how MAUI handlers map to native views is essential for delivering performant Android apps.citeturn0search1turn4search0

## 1. Architecture & build targets

- **Target framework:** Android builds compile to `net9.0-android` using the Mono ahead-of-time toolchain with support for arm64, x86_64, and emulator runtimes.citeturn0search1
- **Project layout:** Platform resources (AndroidManifest.xml, styles, drawables) live under `Platforms/Android`. Configure Gradle packaging options, permissions, and intent filters directly in the manifest or via MSBuild item metadata.citeturn0search1
- **Handlers & renderers:** MAUI handlers map to Android views (e.g., `ButtonHandler.Mapper.AppendToMapping` against `Android.Widget.Button`) so you can tweak native properties with conditional compilation.citeturn4search0turn0search8
- **Packaging:** Use `dotnet publish -f net9.0-android -c Release` to produce signed `.aab` bundles. Configure `AndroidPackageFormat=aab`, keystore credentials, and Play Console tracks for rollout.citeturn5search4

## 2. SDK feature map

| Capability | Namespaces & types | Scenario |
|------------|-------------------|----------|
| Lifecycle hooks | `Microsoft.Maui.LifecycleEvents.Android`, `Android.App.Application` | Observe `OnCreate`, `OnActivityResult`, and resume events for analytics or deep links.citeturn0search1turn4search0 |
| Device services | `Microsoft.Maui.Devices` (`Vibration`, `Flashlight`, `DeviceDisplay`) | Access sensors with cross-platform APIs backed by Android implementations.citeturn4search5 |
| Permissions | `Microsoft.Maui.ApplicationModel.Permissions`, `AndroidX.Core.Content.ContextCompat` | Request runtime permissions (camera, location, notifications) with fallback flows for API level ≥ 33.citeturn4search5turn0search1 |
| Background work | `AndroidX.Work.WorkManager`, `Android.App.JobScheduler` | Schedule resilient background sync or notifications.citeturn0search1 |
| Google Play services | `Android.Gms.Location.FusedLocationProviderClient`, `Firebase.Messaging.FirebaseMessaging` | Integrate location, analytics, or FCM push via bindings or NuGet packages.citeturn0search1 |

## 3. Accessing native APIs

Inject native functionality via platform partial classes or dependency injection.

```csharp
public interface IVibrationService
{
    void Vibrate(TimeSpan duration);
}

#if ANDROID
using Android.Content;
using Android.OS;

public sealed class AndroidVibrationService : IVibrationService
{
    private readonly VibratorManager _manager;

    public AndroidVibrationService(Context context) =>
        _manager = (VibratorManager)context.GetSystemService(Context.VibratorManagerService)!;

    public void Vibrate(TimeSpan duration) =>
        _manager.DefaultVibrator.Vibrate(VibrationEffect.CreateOneShot(duration.Milliseconds, VibrationEffect.DefaultAmplitude));
}
#endif
```

Register the service in `MauiProgram`:

```csharp
#if ANDROID
builder.Services.AddSingleton<IVibrationService>(sp =>
    new AndroidVibrationService(Android.App.Application.Context));
#endif
```

This keeps shared view models device-agnostic while providing tactile feedback aligned with Android UX guidelines.citeturn4search0

## 4. Feature playbooks

### 4.1 Camera & media capture

Use `MediaPicker.Default.CapturePhotoAsync()` for quick captures, or drop down to `AndroidX.Camera.Core` for fine-grained control. Declare `android.permission.CAMERA` and `android.permission.WRITE_EXTERNAL_STORAGE` (API < 33) in the manifest, and request runtime permissions before launching the camera intent.citeturn4search5turn0search1

### 4.2 Location & geofencing

Leverage `Geolocation.Default.GetLocationAsync()` for cross-platform access; for continuous tracking, bind to `FusedLocationProviderClient` and handle foreground service notifications to satisfy Android 14 background location policies.citeturn4search5turn0search1

### 4.3 Notifications & shortcuts

Configure notification channels, request POST_NOTIFICATIONS permission (API 33+), and use `FirebaseMessaging` for FCM push. Surface app shortcuts via `ShortcutManager`.citeturn0search1

## 5. Build, debug, and distribute

1. **Debug:** Use Android Device Manager or physical devices with USB debugging. Hot Reload accelerates layout verification.citeturn0search1
2. **Bundle:** `dotnet publish -f net9.0-android -c Release /p:AndroidPackageFormat=aab /p:AndroidKeyStore=true ...`.citeturn5search4
3. **Validate:** Run `bundletool build-apks` and Play Console pre-launch reports to catch unsupported hardware features before rollout.citeturn5search4

## 6. Readiness checklist

- [ ] Manifest declares permissions, intent filters, adaptive icons, and supports Android 14 (API 34) behaviour changes.citeturn0search1
- [ ] Background services (WorkManager, foreground services) include user-visible notifications and battery optimisation exceptions where needed.citeturn0search1
- [ ] Release builds enable R8/ProGuard trimming, configured via `AndroidLinkMode` plus custom keep rules for reflection-heavy libraries.citeturn5search4

