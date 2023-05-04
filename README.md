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
| `connectionStrings:database` | Connection string to the app database. When using a Docker container to host the app, username and password must be used in place of a trusted connection. |
| `authentication:authority` | Auth0 authority for your account |
| `authentication:audience` | API identifier in Auth0 |
| `allowedOrigins` | Allowed origin addresses when running the app locally |

**Example User Secrets File for `SocialMedia.WebAPI.csproj`**:

```json
{
  "connectionStrings": {
    "database": "Server=localhost;Database=SocialMedia;Trusted_Connection=true;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "authentication": {
    "authority": "[ACCOUNT].auth0.com",
    "audience": "socialmedia"
  },
  "allowedOrigins": [
    "http://localhost:5000",
    "https://localhost:5001"
  ]
}
```

## Available PowerShell Scripts

These scripts are aimed to normalize script patterns across projects and platforms (based on [Scripts to Rule Them All](https://github.com/github/scripts-to-rule-them-all)).

| Script | Description |
| --- | --- |
| `./scripts/setup.ps1` | Sets project back to an initial state and then runs `bootstrap.ps1`. |
| `./scripts/bootstrap.ps1` | Checks for app dependencies. Errors/warnings will be displayed if any required software is missing (this script will not install software on your system). |
| `./scripts/build.ps1` | Builds the app.<br>Command args:<br>&nbsp;&nbsp;`[-local]`: Builds the app locally.<br>&nbsp;&nbsp;`[-docker]`: Builds a Docker image.<br>&nbsp;&nbsp;`[-configuration]`: The configuration to use. Defaults to "Debug". |
| `./scripts/test.ps1` | Executes tests in all projects under `./test`.<br>Command args:<br>&nbsp;&nbsp;`[-configuration]`: The configuration to use. Defaults to "Debug". |
| `./scripts/server.ps1` | Builds and runs the app in the "Release" configuration.<br>Command args:<br>&nbsp;&nbsp;`[-local]`: Run via `dotnet run`.<br>&nbsp;&nbsp;`[-docker]`: Builds a Docker image and starts a new container.<br>&nbsp;&nbsp;`[-configuration]`: The configuration to use. Defaults to "Release". |
| `./scripts/publish.ps1` | Publishes the app to `./.publish`<br>Command args:<br>&nbsp;&nbsp;`[-local]`: Run via `dotnet run`.<br>&nbsp;&nbsp;`[-docker]`: Builds a Docker image and starts a new container. Ensure you have generated an SSL Certificate PFX file before starting the app this way.<br>&nbsp;&nbsp;`[-configuration]`: The configuration to use. Defaults to "Release". |


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
