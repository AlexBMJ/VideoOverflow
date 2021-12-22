# VideoOverflow
**BDSA 2021**

[![.NET](https://github.com/AlexBMJ/VideoOverflow/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/AlexBMJ/VideoOverflow/actions/workflows/dotnet.yml)
[![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/AlexBMJ/0429939e94ea4bf60727cef5e69faa60/raw/code-coverage.json)](https://app.codecov.io/gh/AlexBMJ/VideoOverflow/)
[![C#](https://img.shields.io/badge/language-C%23-darkgreen.svg)](https://dotnet.microsoft.com/en-us/languages/csharp) 
[![.Net](https://img.shields.io/badge/framework-.NET-purple.svg)](https://dotnet.microsoft.com/en-us/)
[![license](https://img.shields.io/github/license/AlexBMJ/VideoOverflow.svg)](https://github.com/AlexBMJ/VideoOverflow/blob/main/LICENSE)

VideoOverflow is a web application which aggregates educational resources. It enables a user to search for tutorials by category, title and tag. They will then be presented with learning materials in the form of videos, articles and books.

## Prerequisites
_Before attempting to run, make sure you have the following software installed_
- [Docker](https://www.docker.com/)

## Installation
This program relies on [PostgreSQL](https://www.postgresql.org/) to function, scripts have been provided to containerize the database but a properly secured and managed database system should be used in a production setting.

To run the production demo use [docker compose](https://docs.docker.com/compose/) (docker doesn't provide the **docker-compose** binary on some systems ie. [Arch Linux](https://archlinux.org/packages/community/x86_64/docker-compose/))

```
docker-compose up -d
```

That's it! You should now be able to visit **http://localhost**. 

---

For development purposes a PowerShell and BASH [script](https://github.com/AlexBMJ/VideoOverflow/blob/main/run-development.sh) were written to utilize docker.
The scripts provided will automatically start a docker running postgres, create connection strings, and populate the database with demo data. 
_Unix:_ `./run-development.sh`
_Windows:_ `RunDevelopment.ps1`

## Usage
Using the recommended docker compose file, everything should be containerized and mapped out to port 80 for http and 443 for https respectively

When running with the provided scripts, the database binds to **localhost:5001** and the blazor web page is served on **https://localhost:7019**

## Testing 
### \*IMPORTANT\* PLEASE READ BEFORE TESTING
Some of the xUnit tests rely on postgres specific features, therefore a temporary postgres database must be run separately. \
`./run-testdb.sh` or `RunTestDB.ps1`

```
dotnet test
```

## Manually Build & Run
If you wish to build and run the program on bare metal or debug outside a container you will need: 
- [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [PostgreSQL](https://www.postgresql.org/)

Using dotnet tools, set database connection strings with the name "VideoOverflow" in the `Server` and `Infrastructure` projects.
In a production environment, key-stores are recommended.
Once the database has been setup the app can be built from the root of the repository and run as follows:
```
dotnet build
dotnet run --project VideoOverflow.Server
```


## Contributors
- Alexander Bastian Magno Jacobsen (ALJA)
- Anton Marius Nielsen (ANTNI)
- Asmus Tørsleff (ASMT)
- Christian Lyon Lüthcke (CLYT)
- Deniz Isik (DENI)
- Karl Bilsted Michelsen (KABM)

## License
[GPLv3](https://github.com/AlexBMJ/VideoOverflow/blob/main/LICENSE)
