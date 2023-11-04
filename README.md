# NuGetCache

Reverse proxy for NuGet that caches packages.

DockerHub: https://hub.docker.com/repository/docker/koryphaee/nuget-cache

## Why does this exist?

I wanted a slim proxy in front of <https://nuget.org> that simply caches packages.
Searching and any other requests should just be patched through.
The goal was to be able to restore packages in an offline scenario.
NuGet packages are also cached on the local machine but I wanted a more persistent storage on my NAS.

I tried [BaGet](https://github.com/loic-sharma/BaGet) with [read-through caching](https://loic-sharma.github.io/BaGet/configuration/#enable-read-through-caching) but it would only search in it's own database resulting in only cached packages being found.
I also tried looking for other solutions but was unable to find anything.

## How does it work?

I tried to keep it as minimal as possible. It uses [ASP.NET Core](https://dotnet.microsoft.com/en-us/learn/aspnet/what-is-aspnet-core) with the proxying being done by [YARP](https://microsoft.github.io/reverse-proxy).

NuGet has a manifest (<https://api.nuget.org/v3/index.json>) which contains the URLs to different services.
NuGetCache queries that manifest, replaces the service for downloading packages with it's own [PublucUrl](#publicurl) and serves that new version.
Requests to download packages are intercepted and served from cache if possible. Otherwise the package is downloaded from the [UpstreamHost](#upstreamhost) and added to the cache.
All other requests are proxies to the [UpstreamHost](#upstreamhost).
The cache itself itself is just a directory structure like this /\<name\>/\<version\>/\<name\>.\<version\>.nupkg.

## How do I use it?

NuGetCache is intended to be run inside a Docker container. I run it behing another reverse proxy: [Swag](https://github.com/linuxserver/docker-swag). This enables me to access it via https so Visual Studio doesn't complain.

To run it you need the following settings:

### Ports

NuGetCache binds to port 80 inside the container.

### Volumes

NuGetCache saves packages in /mnt/cache.
If you want the cache to persist you should map a volume there.

### Variables

#### GID
The group ID to run the script as.

#### UID

The user ID to run the script as.

#### UMASK

The umask to tun the script under.

#### PublicUrl

The URL under which the instance is reachable by Visual Studio/dotnet.exe/etc.

#### UpstreamHost

The NuGet host to proxy. Defaults to <https://api.nuget.org>