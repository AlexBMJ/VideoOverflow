FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything else and build
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

RUN dotnet dev-certs https -ep ./out/cert.pfx -p Test1234

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "VideoOverflow.Server.dll"]