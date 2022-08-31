using JetBrains.Collections.Viewable;
using JetBrains.Rd.Reflection;

namespace OpenTelemetry.Exporter.Rd.Model;

[RdRpc]
public interface IOpenTelemetryHostModel
{
    ISignal<AspNetCoreActivity> OnActivity { get; }
}

[RdScalar]
public class AspNetCoreActivity
{
    public string DisplayName { get; private set; }
    public string? Method { get; private set; }
    public string? Target { get; private set; }
    public string? Route { get; private set; }
    public int? StatusCode { get; private set; }
    public long Duration { get; private set; }

    public AspNetCoreActivity(
        string displayName,
        string? method,
        string? target,
        string? route,
        int? statusCode,
        long duration
    )
    {
        DisplayName = displayName;
        Method = method;
        Target = target;
        Route = route;
        StatusCode = statusCode;
        Duration = duration;
    }
}