---
title: Windows Platform SDK Playbook
description: Use WinUI 3, Windows App SDK, and desktop-specific capabilities from .NET MAUI apps.
last_reviewed: 2025-11-03
owners:
  - @prodyum/maui-guild
---

# Windows Platform SDK Playbook

When you target Windows, .NET MAUI hosts your app inside a WinUI 3 shell supplied by the Windows App SDK. You can surface desktop-specific features—menu bars, system notifications, multi-window experiences—while keeping a single shared codebase.citeturn0search2turn0search6turn0search8

## 1. Architecture & project layout

- **Target framework:** Windows builds use `net9.0-windows10.0.19041.0`, which maps to WinUI 3 and runs on Windows 10 version 1809 and Windows 11.citeturn0search2turn0search6
- **Platform assets:** The `Platforms/Windows` folder contains the WinUI host (`App.xaml.cs`, `MainWindow.xaml`), app manifest (AppxManifest), and MSIX packaging metadata.citeturn0search2
- **Native access:** Call Windows Runtime (WinRT) APIs directly or wire desktop features via dependency injection, using `Microsoft.UI.Xaml` controls and `Windows.*` namespaces.citeturn0search3turn4search0

## 2. SDK feature map

| Capability | Namespaces & types | Scenario |
|------------|-------------------|----------|
| Windowing & menus | `Microsoft.UI.Windowing`, `MenuBarItem`, `ToolBarTray` | Multi-window desktop layouts, menu bars, accelerators.citeturn0search2turn0search8 |
| Notifications & tiles | `Windows.UI.Notifications.ToastNotificationManager` | Toasts, badges, notification history integration.citeturn0search7 |
| File system & Storage | `Windows.Storage`, `Microsoft.Maui.Storage.FileSystem` | Access KnownFolders, open/save pickers, AppData directories.citeturn0search3 |
| Input & pen | `Windows.Devices.Input`, `Microsoft.UI.Input` | Pointer, pen, and keyboard enhancements for desktop scenarios.citeturn0search2 |
| Composition & graphics | `Microsoft.UI.Composition`, `Win2D` via Windows Community Toolkit | Rich visuals, effects, and animations layered over WinUI surfaces.citeturn0search5turn0search6 |

## 3. Accessing native APIs

Wrap Windows Runtime APIs in services to maintain clean boundaries.

```csharp
public interface IToastService
{
    void Show(string title, string message);
}

#if WINDOWS
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

public sealed class WindowsToastService : IToastService
{
    public void Show(string title, string message)
    {
        var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
        var texts = toastXml.GetElementsByTagName("text");
        texts[0].AppendChild(toastXml.CreateTextNode(title));
        texts[1].AppendChild(toastXml.CreateTextNode(message));

        var toast = new ToastNotification(toastXml);
        ToastNotificationManager.CreateToastNotifier().Show(toast);
    }
}
#endif
```

Register the implementation in `MauiProgram` to keep view models platform-agnostic.citeturn0search7turn4search0

## 4. Feature playbooks

### 4.1 Desktop navigation & menu bars

Define menu hierarchies via `MenuBarItem` collections in XAML or code to match Windows productivity patterns. Attach commands to support keyboard accelerators and access keys for power users.citeturn0search8turn0search2

### 4.2 Window management

Use `AppWindow` and `DisplayArea` APIs to handle multi-monitor layouts, snapping, and compact overlays. Ensure your app handles DPI scaling and high-contrast themes via WinUI styling resources.citeturn0search2turn0search6

### 4.3 Integrating Windows Community Toolkit

Add the `CommunityToolkit.WinUI` NuGet package for helpers (e.g., `ObservableObject`, `HeroImage`, `TabView`) that extend WinUI 3 functionality and accelerate desktop UX requirements.citeturn0search5

## 5. Build, package, and distribute

1. **Debug:** Run `dotnet run -f net9.0-windows10.0.19041.0` or press F5 in Visual Studio to launch the WinUI host window. Hot Reload updates XAML/CS markup instantly.citeturn0search2turn0search7
2. **Package:** `dotnet publish -f net9.0-windows10.0.19041.0 -c Release /p:ApplicationPackages=true` produces a signed MSIX bundle in `AppPackages`.citeturn0search7turn5search4
3. **Distribute:** Submit the MSIX to the Microsoft Store or side-load with `Add-AppxPackage`. Validate signing certificates and capabilities (e.g., `ToastCapable`) before rollout.citeturn0search7turn5search4

## 6. Readiness checklist

- [ ] App manifest declares capabilities (notifications, background tasks) and minimum OS version 17763.citeturn0search2turn0search7
- [ ] Desktop-specific services (toasts, file pickers) abstracted behind interfaces and tested on multi-monitor setups.citeturn0search3turn0search7
- [ ] Release packages signed, notarised, and validated via App Installer or Store ingestion pipelines.citeturn5search4turn0search7

