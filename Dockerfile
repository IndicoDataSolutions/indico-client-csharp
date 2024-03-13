FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR indico-client-csharp
COPY . ./
RUN dotnet restore
RUN dotnet build --no-restore -c Release
RUN dotnet pack --no-build -c Release ./IndicoV2/IndicoV2.csproj

CMD ["sleep", "infinity"]