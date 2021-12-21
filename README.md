# VideoOverflow
BDSA 2021

[![.NET](https://github.com/AlexBMJ/VideoOverflow/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/AlexBMJ/VideoOverflow/actions/workflows/dotnet.yml)

VideoOverflow is an web application which aggregates educational resources. It enables a user to search for tutorials by category and to be presented with learning materials in the form of videos, articles and books.

## Prerequisites
- [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Docker](https://www.docker.com/)


## Installation

This program relies on a [PostgreSQL](https://www.postgresql.org/) database to function, in a production setting, a properly managed database system should be used.
For testing purposes a PowerShell and BASH script have been written using [Docker](https://www.docker.com/).
The scripts provided will automatically start a docker running postgres, create connection strings, and populate the database. 

Linux/MacOS:
```bash
./run.sh
```
Windows:
```powershell
RunApp.ps1
```

**Manual Build and Run**

This project uses .NET 6 and can be built from the root of the repository and run as follows:
```bash
dotnet build
dotnet run --project VideoOverflow.Server
```

## Usage
When testing the database runs at **localhost:5001** and the blazor web page is served on **https://localhost:7019**

## Testing
Some of the xUnit tests rely on postgres specific features, therefore a testing database must be run seperately. \
`./run-testdb.sh` or `RunTestDB.ps1`

```bash
dotnet test
```

## Contributors
Alexander Bastian Magno Jacobsen (ALJA)
Anton Marius Nielsen (ANTNI)
Asmus Tørsleff (ASMT)
Christian Lyon Lüthcke (CLYT)
Deniz Isik (DENI)
Karl Bilsted Michelsen (KABM)

## License
[GPLv3](https://github.com/AlexBMJ/VideoOverflow/blob/main/LICENSE)
