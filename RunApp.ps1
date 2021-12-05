$password = New-Guid
$database = "VideoOverflow"
docker run --name postgres_db -p 5001:5432 -v postgres_data -e POSTGRES_PASSWORD=$password -e POSTGRES_DB=$database -d postgres
$connectionString = "Host=localhost;Database=$database;Username=postgres;Password=$password"
Set-Clipboard $connectionString

Write-Host "Setting up connectionString"
    cd .\VideoOverflow.Server\
    dotnet user-secrets set "VideoOverflow" "$connectionString"
    
    cd..
    cd .\VideoOverflow.Infrastructure\
    dotnet ef migrations add VideoOverflowMigration
    dotnet ef database update
    
  Write-Host "Deleting user-secrets"  
    cd..
    cd .\VideoOverflow.Server\
    dotnet user-secrets remove VideoOverflow
    cd..
    cd .\VideoOverflow.Infrastructure\
     Write-Host "Deleting Migrations" 
    rm migrations
   

