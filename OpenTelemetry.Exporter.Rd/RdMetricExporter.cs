using OpenTelemetry.Metrics;

namespace OpenTelemetry.Exporter.Rd;

public class RdMetricExporter : BaseExporter<Metric>
{
    private readonly IRdClient _client;

    public RdMetricExporter(IRdClient client)
    {
        _client = client;
    }

    public override ExportResult Export(in Batch<Metric> batch)
    {
        foreach (var metric in batch)
        {
            ExportMetric(metric);
        }
        
        return ExportResult.Success;
    }

    private void ExportMetric(Metric metric)
    {
    }
}