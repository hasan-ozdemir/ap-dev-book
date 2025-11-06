---
title: Static Analysis Toolbox for .NET
description: Configure .editorconfig, analyzers, and third-party scanners to enforce consistent, safe C# code.
last_reviewed: 2025-10-29
owners:
  - @prodyum/devops-guild
---

# Static Analysis Toolbox for .NET

Static analysis catches defects, style issues, and security risks before code runs. This module walks through the built-in .NET analyzer ecosystem, community tooling, and enterprise scanners you should combine to keep MAUI and backend codebases consistent.

---

## 1. Analyzer layers overview

| Layer | Scope | Tooling | Notes |
| --- | --- | --- | --- |
| **Project-level analyzers** | Roslyn analyzers that ship with the SDK. | `Microsoft.CodeAnalysis.NetAnalyzers`, IDE/CA rules.citeturn2search2turn2search5 | Enabled by default for SDK-style projects; configure via `.editorconfig`. |
| **Style & formatting** | Enforces naming, spacing, and readability. | `.editorconfig`, `dotnet format`, CSharpier, StyleCop.citeturn1search6turn2search3 | Keep style rules consistent across IDEs. |
| **Domain-specific analyzers** | Apply product-specific rules. | Roslynator, community analyzers, custom rules.citeturn2search4 | Package as NuGet or analyzer DLL. |
| **Enterprise scanning** | Security, quality gate dashboards. | SonarQube/SonarCloud, CodeQL, Snyk, OSS Review Toolkit.citeturn0search10turn2search0turn2search1 | Integrate results as required status checks. |

---

## 2. Configure `.editorconfig`

Use a root `.editorconfig` to set severity, style, and analyzer options. Example:

```
root = true

[*.cs]
dotnet_diagnostic.CA1505.severity = error
dotnet_diagnostic.IDE0044.severity = warning
dotnet_diagnostic.SA0001.severity = warning
csharp_style_var_for_built_in_types = true:suggestion
dotnet_style_qualification_for_field = false:silent
csharp_preferred_modifier_order = public, protected, internal, private, new, abstract, virtual, override, sealed, readonly, unsafe, extern, static, async : warning
```

Place this at the solution root. Use `dotnet format --verify-no-changes --severity diagnostic` to ensure compliance.citeturn1search6turn2search3

### Analyzer config files

- `GlobalAnalyzerConfig` (preview in .NET 9) lets you scope rules per namespace or folder.
- Store common config under `Directory.Build.props/targets` to apply to all projects.

---

## 3. Built-in .NET analyzers

SDK-style projects automatically reference `Microsoft.CodeAnalysis.NetAnalyzers`. To customise rule sets:

```xml
<PropertyGroup>
  <AnalysisLevel>latest-all</AnalysisLevel>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  <WarningsAsErrors>CA1xxx;CA2xxx</WarningsAsErrors>
</PropertyGroup>
```

Important rules:

- **Maintainability** (`CA1501`, `CA1502`, `CA1505`): guard complexity.citeturn2search5
- **Reliability** (`CA2000`, `CA2007`): async/await and disposal correctness.
- **Security** (`CA2100`, `CA3128`): SQL injection, certificate validation.

Run analyzers locally with `dotnet build -warnaserror` or `dotnet format analyzers`.citeturn1search6

---

## 4. StyleCop & formatting

### 4.1 StyleCop.Analyzers

```xml
<ItemGroup>
  <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.556" PrivateAssets="all" />
</ItemGroup>
```

Disable or customise rules in `.editorconfig`. Example:

```
dotnet_diagnostic.SA1101.severity = none   # allow this.field naming
dotnet_diagnostic.SA1600.severity = suggestion   # optional doc comments
```

### 4.2 dotnet format & CSharpier

- `dotnet format` handles whitespace, style, and analyzer fixes (`dotnet format style` / `dotnet format analyzers`).citeturn1search6
- CSharpier provides deterministic formatting across editors; integrate via pre-commit or CI.citeturn1search6

---

