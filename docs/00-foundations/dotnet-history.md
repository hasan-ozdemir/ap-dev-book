---
title: .NET History & Platform Evolution
description: From .NET 1.0 to .NET 10 – releases, innovations, and the naming journey.
last_reviewed: 2025-10-29
owners:
  - @prodyum/language-guild
---

# .NET History & Platform Evolution

.NET has evolved from a Windows-only framework into a cross-platform ecosystem spanning desktop, mobile, cloud, and IoT. This guide charts the journey from .NET Framework 1.0 through .NET 9 (current release) and looks ahead to .NET 10, highlighting key innovations and the rationale behind branding changes (.NET Framework, .NET Core, .NET Standard, “.NET”).

---

## 1. Timeline overview

| Year | Version | Key innovations | Sample |
| --- | --- | --- | --- |
| 2002 | .NET Framework 1.0 | CLR, ASP.NET Web Forms, Windows Forms.citeturn0search0 | `Console.WriteLine("Hello .NET 1.0");` |
| 2003 | .NET Framework 1.1 | ASP.NET mobile controls, ODBC support. | `System.Data.Odbc.OdbcConnection` |
| 2005 | .NET Framework 2.0 | Generics, partial classes, anonymous methods.citeturn0search0 | `List<int> numbers = new();` |
| 2006 | .NET Framework 3.0 | WPF, WCF, WF, CardSpace (Windows only). | XAML-driven UIs. |
| 2007 | .NET Framework 3.5 | LINQ, extension methods, ASP.NET AJAX. | `var evens = numbers.Where(n => n % 2 == 0);` |
| 2010 | .NET Framework 4.0 | Parallel Task Library, MEF. | `Parallel.For(0, 10, Console.WriteLine);` |
| 2012 | .NET Framework 4.5 | async/await, HttpClient. | `await httpClient.GetStringAsync(url);` |
| 2014 | .NET Framework 4.5.2–4.6 | RyuJIT, SIMD, improvements. | `Vector<double>` |
| 2016 | .NET Core 1.0 | Cross-platform runtime, CLI tooling.citeturn0search2 | `dotnet new console` |
| 2017 | .NET Standard 2.0 | Unifies API surface across platforms.citeturn0search4 | Portable libraries share APIs. |
| 2018 | .NET Core 2.1/2.2 | Span<T>, SignalR, global tools. | `Span<byte> buffer = stackalloc byte[256];` |
| 2019 | .NET Core 3.0 | WPF/WinForms on Windows, gRPC, worker services.citeturn0search3 | `dotnet new wpf` |
| 2020 | .NET 5 | Unifies branding; single runtime. | `record` types preview (C# 9). |
| 2021 | .NET 6 (LTS) | MAUI preview, minimal APIs, Hot Reload.citeturn0search3 | `app.MapGet("/", () => "Hello");` |
| 2022 | .NET 7 | Performance boosts, minimal API filters. | `app.MapGet("/json", () => Results.Json(...));` |
| 2023 | .NET 8 (LTS) | Native AOT (GA), MAUI improvements, cloud-native. | `dotnet publish -p:PublishAot=true` |
| 2024 | .NET 9 | MAUI performance, cloud native, preview for .NET 10 features.citeturn0search3 | `Task.WhenEach` (preview). |
| 2025 | .NET 10 (on roadmap) | Unifying incubations (Authentication, Semantic Kernel).citeturn0search11 | Expect broader AI integration. |

---

## 2. Naming & branding evolution

### 2.1 .NET Framework (2002–2020)

- Windows-only runtime with CLR, GAC, Windows Forms, WPF.
- Versions 1.0 → 4.8 (LTS on Windows).
- Still supported for legacy apps; no new features beyond servicing.citeturn0search4

### 2.2 .NET Core (2016–2020)

- Cross-platform rewrite with modular runtime, new CLI.
- Introduced side-by-side installs and container-friendly deployment.
- Versions 1.x → 3.1 (3.1 LTS).

### 2.3 .NET Standard (2016–2020)

- API specification bridging Framework, Core, Mono, Xamarin.
- Enabled library authors to target multiple runtimes with one contract.
- Superseded by “target .NET” once platforms unified; guidance now is to target .NET 6+ where possible.citeturn0search4

### 2.4 “.NET” (since 2020)

- Beginning with .NET 5, Microsoft adopted a single product name (“.NET”) replacing “.NET Core”.
- Annual cadence: odd-numbered releases are STS (18-month support), even-numbered releases are LTS (3-year support).
- .NET 9 (2024) is STS; .NET 10 (2025) will be LTS.citeturn0search3turn0search11

---

## 3. Highlights by release

### .NET Framework era

- **Generics (2.0)** – revolutionized .NET collections.
- **Language Integrated Query (3.5)** – unified querying with C#/VB.
- **async/await (4.5)** – simplified asynchronous programming.

### .NET Core to unified .NET

- **Cross-platform runtime** – Linux/macOS support from Core 1.0.
- **Microservices & containers** – lightweight footprints with self-contained deployments.
- **Minimal APIs & Hot Reload** – developer productivity leaps in .NET 6.
- **Native AOT** – ahead-of-time compilation for console/cloud apps (GA in .NET 8).citeturn0search3

### .NET 9 focus areas

- MAUI handler performance, trimming, technology incubators (.NET Aspire).
- Arm64 performance improvements; C# 13 previews.citeturn0search3

### .NET 10 outlook

- Unified incubations (Authentication, Semantic Kernel, Components).
- Continued investment in AI, cloud-native container support, developer experience.citeturn0search11

---

## 4. Code snippets through time

### Generics (.NET 2.0)

```csharp
List<string> names = new() { "Ana", "Ben", "Cara" };
```

### LINQ (.NET 3.5)

```csharp
var filtered = names.Where(n => n.StartsWith("C"));
```

### async/await (.NET 4.5)

```csharp
public async Task<string> DownloadAsync(string url)
{
    using var client = new HttpClient();
    return await client.GetStringAsync(url);
}
```

### Minimal API (.NET 6)

```csharp
var app = WebApplication.Create();
app.MapGet("/", () => "Hello .NET 6");
app.Run();
```

### Native AOT publish (.NET 8)

```bash
dotnet publish -c Release -p:PublishAot=true
```

---

## 5. Support & lifecycle

- LTS releases: .NET 6, .NET 8 (current LTS). .NET 10 will be next LTS.
- STS releases receive 18 months of support; plan upgrades accordingly.
- .NET Framework remains in maintenance for Windows; no new features.

---

## 6. Checklist for teams

- [ ] Know which .NET release your product targets; align with support timelines.
- [ ] Audit dependencies to ensure .NET 6+ compatibility.
- [ ] Use global.json to pin SDK versions across teams.
- [ ] Monitor .NET Announcements (GitHub repos) for breaking changes.
- [ ] Plan for yearly updates; schedule LTS upgrades every 2–3 years.

---

## Further reading

- Official .NET release notes and history.citeturn0search0turn0search2turn0search3
- .NET branding and naming guidance.citeturn0search4turn0search10
- .NET 9/10 roadmap (unified incubations).citeturn0search11
