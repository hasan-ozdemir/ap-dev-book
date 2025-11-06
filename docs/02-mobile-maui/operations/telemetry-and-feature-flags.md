title: Telemetry & Feature Flags
description: Implementing Application Insights, Azure Monitor, and feature flag strategies for .NET MAUI apps.
last_reviewed: 2025-11-03
owners:
  - @prodyum/site-reliability
---

# Telemetry and Feature Flags

Modern MAUI applications rely on continuous monitoring and controlled experiments to stay healthy in production. This guide explains how to wire Application Insights, OpenTelemetry, and Azure App Configuration so you can observe behaviour, ship safely, and roll back quickly.

## 1. Observability layers

- **Logs:** Inject `ILogger` across the app to centralise diagnostic events; Application Insights and Azure Monitor Log Analytics ingest structured logs out of the box.citeturn6search0turn5search1
- **Metrics:** Collect CPU, memory, and render times via `EventCounters` or OpenTelemetry Metrics. .NET 9 enables `Meter` APIs even in mobile builds.citeturn1search8turn5search1
- **Traces:** Instrument REST, GraphQL, and gRPC calls with `ActivitySource` so distributed traces correlate automatically with Application Insights `RequestTelemetry`.citeturn6search0

## 2. Application Insights integration

1. Create an Application Insights resource in Azure (Standard plan).citeturn6search0  
2. Add the exporters to `Directory.Build.props`:
   ```xml
   <PackageReference Include="Azure.Monitor.OpenTelemetry.AspNetCore" Version="1.3.0" />
   <PackageReference Include="Azure.Monitor.OpenTelemetry.Exporter" Version="1.3.0" />
   ```
3. Register telemetry in `MauiProgram.cs`:
   ```csharp
   builder.Services.AddOpenTelemetry()
       .UseAzureMonitor(options =>
       {
           options.ConnectionString = config["Azure:ApplicationInsights:ConnectionString"];
       })
       .WithTracing(t => t.AddSource("Contoso.Todo"))
       .WithMetrics(m => m.AddMeter("Contoso.Todo"));
   ```
4. Inject `ILogger<T>` wherever you need contextual logging—events flow to Application Insights automatically.

Azure Monitor’s .NET collector shares connection strings across mobile, desktop, and cloud workloads, and `UseAzureMonitor` captures HTTP, gRPC, and SQL telemetry without extra code.citeturn5search1turn6search0

## 3. Crash and performance tracking

- **App Center migration:** Microsoft recommends moving diagnostics from App Center to Azure Monitor; complement that with BrowserStack and Azure Pipelines for automated end-to-end traces.citeturn6search4
- **Crash reports:** Monitor `AppDomain.CurrentDomain.UnhandledException` and `TaskScheduler.UnobservedTaskException` to forward unhandled errors via `TrackException`.citeturn6search0
- **Real-time alerts:** Build Log Analytics queries and wire them to Alert Rules so Teams or Slack webhooks fire when KPIs breach thresholds.citeturn5search1

## 4. Feature flag strategy

Azure App Configuration’s Feature Management SDK ships lightweight clients that run inside MAUI apps.

1. Install packages:
   ```bash
   dotnet add src/Contoso.Todo package Microsoft.FeatureManagement
   dotnet add src/Contoso.Todo package Microsoft.FeatureManagement.AppConfiguration
   ```
2. Configure the pipeline:
   ```csharp
   builder.Configuration.AddAzureAppConfiguration(options =>
       options.Connect(config["Azure:AppConfigConnection"])
              .UseFeatureFlags());

   builder.Services.AddAzureAppConfiguration();
   builder.Services.AddFeatureManagement()
       .UseDisabledFeaturesHandler<DisabledBannerHandler>();
   ```
3. Inject `IFeatureManager` to toggle behaviour at runtime:
   ```csharp
   if (await _featureManager.IsEnabledAsync("Todo:NewEditor", ct))
   {
       // Enable the new editor experience
   }
   ```

The App Configuration portal provides gradual rollouts and targeting rules, and `FeatureManagerSnapshot` caches flag values when the device goes offline.citeturn6search5

## 5. Production validation

- **Live Metrics Stream:** After each release, watch CPU, request rate, and failure counts. Spike? Start the rollback playbook immediately.citeturn6search0
- **Flag auditing:** Track who toggled which feature via App Configuration’s Activity Log and key–value history.citeturn6search5
- **User feedback:** Correlate telemetry (`TrackEvent("FeatureUsed")`), store reviews, and Notification Hubs segments to evaluate adoption.citeturn6search4

With these practices, Prodyum teams can detect incidents quickly, ship experiments safely, and fine-tune MAUI experiences based on real production insight.
