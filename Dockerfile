FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app

COPY *.csproj ./
COPY *.cs ./

RUN dotnet build linq2db-repro.csproj -c Debug -o dist

ENTRYPOINT [ "bash" ]
