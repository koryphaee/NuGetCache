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
COPY Server/launch.sh .
RUN chmod a+rx launch.sh
ENV UpstreamHost="https://api.nuget.org"
ENV CacheDirectory="/mnt/cache/"
ENV Urls="http://+:80",
ENTRYPOINT /app/launch.sh