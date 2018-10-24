FROM microsoft/dotnet:2.1.402-sdk AS build-env

RUN dotnet --version
RUN cat /etc/os-release

ARG BUILD_NUMBER=0
ENV BUILD_NUMBER ${BUILD_NUMBER}

COPY src /app/src
# COPY scripts /
COPY AzureDevOpsKats.sln /app/AzureDevOpsKats.sln
COPY NuGet.config /app/NuGet.config

WORKDIR /app
RUN dotnet restore /p:RestoreUseSkipNonexistentTargets="false"

WORKDIR /app/src/AzureDevOpsKats.Web
RUN dotnet publish -o /publish -c Release -f netcoreapp2.1 -r debian.9-x64

# Runtime Image Stage
FROM microsoft/dotnet:2.1-aspnetcore-runtime

RUN cat /etc/os-release

COPY --from=build-env /*.sh /

RUN apt-get update && apt-get install -y \
        curl \
        apt-transport-https \
        net-tools \
        dnsutils \ 
        iputils-ping \
        nmap \ 
        telnet \
    && rm -rf /var/lib/apt/lists/* \
    && curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /wait_for_it.sh \
    && chmod +x /*.sh

WORKDIR /publish
COPY --from=build-env /publish .

ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT Staging

# Open up port
EXPOSE 8150 3333
ENTRYPOINT ["dotnet", "AzureDevOpsKats.Web.dll"]