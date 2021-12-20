#!/bin/bash
sudo docker run --rm --name test_db -p 5002:5432 -v testdb_data -e POSTGRES_PASSWORD=test_db -d postgres
connectionString="Server=localhost;Database=test_database;Port=5002;UserId=postgres;Password=testdb"
echo "$connectionString"