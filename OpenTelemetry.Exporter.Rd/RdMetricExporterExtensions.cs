using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;

namespace OpenTelemetry.Exporter.Rd;

public static class RdMetricExporterExtensions
{
    public static MeterProviderBuilder AddRdExporter(this MeterProviderBuilder builder) =>
        builder switch
        {
            null => throw new ArgumentNullException(nameof(builder)),
            IDeferredMeterProviderBuilder deferredTracerProviderBuilder => deferredTracerProviderBuilder.Configure(
                (sp, b) =>
                {
                    var client = sp.GetService<IRdClient>() ??
                                 throw new ApplicationException("Unable to get IRdClient");
                    AddRdExporter(b, client);
                }),
            _ => builder
        };

    private static void AddRdExporter(MeterProviderBuilder builder, IRdClient client)
    {
        var exporter = new RdMetricExporter(client);

        var metricReader = new PeriodicExportingMetricReader(exporter);

        builder.AddReader(metricReader);
    }
}