using JetBrains.Collections.Viewable;
using JetBrains.Rd.Reflection;

namespace OpenTelemetry.Exporter.Rd.Model;

[RdRpc]
public interface IOpenTelemetryHostModel
{
    ISignal<RdActivity> OnActivity { get; }
}

[RdScalar]
public class RdActivity
{
    public string? Id { get; set; }
    public string? ParentId { get; set; }
    public string TraceId { get; set; }
    public string DisplayName { get; set; }
    public string OperationName { get; set; }
    public string SourceName { get; set; }
    public long Duration { get; set; }
    public List<RdTag> Tags { get; set; }
}

[RdScalar]
public class RdTag
{
    public string Key { get; set; }
    public string? Value { get; set; }
}