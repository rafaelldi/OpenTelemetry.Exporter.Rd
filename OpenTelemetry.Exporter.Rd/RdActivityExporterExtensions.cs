using OpenTelemetry.Trace;

namespace OpenTelemetry.Exporter.Rd;

public static class RdActivityExporterExtensions
{
    public static TracerProviderBuilder AddRdExporter(this TracerProviderBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddProcessor(new SimpleActivityExportProcessor(new RdActivityExporter()));
    }
}