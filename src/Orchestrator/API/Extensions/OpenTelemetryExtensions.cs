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
        var serviceName = configuration.GetValue<string>("OpenTelemetry:ServiceName") ?? "Orchestrator";
        var serviceVersion = configuration.GetValue<string>("OpenTelemetry:ServiceVersion") ?? "1.0.0";
        var jaegerEndpoint = configuration.GetValue<string>("OpenTelemetry:JaegerEndpoint") ?? "http://localhost:14268/api/traces";

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
                .AddJaegerExporter(options =>
                {
                    options.AgentHost = configuration.GetValue<string>("OpenTelemetry:JaegerHost") ?? "localhost";
                    options.AgentPort = configuration.GetValue<int>("OpenTelemetry:JaegerPort") ?? 6831;
                    options.ExportProcessorType = OpenTelemetry.ExportProcessorType.Batch;
                }))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter("IntelliFlow.Orchestrator")
                .AddPrometheusExporter());

        return services;
    }

    /// <summary>
    /// Adds OpenTelemetry middleware to the pipeline.
    /// </summary>
    public static IApplicationBuilder UseOpenTelemetry(this IApplicationBuilder app)
    {
        // Use Prometheus exporter endpoint
        app.UsePrometheusScraping();
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
