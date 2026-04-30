using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Protos;

var builder = WebApplication.CreateBuilder(args);

// gRPC client
var backendUrl = builder.Configuration["BackendService:Url"] ?? "http://localhost:5001";

builder.Services.AddGrpcClient<OrderService.OrderServiceClient>(o =>
{
    o.Address = new Uri(backendUrl);
});

// OpenTelemetry
var otelEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r
        .AddService("FrontendApi")
        .AddContainerDetector())
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(o => o.Endpoint = new Uri(otelEndpoint)))
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(o => o.Endpoint = new Uri(otelEndpoint)));

builder.Logging.AddOpenTelemetry(o =>
{
    o.IncludeScopes = true;
    o.IncludeFormattedMessage = true;
    o.AddOtlpExporter(e => e.Endpoint = new Uri(otelEndpoint));
});

var app = builder.Build();

app.MapGet("/api/orders/{id:int}", async (int id, OrderService.OrderServiceClient client, ILogger<Program> logger) =>
{
    logger.LogInformation("Fetching order {OrderId}", id);
    var reply = await client.GetOrderAsync(new GetOrderRequest { Id = id });
    return Results.Ok(new
    {
        reply.Id,
        reply.Item,
        reply.Quantity,
        reply.Status,
        reply.CreatedAt
    });
});

app.MapPost("/api/orders", async (PlaceOrderRequest request, OrderService.OrderServiceClient client, ILogger<Program> logger) =>
{
    logger.LogInformation("Placing order for Item={Item}, Quantity={Quantity}", request.Item, request.Quantity);
    var reply = await client.PlaceOrderAsync(request);
    return Results.Created($"/api/orders/{reply.Id}", new
    {
        reply.Id,
        reply.Item,
        reply.Quantity,
        reply.Status,
        reply.CreatedAt
    });
});

app.Run();
