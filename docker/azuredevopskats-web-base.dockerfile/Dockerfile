# Build Stage
FROM microsoft/dotnet:2.1-sdk

RUN apt-get update && apt-get install -y \
    tree \
 && rm -rf /var/lib/apt/lists/*

##RUN echo "export PATH=\"\$PATH:\$HOME/.dotnet/tools\"" >> ~/.bash_profile

RUN cat << \EOF >> ~/.bash_profile \
    export PATH="$PATH:/root/.dotnet/tools" \
    EOF

RUN dotnet tool install -g dotnet-setversion

RUN dotnet tool list -g

COPY .  /app

WORKDIR /app


RUN dotnet restore
