---
title: Google Play Console
description: Deliver production-ready Android builds while complying with Google Play 2025 policies.
last_reviewed: 2025-10-29
owners:
  - @prodyum/release-office
---

# Google Play Console

## 1. Policy highlights (2025)

- New apps and updates must target **Android 15 (API level 35)** by **31 Aug 2025** or they will be rejected from publishing.citeturn2search0
- Use Google Play’s internal, closed, and production tracks plus country-specific custom listings to stage rollouts safely.citeturn1search1turn1search4
- Audit third-party SDKs quarterly; providers must register in the **Google Play SDK Console** and adhere to SDK policy disclosures.citeturn1search0turn2search5

## 2. Build pipeline steps

1. Generate a signed `.aab` using the GitHub Actions or Azure DevOps templates in this portal.
2. Store keystores in Azure Key Vault (or another HSM-backed store) and download with short-lived secrets during CI/CD.citeturn3search0
3. Upload via the Play Developer API or GitHub Action (`google-github-actions/upload-android@v1`) to the internal testing track.
4. Promote builds to closed, open, and production tracks only after telemetry and policy checks pass.

## 3. Play Console configuration

- **App content:** Update the Data Safety section whenever permissions, SDKs, or data collection practices change.citeturn5search1
- **Store listing:** Provide device-specific screenshots (phones, tablets, Chromebooks, Wear) aligned with Google Play asset guidelines.citeturn1search4
- **Pricing & distribution:** Confirm country availability and use staged rollouts (e.g., 10% increments) to limit blast radius before full launch.citeturn1search4

## 4. Pre-launch reports

- Enable automated pre-launch tests across form factors and investigate crashes/ANRs before shipping.citeturn2search6
- Resolve issues flagged by Android Vitals; stay under Google’s bad-behaviour thresholds for crashes (≤ 1.09 %) and ANR (≤ 0.47 %).citeturn8search0

## 5. Post-launch monitoring

- Monitor **Reach and devices** plus device catalog exclusions to ensure API level and hardware compliance.citeturn1search2
- Respond to reviews promptly—Google highlights higher satisfaction for developers who reply within 24 hours.citeturn1search3
- Pause staged rollouts if crash/ANR metrics breach thresholds or regression alerts fire in Android Vitals.citeturn8search0

## 6. Common rejection reasons

- Content rating questionnaire incomplete or inconsistent with gameplay footage.citeturn2search5
- Missing privacy policy URL for apps targeting children, VPN categories, or sensitive data.citeturn2search7
- Inaccurate Data Safety disclosures versus runtime behaviour (permissions, SDK signals, network logging).citeturn5search1

Stay aligned with the quarterly Google Play policy updates (typically February, May, August, October) by subscribing to policy digest emails.citeturn2search5
