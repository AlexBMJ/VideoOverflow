Write-Host "Removing user-secrets"  
    cd .\VideoOverflow.Server\
    dotnet user-secrets remove ConnectionStrings:VideoOverflow
    cd..
    cd .\VideoOverflow.Infrastructure\
    cd..
   
