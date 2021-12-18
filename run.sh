#!/bin/bash
password="Test1234"
database="VideoOverflow"
sudo docker stop videooverflow_db
sudo docker rm videooverflow_db
sudo docker run --name videooverflow_db -p 5001:5432 -v postgres_data -e POSTGRES_PASSWORD=$password -e POSTGRES_DB=$database -d postgres
connectionString="Server=localhost;Database=$database;Port=5001;UserId=postgres;Password=$password"

echo "Setting up connectionString"
cd VideoOverflow.Server || exit
dotnet user-secrets set "ConnectionStrings:VideoOverflow" "$connectionString"
cd ..

cd VideoOverflow.Infrastructure || exit
dotnet user-secrets set "ConnectionStrings:VideoOverflow" "$connectionString"
rm -rf Migrations
dotnet ef migrations add VideoOverflowMigration
dotnet ef database update

echo "Starting Application...."
cd ..
cd VideoOverflow.Server || exit
dotnet watch