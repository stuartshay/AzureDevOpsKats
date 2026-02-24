# Stage 1: Build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
WORKDIR /src

COPY Directory.Build.props global.json ./
COPY AzureDevOpsKats.sln ./
COPY src/AzureDevOpsKats.Web/AzureDevOpsKats.Web.csproj src/AzureDevOpsKats.Web/
COPY src/AzureDevOpsKats.Client/AzureDevOpsKats.Client.csproj src/AzureDevOpsKats.Client/
RUN dotnet restore src/AzureDevOpsKats.Web/AzureDevOpsKats.Web.csproj -a $TARGETARCH

COPY src/ src/
RUN dotnet publish src/AzureDevOpsKats.Web/AzureDevOpsKats.Web.csproj \
    -a $TARGETARCH \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

LABEL org.opencontainers.image.title="AzureDevOpsKats" \
    org.opencontainers.image.description="Cat photo gallery — .NET 10 Blazor" \
    org.opencontainers.image.source="https://github.com/stuartshay/AzureDevOpsKats"

WORKDIR /app

RUN mkdir -p /app/data /app/wwwroot/uploads && \
    chown -R app:app /app

COPY --from=build --chown=app:app /app/publish .

USER app

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "AzureDevOpsKats.Web.dll"]
