using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Nihlen.Common.Telemetry;

// https://docs.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-instrumentation-walkthroughs
public static class TelemetryExtensions
{
    /// <summary>
    /// Setup OpenTelemetry tracing and metrics to the specified endpoint.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="serviceName">Service name (ex. Nihlen.Common)</param>
    /// <param name="serviceVersion">Service version (ex. 1.0.0)</param>
    /// <param name="otlpEndpoint">The OpenTelemetry protocol endpoint URL. Default is http://localhost:4317 with gRPC protocol.</param>
    /// <returns></returns>
    public static IServiceCollection? AddCustomTelemetry(this IServiceCollection? services, string? serviceName = null, string? serviceVersion = null, string? otlpEndpoint = null)
    {
        if (services is null)
            return null;

        try
        {
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddTelemetrySdk()
                .AddEnvironmentVariableDetector();

            serviceVersion ??= Assembly.GetExecutingAssembly().GetName().Version?.ToString();

            if (serviceName is not null && serviceVersion is not null)
            {
                Telemetry.ActivitySource = new ActivitySource(serviceName, serviceVersion);

                resourceBuilder = ResourceBuilder.CreateDefault()
                    .AddService(
                        serviceName: serviceName,
                        serviceVersion: serviceVersion
                    );
            }

            Action<OtlpExporterOptions> otlpOptions = o =>
            {
                o.Endpoint = new Uri(otlpEndpoint ?? "http://localhost:4317");
                o.Protocol = OtlpExportProtocol.Grpc;
            };

            services.AddOpenTelemetryTracing(b => b
                .AddSource(serviceName)
                .SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter(otlpOptions)
                .AddAspNetCoreInstrumentation(o =>
                {
                    o.RecordException = true;
                })
                .AddHttpClientInstrumentation(o =>
                {
                    o.RecordException = true;
                })
                .AddSqlClientInstrumentation(o =>
                {
                    o.SetDbStatementForText = true;
                    o.SetDbStatementForStoredProcedure = true;
                    o.EnableConnectionLevelAttributes = true;
                    o.RecordException = true;
                })
            );

            services.AddOpenTelemetryMetrics(b => b
                .AddMeter(serviceName)
                .SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter(otlpOptions)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
            );
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to setup OpenTelemetry tracing and metrics for {ServiceName} {ServiceVersion} {OtlpEndpoint}", serviceName, serviceVersion, otlpEndpoint);
        }

        return services;
    }
}
