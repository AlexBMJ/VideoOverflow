docker run --name postgres_db -p 5001:5432 -v postgres_data -e POSTGRES_PASSWORD=Test1234 -e POSTGRES_DB=$database -d postgres
$connectionString = "Server=localhost;Database=VideoOverflow;Port=5001;UserId=postgres;Password=Test1234"
Set-Clipboard $password

Write-Host "Setting up connectionString...."
    cd .\VideoOverflow.Server\
    dotnet user-secrets set ConnectionStrings:VideoOverflow "$connectionString"

    cd..
    cd .\VideoOverflow.Infrastructure\
    dotnet user-secrets set ConnectionStrings:VideoOverflow "$connectionString"
    dotnet ef migrations add VideoOverflowMigration
    dotnet ef database update
    
Write-Host "Starting Application...."    
    cd..
    cd .\VideoOverflow.Server\
    dotnet watch
    
  
    
