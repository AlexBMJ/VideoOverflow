Write-Host "Removing user-secrets"  
    cd .\VideoOverflow.Server\
    dotnet user-secrets remove ConnectionStrings:VideOverflow
    cd..
    cd .\VideoOverflow.Infrastructure\
    cd..
   