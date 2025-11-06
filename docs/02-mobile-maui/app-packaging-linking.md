---
title: Building & Linking .NET MAUI App Packages
description: Configure trimming, AOT, and packaging for Android (.apk/.aab) and iOS (.ipa) while keeping apps reliable.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

# Building & Linking .NET MAUI App Packages

Producing store-ready .NET MAUI packages involves intermediate language (IL) trimming, platform linkers, and Ahead-of-Time (AOT) compilation. Done correctly, these steps shrink binaries and improve startup while preserving critical code paths. This playbook explains linker defaults, how to harden reflection-heavy code, and the commands Prodyum teams use to ship Android App Bundles and iOS IPA files.

---

## 1. Linking fundamentals

- **IL trimming** removes unused managed types. In Release builds, Android links SDK assemblies (`AndroidLinkMode=SdkOnly`) while iOS and Mac Catalyst link all assemblies by default to meet App Store size targets.[^android-link][^ios-link]
- **Native linking** lets clang/LLVM (iOS) or the Android toolchain strip unused native symbols; this runs automatically during Release builds.
- **Native AOT** compiles managed IL to native binaries. .NET 9’s Android toolchain added support for Google Play’s 16 KB page-size requirement, and the same release brought Native AOT for iOS, Mac Catalyst, and Windows, with up to 2.5× smaller apps and twice the startup speed on Apple platforms.[^android16kb-policy][^dotnet16kb-support][^native-aot-maui]

---

## 2. Default linker modes

| Platform | Default Release behaviour | How to configure |
| --- | --- | --- |
| Android | Links SDK assemblies, skips app assemblies. | `<AndroidLinkMode>SdkOnly</AndroidLinkMode>` for the default; switch to `Full` only after validating reflection paths.[^android-link] |
| iOS / Mac Catalyst | Links all assemblies to comply with store requirements. | `<MtouchLink>Full</MtouchLink>` in Release, `None` in Debug for faster iteration.[^ios-link] |
| Windows | Trimming disabled by default. | Enable trimming with `<PublishTrimmed>true</PublishTrimmed>` and pair with analyzers to guard dynamic code.[^dotnet-trim] |

Declare modes explicitly in `.csproj` so build pipelines remain deterministic:

```xml
<PropertyGroup Condition="'$(Configuration)'=='Debug'">
  <AndroidLinkMode>None</AndroidLinkMode>
  <MtouchLink>None</MtouchLink>
</PropertyGroup>

<PropertyGroup Condition="'$(Configuration)'=='Release'">
  <AndroidLinkMode>SdkOnly</AndroidLinkMode>
  <MtouchLink>Full</MtouchLink>
  <PublishTrimmed>true</PublishTrimmed>
</PropertyGroup>
```

---

## 3. Why trim?

| Benefit | Evidence |
| --- | --- |
| Smaller packages | Native AOT on iOS/Mac Catalyst produces apps up to 2.5× smaller than interpreter-based builds, and Android Native AOT satisfies Google Play’s 16 KB page-size policy to avoid upload rejections.[^native-aot-maui][^android16kb-policy][^dotnet16kb-support] |
| Faster cold start | Native AOT eliminates JIT on iOS and dramatically reduces startup time on Android according to .NET team benchmarks.[^native-aot-maui][^dotnet16kb-support] |
| Lower runtime memory | Fewer assemblies and metadata tables reduce Gen0 pressure, especially when trimming unused platform handlers.[^android-link][^ios-link] |
| Policy compliance | Play Console enforces the page-size limit from November 1, 2025; linking and AOT keep binaries compatible.[^android16kb-policy] |

---

## 4. Risks and hardening tactics

| Risk | Mitigation |
| --- | --- |
| Reflection trimming | Decorate types with `[DynamicDependency]`, `[Preserve]`, or describe them in `linker.xml`/`TrimmerRootDescriptor` files so the linker keeps the required members.[^android-link][^dotnet-trim] |
| P/Invoke removal | Set `<IsTrimmable>false</IsTrimmable>` on libraries that provide native exports, or preserve specific entry points in the descriptor file.[^dotnet-trim] |
| Third-party libraries | Audit serializers, DI containers, and UI frameworks; many provide trimming annotations or guidance. Keep `AndroidLinkMode=None`/`MtouchLink=None` in Debug builds until libraries are validated.[^android-link] |
| Debug visibility | Release symbols are trimmed; publish `.pdb`, `.dSYM`, and `.so` symbol packages alongside artifacts for crash forensics.[^dotnet-trim] |

Descriptor example:

```xml
<linker>
  <assembly fullname="Contoso.Mobile">
    <type fullname="Contoso.Mobile.Services.SecretService" preserve="all" />
  </assembly>
</linker>
```

Reference descriptors via `TrimmerRootDescriptor` or `ExtraTrimmerArguments` to ensure the linker honours them.

---

## 5. Trimming plus AOT

