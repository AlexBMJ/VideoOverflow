$password = New-Guid
$database = "VideoOverflow"
docker run --name postgres_db -p 5001:5432 -v postgres_data -e POSTGRES_PASSWORD=$password -e POSTGRES_DB=$database -d postgres
$connectionString = "Host=localhost;Database=$database;Username=postgres;Password=$password"

Write-Host "Configuring Connection String"
    cd .\VideoOverflow.Server\
    dotnet user-secrets init
    dotnet user-secrets set "VideoOverflow" "$connectionString"
    
    cd..
    cd .\VideoOverflow.Infrastructure\
    dotnet ef migrations add VideoOverflowMigration
    dotnet ef database update
