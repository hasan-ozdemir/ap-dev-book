---
title: iOS Platform SDK Playbook
description: Harness the full Apple SDK surface from .NET MAUI using platform APIs, native frameworks, and production packaging.
last_reviewed: 2025-11-03
owners:
  - @prodyum/maui-guild
---

# iOS Platform SDK Playbook

Apple’s mobile stack exposes rich capabilities—UIKit, CoreLocation, CoreMotion, UserNotifications—that you can call directly from .NET MAUI. Microsoft’s guidance emphasises understanding the multi-targeted project layout, Info.plist metadata, and entitlements so cross-platform code still feels native on iPhone and iPad.citeturn0search0turn4search0

## 1. Architecture & build targets

- **Multi-targeting:** The MAUI project produces a `net9.0-ios` build that links against Xamarin.iOS bindings for Apple’s SDKs. Shared code lives under `Platforms/iOS` alongside platform resources (storyboards, asset catalogues, entitlements).citeturn0search0
- **Platform behaviour tweaks:** Use `builder.ConfigureMauiHandlers` plus platform-specific APIs (e.g., `UIKit.UIScrollView`) to align controls with Cupertino UX expectations. Conditional compilation (`#if IOS`) keeps platform logic isolated.citeturn4search0turn0search8
- **Packaging:** App Store builds require Release configuration, device-specific provisioning profiles, and a signed `dotnet publish -f net9.0-ios -c Release` output (simulator builds use `-f net9.0-ios14.2`).citeturn5search4

## 2. SDK feature map

| Capability | Key namespaces & types | Usage highlights |
|------------|-----------------------|------------------|
| Application lifecycle | `UIKit.UIApplication`, `Microsoft.Maui.LifecycleEvents.iOS` | Hook into foreground/background transitions, SceneDelegate notifications.citeturn0search0turn4search0 |
| Device services | `Microsoft.Maui.ApplicationModel` (`Battery`, `Connectivity`, `Flashlight`) | Access sensors and system info with unified APIs backed by native iOS implementations.citeturn4search5 |
| Permissions | `Microsoft.Maui.ApplicationModel.Permissions`, `UserNotifications.UNUserNotificationCenter` | Request capability access (location, notifications) and reconcile Info.plist entitlements.citeturn4search5turn0search0 |
| UI composition | `UIKit.UIViewController`, `UIKit.UIWindowScene`, `Microsoft.Maui.Platform.PlatformView` | Embed native view controllers or customise handlers for MAUI controls.citeturn4search0turn0search8 |
| Background tasks | `Foundation.NSUrlSession`, `BackgroundTasks.BGTaskScheduler` | Implement fetch, refresh, or processing tasks with native schedulers.citeturn0search0 |
| Notifications & widgets | `UserNotifications`, `WidgetKit` (via binding libraries) | Ship push/local notifications and Today widgets with extension projects.citeturn0search0 |

## 3. Accessing native APIs from shared code

Use dependency injection or partial classes to expose native functionality without polluting shared view models.

```csharp
public interface IHapticService
{
    void Pulse();
}

#if IOS
using Foundation;
using UIKit;

[Register(nameof(IosHapticService))]
public sealed class IosHapticService : NSObject, IHapticService
{
    public void Pulse()
    {
        using var generator = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Medium);
        generator.Prepare();
        generator.ImpactOccurred();
    }
}
#endif
```

```csharp
// MauiProgram.cs
builder.Services.AddSingleton<IHapticService, IosHapticService>();
```

This approach keeps shared layers platform-agnostic while giving iOS users tactile feedback consistent with native apps.citeturn4search0

## 4. Feature playbooks

### 4.1 Location & sensors

Combine `Geolocation.Default.GetLocationAsync()` with iOS-specific accuracy controls. Ensure `NSLocationWhenInUseUsageDescription` is present in Info.plist before requesting permissions.citeturn4search5turn0search0

```csharp
var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
var location = await Geolocation.Default.GetLocationAsync(request);
```

### 4.2 Notifications

Register notification categories during startup and bridge to `UNUserNotificationCenter` for advanced scheduling.

```csharp
#if IOS
await UNUserNotificationCenter.Current.RequestAuthorizationAsync(
    UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound);
#endif
```

Add entitlements for push (APS) and update provisioning profiles prior to App Store submission.citeturn0search0turn5search4

### 4.3 App extensions & widgets

Create additional headless MAUI class libraries targeting `net9.0-ios` and reference binding libraries for `WidgetKit` to deliver lock-screen or home-screen experiences. Coordinate data via shared `AppGroup` containers.citeturn0search0

## 5. Build, debug, and distribute

1. **Debug:** Use Visual Studio’s Hot Reload against iOS simulators or physical devices registered with your developer account.citeturn0search0
2. **Archive:** `dotnet publish -f net9.0-ios -c Release /p:CodesignKey="Apple Distribution: ..." /p:CodesignProvision="MyProfile"`.citeturn5search4
3. **Validate:** Run `altool`/`notarytool` or Xcode Transporter to ensure bundle identifiers, entitlements, and Info.plist keys pass Apple’s notarisation tests before App Store Connect submission.citeturn5search4

## 6. Readiness checklist

- [ ] Info.plist includes permission usage strings and required background modes.citeturn0search0
- [ ] Native dependencies (Swift, Objective-C bindings) are embedded via `NativeReference` and tested on arm64 devices.citeturn4search0
- [ ] Release builds run through TestFlight with instrumentation to monitor crashes and startup metrics.citeturn5search4turn0search0

