---
title: REST API Clients in .NET MAUI
description: Guidance for HttpClientFactory, Refit, and resilience patterns when consuming RESTful services.
last_reviewed: 2025-11-03
owners:
  - @prodyum/integration-guild
---

# REST API Client Playbook

Sustainable performance, resilience, and observability are essential when .NET MAUI apps call REST services. This playbook shows how to build a production-ready client layer with .NET 9 features such as `HttpClientFactory`, `Microsoft.Extensions.Http.Resilience`, and productivity helpers like Refit.

## 1. Architecture in context

- **Service layer**: Isolate business rules from the UI by wrapping every API behind `IApiClient` abstractions and concrete services. That approach lets you inject fakes during UI tests.
- **Dependency injection**: Register typed or named clients with `AddHttpClient` in `MauiProgram`. The `ConfigureHttpClientDefaults` + `AddStandardResilienceHandler` combination applies a consistent resilience profile to every client.citeturn1search0turn1search2
- **Observability**: Use the `ConfigureHttpMessageHandlerBuilder` extension to plug Application Insights or OpenTelemetry so every request is traced.citeturn1search8

## 2. Bootstrapping with HttpClientFactory

`MauiProgram.cs`:

```csharp
builder.Services.ConfigureHttpClientDefaults(builder =>
{
    builder.AddStandardResilienceHandler(options =>
    {
        options.Retry.DisableForUnsafeHttpMethods();
        options.CircuitBreaker.FailureRatio = 0.1;
    });
    builder.AddServiceDiscovery();
});

builder.Services.AddHttpClient<TodoApiClient>(client =>
{
    client.BaseAddress = new Uri(config["Api:BaseUrl"]);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("AP-MAUI-Todo/1.0");
});
```

- `ConfigureHttpClientDefaults` centralises timeouts, resilience policies, and default headers for every client.citeturn1search0
- `AddStandardResilienceHandler` chains rate limiting, overall timeout, retry, circuit breaker, and attempt timeout strategies in order.citeturn1search0turn1search2

## 3. Client implementation

```csharp
public sealed class TodoApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public TodoApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<TodoItemDto>> GetAsync(CancellationToken ct)
    {
        using var response = await _http.GetAsync("todos", ct);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        return await JsonSerializer.DeserializeAsync<List<TodoItemDto>>(stream, _jsonOptions, ct)
               ? [];
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoRequest request, CancellationToken ct)
    {
        using var content = JsonContent.Create(request, options: _jsonOptions);
        using var response = await _http.PostAsync("todos", content, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoItemDto>(_jsonOptions, ct)
               ?? throw new InvalidOperationException("Empty payload");
    }
}
```

- Use `EnsureSuccessStatusCode` alongside production-grade logging; replace it with a custom error model if required.
- `JsonSerializerDefaults.Web` already enables `camelCase`, ISO dates, and UTF-8 settings.

## 4. Refit and source generators

Refit generates clients from interface definitions, shrinking the amount of data-layer boilerplate.

```csharp
public interface ITodoApi
{
    [Get("/todos")]
    Task<IReadOnlyList<TodoItemDto>> GetAsync(CancellationToken ct = default);

    [Post("/todos")]
    Task<TodoItemDto> CreateAsync([Body] CreateTodoRequest request, CancellationToken ct = default);
}
```

`MauiProgram.cs`:

```csharp
builder.Services
    .AddRefitClient<ITodoApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(config["Api:BaseUrl"]))
    .AddStandardResilienceHandler();
```

- Refit 7+ uses source generators for compile-time validation and is compatible with the .NET 9 SDK in MAUI projects.citeturn0search9turn0search10
- Because Refit clients are built on `HttpClientFactory`, the defaults configured earlier apply automatically.

## 5. Resilience fine-tuning

- Call `DisableForUnsafeHttpMethods` to avoid automatic retries for POST/DELETE requests and protect non-idempotent operations.citeturn1search0
- For advanced requirements, extend the policy pipeline with `AddResilienceHandler("api", config => { ... })` to plug custom Polly strategies such as hedging or delayed fallback.citeturn1search4
- Add telemetry handlers with `builder.Services.AddHttpClient("TodoApi").AddStandardResilienceHandler().AddHttpMessageHandler<LoggingHandler>();`.

## 6. Security and authentication

- **Azure AD / Entra ID B2C**: Inject an `IAccessTokenProvider` implementation and append `AuthenticationHeaderValue("Bearer", token)`.
- **mTLS or certificate pinning**: Use `HttpClientHandler.ServerCertificateCustomValidationCallback` to block man-in-the-middle attacks.
- **Rate limits**: Configure `options.Retry.ShouldHandle` based on the provider's throttling guidance.citeturn1search0

## 7. Observability

- Leverage `Microsoft.Extensions.Logging` so every request flows into the MAUI telemetry pipeline; forward to Application Insights or alternative sinks as required.citeturn1search8
- Emit distributed traces with ActivitySource, for example `using var activity = _activitySource.StartActivity("Todos/Get");`, so OpenTelemetry collectors can correlate requests.
- Combine `ILogger` with `EventCounters` to track latency, error rates, and retry counts.

## 8. Testing strategy

- **Unit tests**: Mock `HttpMessageHandler` using libraries such as `Moq` or `RichardSzalay.MockHttp` to isolate business logic.
- **Contract tests**: Include OpenAPI schema checks in CI to detect breaking API changes early.
- **Integration tests**: Run `WebApplicationFactory` harnesses or Postman/Newman collections against QA; fail the build on unexpected HTTP status codes.

## 9. Post-release health checks

- Inspect `ResiliencePipeline` metrics (retry counts, circuit state) whenever client-side error rates rise.
- Tune retry settings based on App Store/Play Store feedback-mobile networks may need longer timeouts.
- When the API publishes a new version, split your Refit interfaces (for example, `ITodoApiV2`) and plan a phased rollout.

Applying these practices keeps your MAUI application secure, resilient, and observable-meeting the expectations placed on senior engineers responsible for REST integrations.