## 5. Roslynator & custom analyzers

Roslynator adds 500+ diagnostics, refactorings, and fixes:

```xml
<PackageReference Include="Roslynator.Analyzers" Version="4.7.0" PrivateAssets="all" />
```

For product-specific rules:

1. Create an analyzer project with the **Analyzer with Code Fix** template.
2. Implement `DiagnosticAnalyzer` and `CodeFixProvider`.
3. Publish as a NuGet package; reference with `PrivateAssets="all"`.

Document custom rule IDs and suppressions in `docs/quality-rules.md` to keep the team aligned.

---

## 6. SonarQube / SonarCloud integration

Sonar adds depth: code smell tracking, security hotspots, coverage. The .NET scanner now supports .NET 9 SDKs and provides GitHub Action templates.citeturn0search10turn2search0turn2search1

### GitHub Action snippet

```yaml
jobs:
  sonar:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 9.0.x
      - name: Cache Sonar packages
        uses: actions/cache@v4
        with:
          path: ~/.sonar/cache
          key: ${{ runner.os }}-sonar
      - name: Install Sonar scanner
        run: dotnet tool install --global dotnet-sonarscanner
      - name: Sonar begin
        run: dotnet sonarscanner begin /k:"Company_Project" /o:"company" /d:sonar.token="${{ secrets.SONAR_TOKEN }}"
      - name: Build
        run: dotnet build
      - name: Test
        run: dotnet test --collect:"XPlat Code Coverage"
      - name: Sonar end
        run: dotnet sonarscanner end /d:sonar.token="${{ secrets.SONAR_TOKEN }}"
```

Mark the Sonar quality gate as a required status check in branch policies.

---

## 7. CodeQL security analysis

GitHub’s CodeQL Action v3 is required for .NET 9 support (v2 deprecated March 2025).citeturn2search1 Use the security analysis workflow to catch vulnerable patterns.

```yaml
name: "CodeQL"
on:
  pull_request:
  push:
    branches: [ main ]
jobs:
  analyze:
    runs-on: ubuntu-latest
    permissions:
      actions: read
      contents: read
      security-events: write
    steps:
      - uses: actions/checkout@v4
      - uses: github/codeql-action/init@v3
        with:
          languages: csharp
      - uses: github/codeql-action/autobuild@v3
      - uses: github/codeql-action/analyze@v3
```

Turn on **code scanning alerts** as blocking status checks for critical issues.

---

## 8. Secret & dependency scanning

Add explicit scanners to address supply-chain risks:

- **GitHub secret scanning** (enterprise) or Gitleaks.
- **Dependency review** / Dependabot alerts for vulnerable packages.
- **oss-review-toolkit** for license compliance.

Schedule recurring workflows (`cron`) to run `dotnet list package --vulnerable` and upload SARIF results.

---

## 9. Analyzer baseline & suppression hygiene

Avoid blanket suppressions:

- Store suppression explanations in source (`#pragma warning disable`) with work item references.
- Use `.globalconfig` to tailor severity instead of disabling analyzers.
- Revisit suppression lists quarterly; delete unused entries.

For large legacy imports, generate a baseline using `dotnet format analyzers --baseline`. Track debt reduction on a backlog.

---

## 10. Integration checklist

- [ ] Commit `.editorconfig` with style + analyzer severity.
- [ ] Enforce `AnalysisLevel=latest-all` and treat critical rules as errors.
- [ ] Add StyleCop.Analyzers and Roslynator for additional coverage.
- [ ] Register Sonar and CodeQL actions; mark as required status checks.
- [ ] Automate dotnet-format / analyzers in pre-commit and CI.
- [ ] Document custom analyzers and suppression policies.

---

## Further reading

- .NET analyzer configuration overview.citeturn2search2turn2search5
- `.editorconfig` style and analyzer options.citeturn1search6turn2search3
- SonarQube .NET 9 support announcement.citeturn0search10
- Sonar scanner for .NET quickstart.citeturn2search0
- CodeQL Action v3 migration guidance.citeturn2search1
