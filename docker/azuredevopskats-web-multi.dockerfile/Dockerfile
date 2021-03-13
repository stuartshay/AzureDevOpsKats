FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

RUN dotnet --version
RUN cat /etc/os-release

ARG BUILD_NUMBER=0
ENV BUILD_NUMBER ${BUILD_NUMBER}

COPY src /app/src
# COPY scripts /
COPY AzureDevOpsKats.sln /app/AzureDevOpsKats.sln
COPY NuGet.config /app/NuGet.config
COPY common.props /app/common.props

RUN curl -sL https://deb.nodesource.com/setup_14.x |  bash -
RUN apt-get install -y nodejs

# Install .NET Tools 
ENV PATH="/root/.dotnet/tools:${PATH}"
RUN dotnet tool install --global sempack --version 1.1.7057.22950
RUN dotnet tool install --global dotnet-dbinfo --version 1.4.0 
RUN dotnet tool list -g

WORKDIR /app
RUN dotnet restore /p:RestoreUseSkipNonexistentTargets="false"
#RUN sempack -f foo/foo.csproj --major --minor --build-version --revision


WORKDIR /app/src/AzureDevOpsKats.Web
RUN dotnet publish -o /publish -c Release -f net5.0 


# Runtime Image Stage
FROM mcr.microsoft.com/dotnet/aspnet:5.0

RUN cat /etc/os-release

# COPY --from=build-env /*.sh /

RUN apt-get update && apt-get install -y \
        curl \
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
