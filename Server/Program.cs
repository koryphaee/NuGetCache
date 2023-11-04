using System.Reflection;
using Server;
using Server.Data;

string assembly = Assembly.GetExecutingAssembly().Location;
string directory = Path.GetDirectoryName(assembly)!;
Directory.SetCurrentDirectory(directory);

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
NuGetCacheOptions options = builder.Configuration.Get<NuGetCacheOptions>() ?? throw new Exception("Can't bind options");

Console.WriteLine("Dumping configuration...");
Console.WriteLine($"UpstreamHost = '{options.UpstreamHost}'");
Console.WriteLine($"CacheDirectory = '{options.CacheDirectory}'");
Console.WriteLine($"PublicUrl = '{options.PublicUrl}'");
Console.WriteLine();

builder.Services
    .AddSingleton(options)
    .AddSingleton(new HttpClient())
    .AddSingleton<UpstreamInfoProvider>()
    .AddSingleton(provider => provider.GetRequiredService<UpstreamInfoProvider>().Info)
    .AddSingleton<PackageCacheMiddleware>()
    .AddReverseProxy()
    .ConfigureProxy(options);

WebApplication app = builder.Build();
app.MapManifest(options);
app.MapReverseProxy(proxy => proxy.UseMiddleware<PackageCacheMiddleware>());
await app.Services.GetRequiredService<UpstreamInfoProvider>().Initialize();
Console.WriteLine("Starting host...");
app.Run();