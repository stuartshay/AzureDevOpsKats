FROM stuartshay/azuredevopskats:2.1.1-base AS build-env

ARG BUILD_NUMBER=0
ENV BUILD_NUMBER ${BUILD_NUMBER}

COPY src /app/src
COPY _stylecop /app/_stylecop
COPY NuGet.Config /app/NuGet.Config
COPY AzureDevOpsKats.sln /app/AzureDevOpsKats.sln

WORKDIR /app/src/AzureDevOpsKats

#RUN dotnet setversion 2.1.2.${BUILD_NUMBER};
RUN dotnet publish -o /publish -c Release -f netcoreapp2.1 -r debian.9-x64

# Runtime Image Stage
FROM microsoft/dotnet:2.1-aspnetcore-runtime

RUN apt-get update && apt-get install -y \
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
