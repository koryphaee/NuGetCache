using Server.Data;
using Yarp.ReverseProxy.Configuration;

namespace Server;

public static class Extensions
{
    public static void ConfigureProxy(this IReverseProxyBuilder builder, NuGetCacheOptions options)
    {
        var clusters = new List<ClusterConfig>
        {
            new()
            {
                ClusterId = "defaultCluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "defaultDestination", new DestinationConfig { Address = options.UpstreamHost } }
                }
            }
        };

        var routes = new List<RouteConfig>
        {
            new()
            {
                RouteId = "defaultRoute",
                Match = new RouteMatch
                {
                    Path = "{**catch-all}"
                },
                ClusterId = clusters.Single().ClusterId
            }
        };

        builder.LoadFromMemory(routes, clusters);
    }

    public static void MapManifest(this IEndpointRouteBuilder builder, NuGetCacheOptions options)
    {
        builder.MapGet(options.ManifestPath, (UpstreamInfo upstreamInfo) => Results.Text(upstreamInfo.ManifestJson, "application/json"));
    }
}
