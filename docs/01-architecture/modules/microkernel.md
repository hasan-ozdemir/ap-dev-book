---
title: Microkernel (Plug-in) Architecture
description: Keep a minimal core and extend functionality through plug-ins for configurable products.
last_reviewed: 2025-10-31
owners:
  - @prodyum/architecture-guild
---

# Microkernel (Plug-in) Architecture

The microkernel (plug-in) architecture separates a minimal core that provides runtime services—bootstrapping, messaging, configuration, extensibility points—from plug-ins that deliver vertical business features. Azure Architecture Center recommends this style for products that must be customized rapidly per customer or deployment.citeturn1search1

---

## Structural elements

| Component | Responsibilities |
| --- | --- |
| **Microkernel core** | Hosts the application shell, loads plug-ins, manages lifecycle, and exposes contracts.citeturn1search1 |
| **Plug-ins** | Implement business features, UI modules, or integration adapters while depending only on core contracts.citeturn1search1 |
| **Internal services** | Provide shared capabilities such as logging, telemetry, localization, and caching.citeturn1search1 |
| **Adapters** | Normalize external APIs (ERP, payment gateway) so plug-ins interact through stable abstractions.citeturn1search1 |

---

## Implementation guidelines for .NET

1. **Define contracts:** Publish interfaces (`IModule`, `ICommand`, `IPluginContext`) in a core assembly; versions of these contracts govern compatibility.
2. **Discover plug-ins:** Load assemblies from configurable folders or NuGet feeds at startup, registering their services via `Microsoft.Extensions.DependencyInjection`.citeturn1search1
3. **Isolate failures:** Consider AppDomain or process isolation for untrusted plug-ins; capture health metrics per plug-in to aid troubleshooting.
4. **Configuration:** Use feature flags or metadata to toggle plug-ins per tenant/customer without redeploying the core.

```csharp
var pluginAssemblies = PluginLoader.Discover("./plugins");

foreach (var assembly in pluginAssemblies)
{
    services.Scan(scan => scan
        .FromAssemblies(assembly)
        .AddClasses(c => c.AssignableTo<IModule>())
        .As<IModule>()
        .WithSingletonLifetime());
}
```

---

## When to adopt the microkernel style

- Product lines require rapid customer-specific extensions (POS, ERP, industrial automation).citeturn1search1
- You need to offer white-label experiences where features are enabled or disabled per tenant without branching code.
- Regulatory or regional requirements demand plug-in replacements (payment providers, tax calculation, localized workflows).

---

## Trade-offs

| Advantages | Considerations |
| --- | --- |
| Accelerates customization and experimentation | Core complexity increases as more extension points emerge |
| Supports independent plug-in release cadence | Strong governance needed to prevent breaking contracts |
| Enables marketplace/ecosystem models | Versioning and dependency conflicts require disciplined testing |

---

## Best practices

- Document plug-in development guidelines and build validation pipelines that run plug-in tests against the latest core.citeturn1search1
- Emit telemetry tagged by plug-in to identify performance regressions.
- Provide compatibility shims or adapter layers when contracts evolve to keep legacy plug-ins running.

---

## Further reading

- Azure Architecture Center – Microkernel architecture style.citeturn1search1
