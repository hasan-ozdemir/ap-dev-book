---
title: Visual Studio Hot Restart Guide
description: Configure and use Hot Restart in Visual Studio 2022 to deploy .NET MAUI apps to iOS devices without a Mac.
last_reviewed: 2025-10-31
owners:
  - @prodyum/maui-guild
---

# Visual Studio 2022 Hot Restart Guide

Hot Restart lets you deploy and debug a .NET MAUI iOS build from a Windows machine by temporarily side-loading your app onto a USB-connected device. It is ideal for rapid UI iteration and smoke-testing flows before you have access to a Mac build host.

---

## 1. Prerequisites

- Visual Studio 2022 version 17.14 with the **.NET Multi-platform App UI** workload installed.citeturn1view0
- An active Apple Developer Program membership so Visual Studio can register signing certificates and provisioning profiles.citeturn1view0
- The Microsoft Store edition of iTunes, which supplies the Apple Mobile Device drivers that Hot Restart requires.citeturn1view0
- A 64-bit iOS device running a supported iOS release, connected over USB and unlocked.citeturn1view0

> ⚠️ Hot Restart accelerates inner-loop testing but does not replace Mac-based release builds for App Store distribution.citeturn1view0

---

## 2. One-time setup workflow

1. **Connect and trust the device**  
   Open iTunes to confirm the iPhone or iPad appears and tap **Trust** if prompted so Windows can detect it.citeturn1view0
2. **Launch the Hot Restart wizard**  
   In Visual Studio, select the iOS head project, choose **iOS Local Device**, and press **F5** to start the setup wizard.citeturn1view0
3. **Install supporting components**  
   Allow Visual Studio to download any required Apple Mobile Device support packages and provisioning helpers.citeturn1view0
4. **Authenticate with Apple**  
   Sign in with your Apple Developer credentials so the wizard can create signing assets.citeturn1view0
5. **Provision the device**  
   After provisioning completes, the target picker lists your device (for example, `iOS Local Device > <Your iPhone>`), indicating Hot Restart is ready.citeturn1view0

---

## 3. Iterating with Hot Restart

1. Keep **iOS Local Device** selected and run the app. Visual Studio deploys a lightweight shell bundle and injects your latest build.citeturn1view0
2. Launch the app manually the first time (iOS prompts for trust); afterwards deployments typically launch automatically with the debugger attached.citeturn1view0
3. Edit code or XAML and rebuild. Hot Restart reuses the installed bundle, so subsequent deployments complete in seconds.citeturn1view0

---

## 4. Known limitations

| Limitation | Impact | Mitigation |
| --- | --- | --- |
| Debug-only deployments | Hot Restart can launch only debug builds, so you cannot validate release-only options such as trimming or AOT.citeturn1view0 | Schedule regular Pair-to-Mac or CI builds to exercise release configurations. |
| Native libraries unsupported | Static libraries, frameworks, XCFrameworks, and binding resource packages do not load when running via Hot Restart.citeturn1view0 | Verify native components with Mac-based builds or provide managed fallbacks. |

---

## 5. Troubleshooting checklist

- **Device not listed** – Confirm the device appears in iTunes, unlock it, and tap **Trust** so Windows exposes it to Visual Studio.citeturn1view0
- **Watchdog terminates the app** – Reduce work on the startup path or test on newer hardware; older devices can be killed by iOS watchdogs before the debugger attaches.citeturn1view0
- **Report recurring issues** – Use **Help > Send Feedback > Report a Problem** in Visual Studio to share logs with Microsoft.citeturn1view0

---

## 6. Best practices for teams

- Capture device pairing steps (for example, installing iTunes from the Microsoft Store) in team onboarding guides so everyone can reproduce the setup quickly.citeturn1view0
- Plan Mac-based validation (TestFlight, App Store notarization, UI automation) even when Hot Restart covers daily development loops.citeturn1view0

> ℹ️ No public Microsoft guidance covers shared Apple Developer credential management for Hot Restart (checked October 31, 2025). Document your internal process and review it regularly.



