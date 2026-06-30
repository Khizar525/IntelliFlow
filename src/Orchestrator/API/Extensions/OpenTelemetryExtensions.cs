using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace Orchestrator.API.Extensions;

/// <summary>
/// Extension methods for configuring OpenTelemetry tracing and metrics.
/// </summary>
public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Adds OpenTelemetry tracing and metrics to the service collection.
    /// </summary>
    public static IServiceCollection AddOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var serviceName = configuration.GetValue<string>("OpenTelemetry:ServiceName") ?? Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ?? "Orchestrator";
        var serviceVersion = configuration.GetValue<string>("OpenTelemetry:ServiceVersion") ?? "1.0.0";
        var otlpEndpoint = configuration.GetValue<string>("OpenTelemetry:OtlpEndpoint")
            ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")
            ?? "http://localhost:4317";

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: serviceName,
                    serviceVersion: serviceVersion,
                    serviceInstanceId: Environment.MachineName)
                .AddAttributes(new Dictionary<string, object>
                {
                    { "deployment.environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "development" },
                    { "service.owner", "M. Khizar Akram" }
                }))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.Filter = (httpContext) =>
                    {
                        // Filter out health check endpoints from tracing
                        var path = httpContext.Request.Path.Value ?? "";
                        return !path.Contains("/health") && !path.Contains("/swagger");
                    };
                })
                .AddHttpClientInstrumentation(options =>
                {
                    options.RecordException = true;
                })
                .AddSource("IntelliFlow.Orchestrator")
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpEndpoint);
                }))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter("IntelliFlow.Orchestrator"));

        return services;
    }

    /// <summary>
    /// Adds OpenTelemetry middleware to the pipeline.
    /// </summary>
    public static IApplicationBuilder UseOpenTelemetry(this IApplicationBuilder app)
    {
        // OpenTelemetry is configured in services
        return app;
    }
}

/// <summary>
/// Custom activity source for IntelliFlow pipeline operations.
/// </summary>
public static class PipelineActivitySource
{
    public const string ServiceName = "IntelliFlow.Pipeline";
    
    private static readonly System.Diagnostics.ActivitySource Source = new(ServiceName);
    
    public static System.Diagnostics.Activity? StartActivity(string operationName)
    {
        return Source.StartActivity(operationName);
    }
    
    public static System.Diagnostics.Activity? StartActivity(string operationName, System.Diagnostics.ActivityKind kind)
    {
        return Source.StartActivity(operationName, kind);
    }
}
