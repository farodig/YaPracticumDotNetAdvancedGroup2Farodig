using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Compact;

namespace EventService.Presentation.ConfigurationBuilders
{
    internal static class LoggerBuilder
    {
        public static void ConfigureLog(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((ctx, cfg) =>
                cfg.ReadFrom.Configuration(ctx.Configuration)
                    .WriteTo.Console(new CompactJsonFormatter()));
        }

        public static void ConfigureTelemetry(this WebApplicationBuilder builder)
        {
            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName: "events-service"))
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddOtlpExporter(o => o.Endpoint = new Uri(builder.Configuration["Otlp:Endpoint"]!)))
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter());
        }
    }
}
