title: Mobile UX Foundations for .NET MAUI
description: Principles, heuristics, and accessibility considerations for MAUI user experiences.
last_reviewed: 2025-11-03
owners:
  - @prodyum/maui-guild
---

# Mobile UX Foundations

.NET MAUI empowers teams to ship consistent, accessible experiences across four platforms from a single codebase. This playbook helps Prodyum squads maintain the same UX bar from concept to store submission with practical principles, control guidance, and validation checklists.

## 1. Define experience goals

- **Information architecture:** Align the navigation model (Shell, Flyout, Tabs, Drawer) with platform expectations so Android and iOS journeys feel native. Shell’s project-wide routes and templates map bottom navigation and tab bar details automatically.citeturn3search0
- **Visual consistency:** Centralise color palettes in `Styles.xaml` or `AppThemeColor` resources; combine XAML styling with CSS and `ResourceDictionary` to keep typography and spacing uniform.citeturn5search3
- **Performance-aware flows:** Prefer `VisualStateManager` and CommunityToolkit behaviours for lightweight interactions so animations stay under 16 ms per frame, even when data binding executes in the background.citeturn3search3turn4view0

## 2. Touch ergonomics and gesture design

- **Touch targets:** Respect platform guidance—44 × 44 dp (Apple) and 48 × 48 dp (Material). Adjust padding and hit areas with `DeviceDisplay.MainDisplayInfo.Density` to cover high-DPI hardware.citeturn4search1turn4search3
- **Gesture hierarchy:** Provide tap or menu alternatives for drag or swipe-only actions. CommunityToolkit and partner component libraries expose drag-and-drop or swipe gestures with MVVM-friendly commands.citeturn7search0turn3search3
- **Animation testing:** Pair `VisualStateManager`, `CommunityToolkit.Maui.Animations`, and Live Preview to validate transitions on emulators and devices without blocking the UI thread.citeturn4view0turn1search9

## 3. Typography and color systems

- Use `SemanticFontSize` and `DynamicResource` so font scaling respects system accessibility settings, aligning with WCAG 1.4.4.citeturn4search0turn4search3
- Configure `AppThemeBinding` and dual color resources to maintain at least a 4.5:1 contrast ratio between light and dark themes; add UI tests that assert `AutomationProperties.IsInAccessibleTree` for critical controls.citeturn5search3turn6search1
- Deliver crisp icons on notched or high-density devices by using `SvgImageSource` plus the `maui-icon` build targets that validate asset sizes during CI.citeturn3search3

## 4. Accessibility and assistive tech

- **Reading order:** Leverage `SemanticOrderView` and `SemanticProperties` to synchronise visual hierarchy with screen-reader traversal.citeturn6search1turn4search3
- **Automation metadata:** Populate `AutomationProperties.Name` and `HelpText` for VoiceOver/TalkBack, and hide decorative elements with `AutomationProperties.IsInAccessibleTree="False"`.citeturn6search1
- **Dynamic updates:** When list items change frequently, refresh `SemanticProperties.Description` to prevent repeated announcements and improve comprehension.citeturn6search1

## 5. Adapting to platform differences

- **Visual density:** `OnIdiom` and `OnPlatform` triggers keep layout variants in one XAML file, while .NET 9’s layout performance improvements let you build custom grids when needed.citeturn4search3turn3search3
- **Safe areas:** Use `On<iOS>.SetUseSafeArea(true)` to respect cut-outs, and apply Android `WindowInsets` APIs (via handlers or platform effects) to manage status/navigation bars.citeturn2search0turn1search6
- **Desktop readiness:** Introduce `SplitView`, `Expander`, `ToolBarItem`, and pointer/keyboard Visual States to take advantage of wide screens when MAUI targets desktop form factors.citeturn3search3turn1search9

## 6. Design-validation lifecycle

1. **Wireframe to prototype:** Store Figma or XD design tokens in a shared `ResourceDictionary` so updates flow through all XAML pages.citeturn5search3
2. **Live preview:** Visual Studio 17.14 Live Preview and Hot Reload accelerate iteration; test on emulators, simulators, and physical devices before sign-off.citeturn4view0turn1search9
3. **Accessibility sweep:** Automate scans with Appium + axe or Windows Accessibility Insights; fail the pipeline if required `AutomationId` values are missing.citeturn6search1
4. **Checklist sign-off:** Confirm navigation flows with the product owner, validate themes across DPI settings, and secure QA approval of the accessibility report.citeturn4search1turn6search1

Pair these foundations with the Responsive Layout, Control Reference, and Visual Patterns guides to deliver cohesive, accessible MAUI experiences end-to-end.