```xml
<PropertyGroup Condition="'$(Configuration)'=='Release'">
  <PublishTrimmed>true</PublishTrimmed>
  <RunAOTCompilation>true</RunAOTCompilation>
  <InvariantGlobalization>true</InvariantGlobalization>
</PropertyGroup>
```

Key considerations:

- Android Native AOT requires the .NET 9 SDK, API level 35 tooling, and NDK r26d+ to meet Google’s policy window.[^dotnet16kb-support][^android16kb-policy]
- Build times increase; run the trimmed+AOT build in CI smoke tests before promoting artifacts.
- iOS always runs under AOT; trimming primarily reduces IPA size and download footprint.[^native-aot-maui]

---

## 6. Packaging commands

### Android (App Bundle)

```bash
dotnet publish -f net9.0-android -c Release \
  -p:AndroidPackageFormat=aab \
  -p:AndroidSigningKeyStore=contoso.keystore \
  -p:AndroidSigningKeyAlias=contoso \
  -p:AndroidSigningKeyPass=$KEY_PASS \
  -p:AndroidSigningStorePass=$STORE_PASS \
  -p:RunAOTCompilation=true \
  -p:PublishTrimmed=true
```

- Validate packages with `bundletool build-apks --bundle Contoso.Mobile.aab --output Contoso.apks` before upload.[^bundletool]
- Stage rollouts through the Google Play Console (internal → closed → production) and monitor crash/ANR dashboards for trimmed builds.[^google-play-release]

### iOS / Mac Catalyst (IPA)

```bash
dotnet publish -f net9.0-ios -c Release \
  -p:ArchiveOnBuild=true \
  -p:BuildIpa=true \
  -p:IpaPackageDir=artifacts/ios
```

- Refresh certificates and provisioning profiles via Xcode, then upload the generated `.ipa` using Xcode Organizer or Transporter.[^app-store-upload]
- Store signing secrets in secure pipelines (Azure Key Vault, GitHub OIDC) and rotate annually with your Apple Developer renewals.[^app-store-upload]

---

## 7. Debug versus release configurations

- **Debug**: keep trimming disabled (`AndroidLinkMode=None`, `MtouchLink=None`) for quick iterations and richer diagnostic data.
- **Release**: enable trimming and AOT, produce symbol packages, and publish configuration-specific property sheets (for example `Directory.Build.props` for `Release-Internal` vs. `Release-Signed`).
- Document the expected linker settings in your repository README and enforce them with CI guardrails or analyzer tests.[^dotnet-trim]

---

## 8. Validate trimmed builds continuously

- Execute UI automation against the trimmed build artefact (not Debug binaries) using Microsoft’s Appium + NUnit samples and BrowserStack’s hosted device cloud to catch reflection issues early.[^appium-sample][^browserstack]
- As App Center retires in March 2025 (device lab) and June 2026 (Diagnostics), move scheduled runs and crash analytics to BrowserStack, Firebase, or Application Insights ahead of the deadlines.[^appcenter-retirement]
- Monitor telemetry for missing method exceptions or P/Invoke failures—common signals that additional trimming descriptors are required.

Trimmed builds deliver faster, smaller apps, but only if you continuously test the release configuration and preserve the critical code paths your business logic depends on.

---

[^android-link]: Microsoft Learn, "Linking a .NET MAUI Android app," updated October 24, 2024. citeturn9search0
[^ios-link]: Microsoft Learn, "Linking a .NET MAUI iOS app," updated October 24, 2024. citeturn9search1
[^dotnet-trim]: Microsoft Learn, "Prepare .NET apps for trimming," updated August 20, 2024. citeturn13search0
[^native-aot-maui]: Microsoft Learn, "Native AOT deployment overview," accessed November 1, 2025; .NET Blog, "Native AOT for .NET 9," October 24, 2024. citeturn10search1turn10search0
[^android16kb-policy]: Android Developers Blog, "Upcoming Google Play requirement for 16 KB page size in native libraries," May 9, 2024. citeturn11search0
[^dotnet16kb-support]: Microsoft Learn Blog, ".NET 9 supports Google Play's 16 KB page-size requirement," July 31, 2024. citeturn15search0
[^bundletool]: Android Developers, "bundletool," accessed November 1, 2025. citeturn5search0
[^google-play-release]: Google Play Console Help, "Release with confidence," accessed November 1, 2025. citeturn1search0
[^app-store-upload]: App Store Connect Help, "Upload an app with Xcode or Transporter," accessed November 1, 2025. citeturn12search0
[^appium-sample]: Microsoft Learn, ".NET MAUI - UI testing on BrowserStack with Appium and NUnit," April 15, 2025. citeturn3search0
[^browserstack]: BrowserStack, "Debug Appium tests with App Automate," accessed November 1, 2025. citeturn14search0
[^appcenter-retirement]: Microsoft Learn, "Visual Studio App Center Retirement," March 20, 2025. citeturn4search0



