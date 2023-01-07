using System.Net;
using JetBrains.Collections.Viewable;
using JetBrains.Lifetimes;
using JetBrains.Rd;
using JetBrains.Rd.Impl;
using JetBrains.Rd.Reflection;
using OpenTelemetry.Exporter.Rd.Model;

namespace OpenTelemetry.Exporter.Rd;

public class RdClient : IRdClient, IDisposable
{
    private const string OtelExporterRdPort = "OTEL_EXPORTER_RD_PORT";
    private const int DefaultProtocolPort = 8686;

    private readonly LifetimeDefinition _lifetimeDefinition;
    private IOpenTelemetryHostModel? _model;

    public RdClient()
    {
        _lifetimeDefinition = new LifetimeDefinition();
        var lifetime = _lifetimeDefinition.Lifetime;
        var scheduler = SingleThreadScheduler.RunOnSeparateThread(lifetime, "OpenTelemetry.Exporter.Rd Scheduler");
        var envVarPort = Environment.GetEnvironmentVariable(OtelExporterRdPort);
        var port = string.IsNullOrEmpty(envVarPort) ? DefaultProtocolPort : int.Parse(envVarPort);
        var wire = new SocketWire.Client(
            lifetime,
            scheduler,
            new IPEndPoint(IPAddress.Loopback, port),
            "OpenTelemetry.Exporter.Rd Client"
        );
        var reflectionSerializers = new ReflectionSerializersFacade();
        var serializers = new Serializers(TaskScheduler.Default, reflectionSerializers.Registrar);
        var protocol = new Protocol(
            "OpenTelemetry.Exporter.Rd Client",
            serializers,
            new Identities(IdKind.Client),
            scheduler,
            wire,
            lifetime
        );
        scheduler.Queue(() => RunRdClient(reflectionSerializers, protocol, lifetime));
    }

    private void RunRdClient(ReflectionSerializersFacade reflectionSerializers, Protocol protocol, Lifetime lifetime)
    {
        _model = reflectionSerializers.ActivateProxy<IOpenTelemetryHostModel>(lifetime, protocol);
    }

    public void SendActivity(RdActivity activity)
    {
        _model?.OnActivity.Fire(activity);
    }

    public void Dispose()
    {
        _lifetimeDefinition.Dispose();
    }
}