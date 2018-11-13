# Build Stage
FROM microsoft/dotnet:2.1-sdk

RUN apt-get update && apt-get install -y \
    tree \
 && rm -rf /var/lib/apt/lists/*

COPY .  /app

WORKDIR /app

RUN dotnet restore
