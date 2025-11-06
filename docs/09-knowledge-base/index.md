---
title: Developer Knowledge Base
description: High-impact Q&A distilled from the portal to help engineers solve common .NET MAUI, C#, Azure, and delivery problems fast.
last_reviewed: 2025-11-03
owners:
  - @prodyum/knowledge-guild
---

# Developer Knowledge Base

This knowledge base curates frequent questions Prodyum engineers raise while delivering .NET 9/.NET MAUI products. Each answer points to authoritative Microsoft documentation or trusted tooling vendors so you can move from problem to production quickly. It is equally available to partner organisations and independent developers who want distilled answers without combing through every playbook.

## How to use this guide

1. Identify the area you are working on (CI/CD, platform integration, architecture, Azure services, or language mastery).
2. Scan the numbered questions and review the concise answer plus cited references for deeper reading.
3. Apply the checklists or code snippets in your project, then explore the linked resources for implementation detail.

---

## A. CI/CD & Delivery Operations

1. **How do we keep .NET MAUI workloads stable in CI when the .NET SDK updates?**  
   Pin the SDK with `global.json` and run `dotnet workload install maui --version <sdk-version>` inside every pipeline so manifest changes cannot break builds; the .NET team recommends reinstalling workloads whenever the SDK or Xcode versions shift.citeturn2search0turn2search3

2. **What is the official migration plan now that Visual Studio App Center build/test/distribute retires on 31 March 2025?**  
   Microsoft's retirement guide directs teams to move builds to Azure Pipelines or GitHub Actions, automated device testing to BrowserStack, and distribution to TestFlight/Google Play while preserving certificates and release history.citeturn0search0

3. **Which hosted runners should we target for MAUI pipelines in late 2025?**  
   Adopt the new `macos-15` Apple Silicon and `windows-2025` runners announced for GitHub Actions and Azure DevOps, and update YAML labels ahead of the retirement windows for macOS 12 and other legacy images.citeturn0search4turn1search6

4. **How can we build Android and iOS packages in a single GitHub Actions workflow?**  
   Follow the Weather21 sample pipeline: run on `macos-15`, install MAUI workloads once, publish the `net9.0-android` and `net9.0-ios` targets in parallel, then upload artefacts for signing or store submission.citeturn2search6

5. **When should we choose Codemagic versus Bitrise for mobile CI/CD?**  
   Choose Codemagic when you want managed Apple Silicon hardware, MAUI-ready YAML templates, and secure groups for signing assets; choose Bitrise when you need App Center migration tooling, OTA install pages, and a rich step marketplace for mobile distribution.citeturn1search5turn1search0

---

## B. MAUI Platform Integration

6. **What is the recommended approach for precise geolocation in .NET MAUI?**  
   Use `GeolocationRequest` with `GeolocationAccuracy.Best`, provide Info.plist and AndroidManifest descriptions, and request runtime permission via `Permissions.LocationWhenInUse` before reading coordinates.citeturn6view0

7. **How do we integrate Azure Notification Hubs for cross-platform push notifications?**  
   Configure Notification Hubs with platform credentials, register MAUI devices with the Azure SDK sample, and send templated notifications so Android, iOS, and Windows receive tailored payloads.citeturn7search1

---

## C. Architecture & Patterns

8. **When should we favour a modular monolith instead of microservices?**  
   Microsoft's architecture guidance recommends starting with a modular monolith to keep deployment simple and enforce domain boundaries, then evolving toward microservices only when scaling or team autonomy outweigh added complexity.citeturn8search7turn8search1

9. **How do Architecture Decision Records (ADRs) complement Domain-Driven Design?**  
   Capture the context, decision, and consequences for each domain boundary or integration choice in ADRs so DDD models stay understandable as systems grow.citeturn9search0

---

## D. Azure & Cloud Operations

10. **What is the quickest path to enable serverless push notifications for MAUI apps?**  
    Pair Azure Notification Hubs with serverless triggers (Azure Functions or Logic Apps) to send platform-tailored payloads without maintaining custom push infrastructure-the official quickstart covers the end-to-end wiring.citeturn7search1

---

## E. Language & Framework Mastery

11. **How should we structure learning sprints to cover every modern C# keyword?**  
    Work through the C# Language Tour modules in this portal, running each console snippet and tracking progress via the completion checklist so engineers stay current with C# 13 and preview features; the Weather21 sample demonstrates how the snippets integrate into MAUI tooling.citeturn2search6

12. **Why is it important to reinstall MAUI workloads locally after an SDK or Xcode update?**  
    Microsoft notes that SDK upgrades can invalidate workloads; reinstalling locally before pushing changes prevents CI pipelines from failing unexpectedly.citeturn2search3
