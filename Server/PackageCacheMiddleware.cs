using Server.Data;

namespace Server;

public class PackageCacheMiddleware
    (
        ILogger<PackageCacheMiddleware> logger,
        NuGetCacheOptions options,
        HttpClient httpClient,
        UpstreamInfo upstreamInfo
    )
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.StartsWithSegments(upstreamInfo.Segment, out PathString remaining) &&
            remaining.ToUriComponent().Split("/") is { Length: 4 } parts)
        {
            string id = parts[1];
            string version = parts[2];
            string name = parts[3];

            string directory = Path.Combine(options.CacheDirectory, id, version);
            Directory.CreateDirectory(directory);
            string file = Path.Combine(directory, name);

            if (!File.Exists(file))
            {
                string uri = upstreamInfo.OriginalUrl + remaining;
                logger.LogInformation("Downloading missing package {uri}", uri);
                await using Stream downloadStream = await httpClient.GetStreamAsync(uri);
                await using FileStream fileStream = File.OpenWrite(file);
                await downloadStream.CopyToAsync(fileStream);
            }

            logger.LogInformation("Sending cached file {path}", file);
            await context.Response.SendFileAsync(file);
            return;
        }

        await next(context);
    }
}
