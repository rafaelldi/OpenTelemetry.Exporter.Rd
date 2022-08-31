using System.Diagnostics;
using System.Net;
using JetBrains.Collections.Viewable;
using JetBrains.Lifetimes;
using JetBrains.Rd;
using JetBrains.Rd.Impl;
using JetBrains.Rd.Reflection;
using OpenTelemetry.Exporter.Rd.Model;

namespace OpenTelemetry.Exporter.Rd;

public class RdActivityExporter : BaseExporter<Activity>
{
    private const string OtelExporterRdPort = "OTEL_EXPORTER_RD_PORT";
    private const int DefaultProtocolPort = 8686;

    private const string AspNetCoreOperationName = "Microsoft.AspNetCore.Hosting.HttpRequestIn";

    private readonly LifetimeDefinition _lifetimeDefinition;
    private IOpenTelemetryHostModel? _model;

    public RdActivityExporter()
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

    public override ExportResult Export(in Batch<Activity> batch)
    {
        foreach (var activity in batch)
        {
            if (activity is null || activity.OperationName != AspNetCoreOperationName)
            {
                continue;
            }

            ExportAspNetCoreActivity(activity);
        }

        return ExportResult.Success;
    }

    private void ExportAspNetCoreActivity(Activity activity)
    {
        string? method = null, target = null, route = null;
        int? statusCode = null;

        foreach (var tag in activity.TagObjects)
        {
            switch (tag.Key)
            {
                case "http.method":
                    method = (string?)tag.Value;
                    break;
                case "http.target":
                    target = (string?)tag.Value;
                    break;
                case "http.route":
                    route = (string?)tag.Value;
                    break;
                case "http.status_code":
                    statusCode = (int?)tag.Value;
                    break;
            }
        }

        var aspNetCoreActivity = new AspNetCoreActivity(
            activity.DisplayName,
            method,
            target,
            route,
            statusCode,
            activity.Duration.Ticks
        );

        _model?.OnActivity.Fire(aspNetCoreActivity);
    }

    protected override void Dispose(bool disposing)
    {
        _lifetimeDefinition.Dispose();
    }
}