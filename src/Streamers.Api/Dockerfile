FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Streamers.Api/Streamers.Api.csproj", "src/Streamers.Api/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["src/streamer.ServiceDefaults/streamer.ServiceDefaults.csproj", "src/streamer.ServiceDefaults/"]
COPY ["src/Streamers.Features/Streamers.Features.csproj", "src/Streamers.Features/"]
RUN dotnet restore "src/Streamers.Api/Streamers.Api.csproj"
COPY . .
WORKDIR "/src/src/Streamers.Api"
RUN dotnet build "Streamers.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Streamers.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Streamers.Api.dll"]
