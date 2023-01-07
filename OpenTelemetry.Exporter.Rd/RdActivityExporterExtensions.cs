using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace OpenTelemetry.Exporter.Rd;

public static class RdActivityExporterExtensions
{
    public static TracerProviderBuilder AddRdExporter(this TracerProviderBuilder builder) =>
        builder switch
        {
            null => throw new ArgumentNullException(nameof(builder)),
            IDeferredTracerProviderBuilder deferredTracerProviderBuilder => deferredTracerProviderBuilder.Configure(
                (sp, b) =>
                {
                    var client = sp.GetService<IRdClient>() ??
                                 throw new ApplicationException("Unable to get IRdClient");
                    AddRdExporter(b, client);
                }),
            _ => builder
        };

    private static void AddRdExporter(TracerProviderBuilder builder, IRdClient rdClient)
    {
        var exporter = new RdActivityExporter(rdClient);
        builder.AddProcessor(new SimpleActivityExportProcessor(exporter));
    }
}