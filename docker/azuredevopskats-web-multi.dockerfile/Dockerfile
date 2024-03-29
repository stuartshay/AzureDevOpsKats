ARG APPLICATION_NAME='azuredevopskats.web'
ARG BUILD_DATE='1/1/2022'
ARG BUILD_NUMBER=0

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

RUN dotnet --version
RUN cat /etc/os-release

ARG BUILD_NUMBER
ENV BUILD_NUMBER ${BUILD_NUMBER}

COPY src /app/src
# COPY scripts /
COPY AzureDevOpsKats.sln /app/AzureDevOpsKats.sln
COPY NuGet.config /app/NuGet.config
COPY common.props /app/common.props

RUN curl -sL https://deb.nodesource.com/setup_16.x |  bash -
RUN apt-get install -y nodejs

# Install .NET Tools
ENV PATH="/root/.dotnet/tools:${PATH}"
RUN dotnet tool install --global dotnet-setversion --version 2.4.0
RUN dotnet tool list -g

WORKDIR /app
RUN dotnet restore /p:RestoreUseSkipNonexistentTargets="false"

WORKDIR /app/src/AzureDevOpsKats.Web

RUN setversion 6.0.1.${BUILD_NUMBER}
RUN dotnet publish AzureDevOpsKats.Web.csproj -o /publish -c Release -f net6.0

# Runtime Image Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0

ARG APPLICATION_NAME
ARG BUILD_DATE
ENV BUILD_DATE ${BUILD_DATE}
ARG BUILD_NUMBER
ENV BUILD_NUMBER ${BUILD_NUMBER}

LABEL org.label-schema.application=${APPLICATION_NAME}
LABEL org.label-schema.build-date=${BUILD_DATE}
LABEL org.label-schema.version=6.0.1.${BUILD_NUMBER}

RUN cat /etc/os-release

# COPY --from=build-env /*.sh /

RUN apt-get update && apt-get install -y \
    curl \
    jq \
    procps \
    tree \
    && rm -rf /var/lib/apt/lists/* \
    && curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /wait_for_it.sh \
    && chmod +x /*.sh

WORKDIR /publish
COPY --from=build-env /publish .

ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT Docker

# Open up port
#EXPOSE 5000 3333
EXPOSE 5000
ENTRYPOINT ["dotnet", "AzureDevOpsKats.Web.dll"]
