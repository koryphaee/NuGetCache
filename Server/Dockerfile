FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Server/Server.csproj Server/
RUN dotnet restore Server/Server.csproj
COPY . .
WORKDIR /src/Server
RUN dotnet build "Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
EXPOSE 80
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT exec setpriv --reuid=${PUID} --regid=${PGID} --clear-groups dotnet Server.dll UpstreamHost=https://api.nuget.org CacheDirectory=/mnt/cache/ Urls=http://+:80 PublicUrl=$PublicUrl