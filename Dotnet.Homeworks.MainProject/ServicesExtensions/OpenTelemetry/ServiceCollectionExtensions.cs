using Dotnet.Homeworks.MainProject.Configuration;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;


namespace Dotnet.Homeworks.MainProject.ServicesExtensions.OpenTelemetry;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services,
        OpenTelemetryConfig openTelemetryConfiguration)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder => builder
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(otlp => otlp.Endpoint = new Uri(openTelemetryConfiguration.OtlpExporterEndpoint))
                .AddJaegerExporter())
            .WithMetrics(builder => builder
                .AddMeter("Dotnet.Homeworks.GetHelloWorldMetrics")
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter());

        return services;
    }
}