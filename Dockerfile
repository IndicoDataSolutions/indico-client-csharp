FROM mcr.microsoft.com/dotnet/sdk:5.0.301 as build

# TODO copy .csproj files separately before running docker restore to take advantage of docker build cache
COPY . ./
RUN dotnet restore 
RUN dotnet build --no-restore

CMD ["sleep","infinity"]