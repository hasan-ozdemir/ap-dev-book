title: GraphQL & gRPC Clients
description: Recipes for GraphQL, gRPC, and realtime connectivity scenarios in .NET MAUI.
last_reviewed: 2025-11-03
owners:
  - @prodyum/integration-guild
---

# GraphQL and gRPC Client Playbook

Beyond REST, GraphQL and gRPC can deliver lean payloads and low-latency experiences for mobile apps. This guide outlines client options, setup steps, performance patterns, and testing strategies tailored to .NET MAUI.

## 1. GraphQL client options

| Library | Key capability | When to choose it |
|---------|----------------|-------------------|
| **Strawberry Shake** | Generates a type-safe client and offline store from your schema; ships MAUI samples.citeturn3search0 | Large schema, offline cache, or multiple platforms. |
| **GraphQL.Client for .NET** | Lightweight HTTP/WebSocket client with manual query handling.citeturn3search1 | Small teams or simple query flows. |
| **Apollo Router / Federation** | Aggregates many back ends into a unified GraphQL endpoint.citeturn4search4 | Microservices that expose a consolidated API. |

### 1.1 Strawberry Shake quick start

1. Add packages:  
   `dotnet add src/Contoso.Todo package StrawberryShake.Transport.Http`
2. Fetch schema and generate code:
   ```bash
   dotnet graphql init https://api.contoso.todo/graphql
   dotnet graphql generate
   ```
3. Register in DI:
   ```csharp
   builder.Services
       .AddContosoTodoClient()
       .ConfigureHttpClient(client =>
       {
           client.BaseAddress = new Uri(config["GraphQL:BaseUrl"]);
       })
       .AddStandardResilienceHandler();
   ```
4. Consume in a ViewModel:
   ```csharp
   var result = await _graphql.GetTodos.ExecuteAsync(ct);
   var todos = result.Data?.Todos?.Nodes ?? Array.Empty<TodoItem>();
   ```

Strawberry Shake 13 adds Hot Reload support and an `EntityStore` offline cache designed for MAUI scenarios.citeturn3search0

### 1.2 GraphQL.Client example

```csharp
var client = new GraphQLHttpClient(new GraphQLHttpClientOptions
{
    EndPoint = new Uri(config["GraphQL:BaseUrl"]),
    EnableMetrics = true
}, new SystemTextJsonSerializer());

var request = new GraphQLRequest
{
    Query = """
        query Todos($after: String) {
          todos(first: 20, after: $after) {
            edges { node { id title completed } cursor }
          }
        }
    """,
    Variables = new { after = cursor }
};

var response = await client.SendQueryAsync<TodoQueryResult>(request, ct);
```

Enable `WebsocketTransport` when you need real-time subscriptions.citeturn3search1

## 2. GraphQL performance and caching

- Strawberry Shake’s normalised `EntityStore` supports offline-first patterns; persist the store under `FileSystem.AppDataDirectory`.citeturn3search0
- Use `AddStandardResilienceHandler` to enforce retry and circuit-breaker policies when calling GraphQL gateways so client failures remain isolated.citeturn1search0turn4search4
- Define depth and complexity limits server-side; Apollo Router reads these constraints from configuration files to guard against expensive queries.citeturn4search4

## 3. gRPC client approaches

| Approach | Summary | Best fit |
|----------|---------|----------|
| **gRPC-Web** | Runs over HTTP/1.1 with a proxy (Envoy, Nginx), which keeps compatibility with firewalls.citeturn4search5 | Shared gateways or restricted corporate networks. |
| **gRPC (HTTP/2)** | `Grpc.Net.Client` reaches native HTTP/2 on Android 5+ and iOS 13+.citeturn4search5turn4search3 | Low-latency streaming and IoT telemetry. |

### 3.1 Defining protobuf contracts

`todo.proto`:
```proto
syntax = "proto3";

service TodoService {
  rpc GetStream (TodoRequest) returns (stream TodoItem);
  rpc Upsert (TodoItem) returns (TodoResponse);
}
```

Generate client assets:
```bash
dotnet new tool-manifest
dotnet tool install dotnet-grpc
dotnet-grpc add-file ./proto/todo.proto -c GrpcClient
```
citeturn4search3

### 3.2 Configuring the MAUI client

```csharp
builder.Services.AddGrpcClient<TodoService.TodoServiceClient>(o =>
{
    o.Address = new Uri(config["Grpc:BaseUrl"]);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new SocketsHttpHandler
    {
        EnableMultipleHttp2Connections = true
    };
    handler.SslOptions.RemoteCertificateValidationCallback = (_, cert, __, ___) =>
        cert?.FriendlyName == "contoso-api";
    return handler;
})
.AddStandardResilienceHandler();
```
citeturn4search5turn1search0

Streaming consumption:

```csharp
using var call = _client.GetStream(new TodoRequest(), ct);
await foreach (var item in call.ResponseStream.ReadAllAsync(ct))
{
    Items.Add(item);
}
```

## 4. Security and compliance

- Limit GraphQL query depth and complexity to control denial-of-service risk.citeturn4search4
- Set deadlines in gRPC calls via `CallOptions` and attach OAuth 2.0 bearer tokens or mTLS certificates for sensitive APIs.citeturn4search5
- Pin certificates or leverage Azure API Management policies when distributing enterprise apps to protect credentials.

## 5. Testing and observability

- **GraphQL:** Strawberry Shake’s `MockTransport` simplifies unit tests; GraphQL.Client works well with snapshot testing or Postman collections.citeturn3search0turn3search1
- **gRPC:** Inspect request timing through `Grpc.Net.Client` logging and `dotnet-counters`; use `ghz` for load testing streams.citeturn4search5
- Feed both protocols into OpenTelemetry so HttpClientFactory-generated `ActivitySource` spans reach Application Insights.citeturn1search8turn1search0

## 6. Selection matrix

| Criteria | GraphQL | gRPC |
|----------|---------|------|
| Data shaping | Client specifies fields per request. | Contract-driven responses via `.proto`. |
| Network profile | Single request covers multiple aggregates. | Bi-directional streaming minimises latency. |
| Firewall compatibility | Works over standard HTTP/1.1. | HTTP/2 (or proxy via gRPC-Web). |
| Code generation | Strawberry Shake source generator. | `dotnet-grpc` generator. |
| Ideal usage | Shared mobile/web data models. | IoT telemetry, realtime updates. |

Armed with both playbooks, MAUI teams can match protocols to their scenario and provide reliable, observable connectivity from a single shared codebase.
