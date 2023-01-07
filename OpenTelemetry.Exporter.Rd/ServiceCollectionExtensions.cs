using Microsoft.Extensions.DependencyInjection;

namespace OpenTelemetry.Exporter.Rd;

public static class ServiceCollectionExtensions
{
    public static void AddRdClient(this IServiceCollection services)
    {
        services.AddSingleton<IRdClient, RdClient>();
    }
}