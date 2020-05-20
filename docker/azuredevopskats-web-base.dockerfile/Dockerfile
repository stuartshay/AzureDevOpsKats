# Build Stage
FROM mcr.microsoft.com/dotnet/core/sdk:3.1.300

RUN apt-get update && apt-get install -y \
    tree \
 && rm -rf /var/lib/apt/lists/*

RUN echo "export PATH=\"\$PATH:\$HOME/.dotnet/tools\"" >> ~/.bash_profile
RUN dotnet tool install -g dotnet-setversion

RUN dotnet tool list -g

COPY .  /app

WORKDIR /app

RUN dotnet restore
