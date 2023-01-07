using OpenTelemetry.Exporter.Rd.Model;

namespace OpenTelemetry.Exporter.Rd;

public interface IRdClient
{
    void SendActivity(RdActivity activity);
}