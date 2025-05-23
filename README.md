# Portfolio Project - Social Media API

A simple social media app intended to demonstrate good development practices. This is the API solution for the [Social Media](https://github.com/agartee/portfolio-socialmedia) SPA app.

## Build Configuration Settings

Configuration settings that are required by scripts are stored in the `.env` file. There is an example file included that can be used as a template (`.env.example`).

| Setting | Description |
| --- | --- |
| `SSL_PFX_PATH` | Path to the SSL certificate PFX file. This is used when the app is launched in a docker container. |
| `SSL_PFX_PASSWORD` | Password for the SSL certificate PFX file. |

## Runtime Configuring Settings

Configuration settings that are required by the application at runtime are generally stored in the user secrets file associated with the web app (`SocialMedia.WebAPI`).

| Setting | Description |
| --- | --- |
| `connectionStrings:database` | Connection string to the database when running the application |
| `connectionStrings:testDatabase` | Connection string to the application database used for integration testing |
| `connectionStrings:dockerDatabase` | Connection string to the database when running the application in a Docker container |
| `authentication:authority` | Auth0 authority for your account |
| `authentication:audience` | API identifier in Auth0 |
| `userManagement:authentication:audience` | API identifier in Auth0 for the Management API |
| `userManagement:authentication:clientId` | Your app's client ID (must be authorized for the Management API in Auth0) |
| `userManagement:authentication:clientSecret` | Your app's client secret (must be authorized for the Management API in Auth0) |
| `cors:allowedOrigins` | Allowed origin addresses when running the app locally |

**Example User Secrets File for `SocialMedia.WebAPI.csproj`**:

```json
{
  "connectionStrings": {
    "database": "Server=localhost;Database=SocialMedia;Trusted_Connection=true;TrustServerCertificate=True;MultipleActiveResultSets=true",
    "dockerDatabase": "Server=host.docker.internal;Database=SocialMedia;User=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true",
    "testDatabase": "Server=localhost;Database=SocialMediaTests;Trusted_Connection=true;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "authentication": {
    "authority": "<ACCOUNT>.auth0.com",
    "audience": "<AUTH0-API-IDENTIFIER>"
  },
  "userManagement": {
    "authentication": {
      "audience": "https://<ACCOUNT>.auth0.com/api/v2/",
      "clientId": "<AUTH0-CLIENT-ID>",
      "clientSecret": "<AUTH0-CLIENT-SECRET>"
    }
  },
  "allowedOrigins": [
    "http://localhost:4000",
    "https://localhost:4001"
  ]
}
```

### Setting User Secrets in Linux

User secrets can be added via the terminal:

```bash
dotnet user-secrets set "connectionStrings:testDatabase" "Server=host.docker.internal;Database=SocialMediaTests;User=sa;Password=@Password!;TrustServerCertificate=True;MultipleActiveResultSets=true;"
```

User secrets are stored in `~/.microsoft/usersecrets/agartee-socialmedia/secrets.json`.

> Note: When running the application or tests from WSL and when the SQL Server database is hosted in a Docker container, you should use `host.docker.internal` as the server in your database connection string.

## Available PowerShell Scripts

These scripts are aimed to normalize script patterns across projects and platforms (based on [Scripts to Rule Them All](https://github.com/github/scripts-to-rule-them-all)).

| Script | Description |
| --- | --- |
| `./scripts/clean.{ps1/sh}` | Sets project back to an initial state. |
| `./scripts/bootstrap.{ps1/sh}` | Checks for app dependencies. Errors/warnings will be displayed if any required software is missing (this script will not install software on your system). |
| `./scripts/build.{ps1/sh}` | Builds the app.<br>Command args:<br>&nbsp;&nbsp;`[-local]`: Builds the app locally.<br>&nbsp;&nbsp;`[-docker]`: Builds a Docker image.<br>&nbsp;&nbsp;`[-configuration]`: The configuration to use. Defaults to "Debug". |
| `./scripts/test.{ps1/sh}` | Executes tests in all projects under `./test`.<br>Command args:<br>&nbsp;&nbsp;`[-configuration]`: The configuration to use. Defaults to "Debug".<br>&nbsp;&nbsp;`[-nobuild]`: Skip build on projects before running tests. |
| `./scripts/server.{ps1/sh}` | Builds and runs the app in the "Release" configuration.<br>Command args:<br>&nbsp;&nbsp;`[-local]`: Run via `dotnet run`.<br>&nbsp;&nbsp;`[-docker]`: Builds a Docker image and starts a new container.<br>&nbsp;&nbsp;`[-configuration]`: The configuration to use. Defaults to "Release". |
| `./scripts/publish.{ps1/sh}` | Publishes the app to `./.publish`<br>Command args:<br>&nbsp;&nbsp;`[-configuration]`: The configuration to use. Defaults to "Release". |


## SQL Server Configuration Recommendations for Docker (Local Development)

Steps to configure your local SQL Server environment for username/password authentication are as follows:

- Enable Windows Authentication mode in [SQL Server Management Studio](https://learn.microsoft.com/en-us/sql/ssms/): *Server Properties -> Security -> SQL Server and Windows Authentication mode -> Enable*) and restart the server.

- Enable TCP/IP Protocol in [Sql Server Configuration Manager](https://learn.microsoft.com/en-us/sql/relational-databases/sql-server-configuration-manager): *SQL Server Network Configuration -> Protocols for MSSQLSERVER -> TCP/IP -> Enable*) and restart the server.

Example T-SQL for creating/recreating a local user named "dev" with `sysadmin` rights (needed to create databases and execute migrations):

```
USE [master]
GO
DROP LOGIN [dev]
GO
CREATE LOGIN [dev] WITH PASSWORD=N'<PASSWORD>', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [dev]
GO
```

Now you can update the `database` connection string in the ASP.NET app:

```json
"connectionStrings": {
    "database": "Server=host.docker.internal;Database=Commissioner;User=dev;Password=<PASSWORD>;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
```

## Generating additional EF Migrations

From the root folder, run the following command, replacing the `$migrationName` with the name of your new migration:

```powershell
dotnet ef migrations add $migrationName --startup-project ./src/SocialMedia.WebAPI/ --project ./src/SocialMedia.Persistence.SqlServer/ --context SocialMediaDbContext
```

This will create the new migration class in the `SocialMedia.Persistence.SqlServer` project's `Migrations` folder.
