FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything else and build
COPY . ./
RUN rm -rf VideoOverflow.Infrastructure/Migrations
RUN dotnet restore
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet user-secrets init --project VideoOverflow.Infrastructure
RUN dotnet user-secrets set "ConnectionStrings:VideoOverflow" "Server=db;Database=VideoOverflow;UserId=postgres;Password=ProdPassword" --project VideoOverflow.Infrastructure
RUN dotnet ef migrations add VideoOverflowMigration --project VideoOverflow.Infrastructure
RUN dotnet publish -c Release -o out
RUN dotnet dev-certs https -ep ./out/cert.pfx -p CertPassword1234

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "VideoOverflow.Server.dll"]
