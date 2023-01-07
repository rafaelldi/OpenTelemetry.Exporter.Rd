using System.Diagnostics;
using OpenTelemetry.Exporter.Rd.Model;

namespace OpenTelemetry.Exporter.Rd;

public class RdActivityExporter : BaseExporter<Activity>
{
    private readonly IRdClient _rdClient;

    public RdActivityExporter(IRdClient rdClient)
    {
        _rdClient = rdClient;
    }

    public override ExportResult Export(in Batch<Activity> batch)
    {
        foreach (var activity in batch)
        {
            ExportActivity(activity);
        }

        return ExportResult.Success;
    }

    private void ExportActivity(Activity activity)
    {
        var rdActivity = new RdActivity
        {
            Id = activity.Id,
            ParentId = activity.ParentId,
            TraceId = activity.TraceId.ToString(),
            DisplayName = activity.DisplayName,
            OperationName = activity.OperationName,
            SourceName = activity.Source.Name,
            Duration = activity.Duration.Ticks,
            Tags = activity.Tags
                .Select(it => new RdTag
                {
                    Key = it.Key,
                    Value = it.Value
                })
                .ToList()
        };

        _rdClient.SendActivity(rdActivity);
    }
}