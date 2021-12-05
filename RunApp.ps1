$password = New-Guid
$database = "VideoOverflow"
docker run --name postgres_db -p 5001:5432 -v postgres_data -e POSTGRES_PASSWORD=$password -e POSTGRES_DB=$database -d postgres
$connectionString = "Server=localhost;Database=$database;UserId=postgres;Password=$password"
Set-Clipboard $password

Write-Host "Setting up connectionString"
    cd .\VideoOverflow.Server\
    dotnet user-secrets set "VideoOverflow" "$connectionString"
    
    cd..
    cd .\VideoOverflow.Infrastructure\
    dotnet ef migrations add VideoOverflowMigration
    dotnet ef database update
    
Write-Host "Starting Application...."    
    cd..
    cd .\VideoOverflow.Server\
    dotnet watch
    
  
    
