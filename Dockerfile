FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
ARG EXPOSE_PORTS="80 443"
EXPOSE $EXPOSE_PORTS

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG CONFIG="Release"
WORKDIR /src
COPY /src .
WORKDIR "/src/SocialMedia.WebAPI"
RUN dotnet restore "SocialMedia.WebAPI.csproj"
RUN dotnet build "SocialMedia.WebAPI.csproj" --configuration $CONFIG --output /app/build

FROM build AS publish
RUN dotnet publish "SocialMedia.WebAPI.csproj" --configuration $CONFIG --output /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN apt-get update
RUN apt-get install -y curl

ENTRYPOINT ["dotnet", "SocialMedia.WebAPI.dll"]

HEALTHCHECK --interval=2s --timeout=30s \
  CMD curl https://localhost/health --insecure || exit 1
