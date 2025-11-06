---
title: Apple App Store & TestFlight
description: Prepare Prodyum releases for App Store review with 2025 policy updates in mind.
last_reviewed: 2025-10-29
owners:
  - @prodyum/release-office
---

# Apple App Store & TestFlight

## 1. Platform requirements (2025)

- Builds submitted after **April 24, 2025** must be compiled with **Xcode 16 or later** using the **iOS 18 SDK (or companion platform SDKs)**.citeturn1search0turn1search2
- Provide updated age rating responses by **January 31, 2026** and ensure App Privacy answers, Accessibility Nutrition Labels, and motion disclosures reflect spatial content.citeturn1search0turn1search6turn4search1
- Include privacy manifests for your app and bundled SDKs; generate an Xcode privacy report to verify declarations before upload.citeturn0search0turn0search3turn0search4

## 2. Build pipeline steps

1. Use the GitHub Actions or Azure DevOps templates in this portal to archive an `.ipa` with `RuntimeIdentifier=ios-arm64`; ensure runner images install Xcode 16.citeturn1search3
2. Sign with a production certificate and provisioning profile from the Prodyum Apple Developer account.
3. Upload using App Store Connect API tooling (`xcrun notarytool`, `Transporter`, or fastlane `deliver`—`altool` remains but is being deprecated).citeturn1search1
4. Attach build metadata:
   - Version (Semantic Versioning: `major.minor.patch`)
   - Build number (CI pipeline run number)
   - Release notes (human readable)

## 3. Metadata checklist

- Localise the app name (≤ 30 characters) and subtitle for each supported locale.citeturn1search1
- Refresh promotional text and in-app events tied to the release.citeturn1search1
- Provide screenshots for 6.7-inch, 6.1-inch, iPad 12.9-inch, and visionOS canvases when supported.
- Verify Privacy Policy, support, and marketing URLs; align data disclosures with the generated Privacy Report.citeturn0search0turn0search3
- Update content rights statements when streaming or third-party assets are used.

## 4. Review notes

- Provide demo credentials seeded with sample data so the review team can access gated features.citeturn1search0
- Document background modes, push notifications, HealthKit, or entitlement-heavy capabilities in the Notes field.citeturn1search0
- If you offer Sign in with Apple, show where accounts can be deleted in-app and describe the path in the review notes.citeturn1search0

## 5. After submission

- Monitor **App Store Connect → Activity → App Store Review** and respond to any follow-up questions promptly.citeturn1search1
- When approved, configure a phased release or manual launch to align with rollout strategy.citeturn0search0turn0search4
- Track crash reports via Xcode Organizer and App Store Connect analytics; sync actionable issues into GitHub.citeturn1search1

## 6. Common rejection reasons

- Missing or inaccurate privacy disclosures (App Privacy answers, privacy manifest, motion/accessibility labels).citeturn0search0turn1search6turn4search1
- Login required but demo credentials not provided.
- UI misaligned with current Human Interface Guidelines (safe-area, status bar, or visionOS layout issues).citeturn1search1
- Linking against deprecated APIs or SDKs not supported by Xcode 16 / iOS 18.citeturn1search0turn1search2

Keep this guide updated as Apple announces additional policy bulletins (WWDC 2026 preview expected June 2026).citeturn1search0
