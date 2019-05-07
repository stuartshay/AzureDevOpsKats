FROM mcr.microsoft.com/windows/nanoserver:1809 AS build-env

RUN dotnet --version

ARG BUILD_NUMBER=0
ENV BUILD_NUMBER ${BUILD_NUMBER}

COPY src /app/src
COPY AzureDevOpsKats.sln /app/AzureDevOpsKats.sln
COPY NuGet.config /app/NuGet.config
COPY common.props /app/common.props

WORKDIR /app
RUN dotnet restore /p:RestoreUseSkipNonexistentTargets="false"

WORKDIR /app/src/AzureDevOpsKats.Web
RUN dotnet publish -o /publish -c Release -f netcoreapp2.2 -r win10-x64

# Runtime Image Stage
FROM microsoft/dotnet:2.1.6-aspnetcore-runtime-nanoserver-1809

WORKDIR /publish
COPY --from=build-env /publish .

ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT Staging

# Open up port
EXPOSE 5000 3333

#CMD ["powershell"]
ENTRYPOINT ["dotnet", "AzureDevOpsKats.Web.dll"]
