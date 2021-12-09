Write-Host "Removing user-secrets"  
    cd .\VideoOverflow.Server\
    dotnet user-secrets remove VideoOverflow
    cd..
    cd .\VideoOverflow.Infrastructure\
    cd..
   