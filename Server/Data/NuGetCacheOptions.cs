namespace Server.Data;

public class NuGetCacheOptions
{
    public required string PublicUrl { get; set; }

    public required string UpstreamHost { get; set; }

    public required string CacheDirectory { get; set; }

    public string ManifestPath => "/v3/index.json";
}