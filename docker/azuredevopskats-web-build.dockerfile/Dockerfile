FROM stuartshay/azuredevopskats:2.2.2-base AS build-env

ARG BUILD_NUMBER=0
ENV BUILD_NUMBER ${BUILD_NUMBER}

COPY src /app/src
COPY common.props /app/common.props
COPY NuGet.config /app/NuGet.config
COPY AzureDevOpsKats.sln /app/AzureDevOpsKats.sln

WORKDIR /app/src/AzureDevOpsKats.Web

RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs

RUN dotnet tool list -g
RUN dotnet publish -o /publish -c Release -f netcoreapp2.2

# Runtime Image Stage
FROM microsoft/dotnet:2.2-aspnetcore-runtime

ARG VCS_REF

LABEL org.label-schema.vcs-ref=$VCS_REF \
    org.label-schema.vcs-url="e.g. https://github.com/microscaling/microscaling"

RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    && rm -rf /var/lib/apt/lists/* \
    && curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /wait_for_it.sh \
    && chmod +x /wait_for_it.sh

WORKDIR /publish
COPY --from=build-env /publish .

# Set environment variables
ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT Staging

# Open up port
EXPOSE 5000 3333

ENTRYPOINT ["dotnet", "AzureDevOpsKats.Web.dll"]
