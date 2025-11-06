---
title: Failure Detection Across Build & Runtime
description: Catch defects at build, deployment, and production time with .NET diagnostics and security tooling.
last_reviewed: 2025-10-29
owners:
  - @prodyum/devops-guild
---

# Failure Detection Across Build & Runtime

Even with static analysis and reviews, defects slip through. This guide explains how to catch issues during compilation, build, deployment, and runtime using the latest .NET diagnostics, logging, and security scanners.

---

## 1. Build-time defenses

### 1.1 Treat warnings as errors

Set `TreatWarningsAsErrors=true` in engineering-critical projects to fail builds on analyzer violations. Combine with rule-specific severities (see Static Analysis Toolbox).citeturn2search2turn2search5

### 1.2 Custom build scripts

- Use `dotnet format analyzers` and `dotnet test` with `--collect:"XPlat Code Coverage"` to block merges on debt.
- Integrate secret scanning (`trufflehog`, `gitleaks`) into CI.
- Leverage GitHub Code Scanning auto-triage for codeql/sarif findings.citeturn0search10turn2search1

### 1.3 SBOM & dependency risk

Generate SBOMs (CycloneDX) and scan for vulnerabilities:

```bash
dotnet tool install --global CycloneDX
dotnet CycloneDX /project packages.lock.json
dotnet list package --vulnerable
```

Automate nightly jobs that fail on high/Critical CVEs with manual approval to override.

---

## 2. Deploy-time smoke tests

- Run synthetic checks immediately after deployment: simple HTTP GET, health endpoint with dependency check.
- Use Feature toggles to ramp features gradually; monitor for error surges.
- Gate production promotions on telemetry (e.g., Application Insights availability tests).

---

## 3. Runtime diagnostics

### 3.1 dotnet monitor (recommended)

`dotnet monitor` surfaces metrics, logs, traces, and dumps via HTTP. Configure sidecar deployment or Kubernetes operator.

```bash
dotnet monitor collect --metrics --logs --dump-on-exception \
  --metricUrls http://+:52325
```

Expose dashboards by piping metrics to Prometheus/Grafana. Enable alerts on thread pool starvation, GC pauses, and high exception rates.citeturn0search7

### 3.2 dotnet-counters & dotnet-trace

- `dotnet-counters`: live monitoring of CPU, GC, HTTP queue length. Useful for on-call triage.citeturn0search7
- `dotnet-trace`: capture ETW-based trace files for offline analysis in PerfView.

### 3.3 Application Insights & OpenTelemetry

- Instrument MAUI apps and services with OpenTelemetry; export traces/metrics/logs to App Insights or Grafana Cloud.
- Use distributed tracing to spot latency spikes across microservices; track span attributes for release identifiers.

---

## 4. Crash reporting

- Enable crash analytics (App Center, Sentry) in MAUI apps; capture stack traces, device info, OS versions.
- Collect breadcrumbs around crash events (last API call, connectivity state). Review weekly.

---

## 5. Runtime security monitoring

- Turn on GitHub Actions *workflow security analysis* to detect malicious pipeline changes.citeturn2search1
- Use Azure Defender for App Service/Container Apps (optional) for runtime intrusion detection.
- Monitor log anomalies with SIEM (Azure Sentinel, Splunk).

---

## 6. Incident response workflow

1. **Detect** (alerts from CI, runtime metrics, crash reports).
2. **Triage**: Determine blast radius, reproduce with diagnostic tools, gather logs/traces.
3. **Mitigate**: Feature flag rollback, hotfix deployments, scaling adjustments.
4. **Root cause**: Add unit/integration tests to cover the defect, update analyzers or pipeline gates.
5. **Learn**: Document in the incident log; feed findings into retrospectives.

Keep runbooks in the repo (`docs/runbooks/*.md`) with contact rotations and escalation matrix.

---

## 7. Tool matrix

| Stage | Tool | Purpose |
| --- | --- | --- |
| Build | `.editorconfig` analyzers, `dotnet format analyzers`, CodeQL | Prevent regressions and vulnerabilities.citeturn2search2turn2search5turn2search1 |
| Deploy | Smoke tests, health checks | Validate release before traffic. |
| Runtime | dotnet monitor, dotnet-counters, App Insights | Detect performance and failure anomalies.citeturn0search7 |
| Security | Secret scanning, SBOM, workflow security analysis | Guard supply chain.citeturn0search10turn2search1 |

---

## 8. Checklist

- [ ] Treat warnings as errors; fail build on analyzer violations.
- [ ] Generate SBOM + vulnerability reports on each release.
- [ ] Configure smoke tests and feature flags for safe rollouts.
- [ ] Deploy `dotnet monitor` or equivalent in staging/production; create alerts.
- [ ] Instrument distributed tracing and crash analytics.
- [ ] Document incident response runbooks and escalation paths.

---

## Further reading

- .NET diagnostics tooling overview (`dotnet monitor`, `dotnet-counters`).citeturn0search7
- GitHub workflow security analysis & CodeQL updates.citeturn2search1
- Supply chain scanning with OSS review tools.citeturn0search10
