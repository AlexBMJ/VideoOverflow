dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef

cd VideoOverflow.Infrastructure || exit
dotnet ef migrations add VideoOverflowMigration

cd ../
sudo docker-compose up -d