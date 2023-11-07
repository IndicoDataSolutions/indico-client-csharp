FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine
COPY . /indico-client-csharp
WORKDIR /indico-client-csharp
RUN apk update && apk add jq vim bash
CMD ["sleep", "infinity"]
