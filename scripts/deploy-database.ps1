[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g., Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Debug",

  [Parameter(Mandatory = $false, HelpMessage = "Name of the database connection string to apply migrations (from the .NET project's user secrets)")]
  [Alias("n")]
  [string]$connectionStringName = "database",

  [Parameter(Mandatory = $false)]
  [Alias("h")]
  [switch]$help
)

if ($help) {
  Write-Output @"

Deploys the application database from a SQL Server project.

Usage: deploy-database.ps1 [-configuration <value>]

Options:
-configuration|-c         Specifies the configuration name for the build. 
                          Common values are "Release" or "Debug". If not 
                          specified, the default is "Debug".
-connectionStringName|-n  Specifies the connection string name from the
                          .NET project's user secrets. If not specified, 
                          the default is "database".
"@ 
  exit
}

$rootDir = (get-item $PSScriptRoot).Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$webAppProjectFile = Join-Path -Path "$rootDir" -ChildPath $config.webApp.projectFile
$databaseProjectFile = Join-Path -Path "$rootDir" -ChildPath $config.database.projectFile

if (-not (Test-Path -Path $webAppProjectFile -PathType Leaf)) {
    Write-Error "Web application project file not found at path: $webAppProjectFile"
    exit 1
  }

if (-not (Test-Path -Path $databaseProjectFile -PathType Leaf)) {
  Write-Error "Database project file not found at path: $databaseProjectFile"
  exit 1
}

function GetConnectionStringFromUserSecrets() {
  $secretsList = dotnet user-secrets list --project $webAppProjectFile
  if (-not $secretsList) {
      return $null;
  }
  
  $secrets = @{}
  $secretsList -split "`n" | ForEach-Object {
      $parts = $_ -split '=', 2
      if ($parts.Count -eq 2) {
          $key = $parts[0].Trim()
          $value = $parts[1].Trim()
          $secrets[$key] = $value
      }
  }

  $result = $secrets["connectionStrings:$($connectionStringName)"];
  
  return $result;
}

function GetConnectionStringFromEnvironment() {
  $envVars = [System.Environment]::GetEnvironmentVariables()
  $envDict = @{}
  foreach ($key in $envVars.Keys) {
      $upperKey = $key.ToUpper()
      $envDict[$upperKey] = $envVars[$key]
  }

  $result = $envDict["CONNECTIONSTRINGS:$($connectionStringName.ToUpper())"]

  if(-not $result) {
    $result = $envDict["CONNECTIONSTRINGS__$($connectionStringName.ToUpper())"];
  }

  return $result;
}

$connectionString = GetConnectionStringFromUserSecrets;
if(-not $connectionString) {
  $connectionString = GetConnectionStringFromEnvironment;
}
if (-not $connectionString) {
  Write-Error "Database connection string not found in user secrets or environment variables."
  exit 1
}

$msBuild = &"${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
    -latest -prerelease -products * -requires Microsoft.Component.MSBuild `
    -find MSBuild\**\Bin\MSBuild.exe

& $msBuild $databaseProjectFile /property:Configuration=$configuration

$projectDir = Split-Path -Parent $databaseProjectFile;
$projectName = [System.IO.Path]::GetFileNameWithoutExtension($databaseProjectFile)

& sqlpackage /a:publish `
  /TargetConnectionString:$connectionString `
  /SourceFile:"$($projectDir)\bin\$($configuration)\$($projectName).dacpac"
