using System.Text.Json;
using Server.Data;

namespace Server;

public class UpstreamInfoProvider
    (
        HttpClient httpClient, 
        NuGetCacheOptions options
    )
{
    public UpstreamInfo Info { get; private set; } = null!;

    public async Task Initialize()
    {
        ManifestDto manifest = await httpClient.GetFromJsonAsync<ManifestDto>(options.UpstreamHost + options.ManifestPath) ?? throw new Exception("Failed to query upstream manifest");
        ResourceDto resource = manifest.Resources.Single(r => r.Type == "PackageBaseAddress/3.0.0");
        string originalUrl = resource.Id.TrimEnd('/');

        // replace the address to the package service with our own URL so we can intercept it
        var uriBuilder = new UriBuilder(resource.Id);
        var ownUri = new Uri(options.PublicUrl);
        uriBuilder.Scheme = ownUri.Scheme;
        uriBuilder.Host = ownUri.Host;
        uriBuilder.Port = ownUri.Port;
        resource.Id = uriBuilder.Uri.ToString();

        // indent just like nuget.org
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        string manifestJson = JsonSerializer.Serialize(manifest, jsonOptions);
        string segment = uriBuilder.Path.TrimEnd('/');
        Info = new UpstreamInfo(manifestJson, segment, originalUrl);
    }
}
