FROM mcr.microsoft.com/dotnet/core/runtime-deps:2.2.4

LABEL maintainer "agentschap Informatie Vlaanderen <informatie.vlaanderen@vlaanderen.be>"

COPY / /app
WORKDIR /app

ENTRYPOINT ["./Be.Vlaanderen.Basisregisters.Redis.Populator"]
