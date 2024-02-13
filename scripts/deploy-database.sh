#!/bin/bash

# Default values
configuration="Debug"
connectionStringName="database"
help=0

show_help() {
  cat << EOF

Deploys the application database from a SQL Server project.

Usage: deploy-database.ps1 [-configuration <value>]

Options:
-configuration|-c         Specifies the configuration name for the build. 
                          Common values are "Release" or "Debug". If not 
                          specified, the default is "Debug".
-connectionStringName|-n  Specifies the connection string name from the
                          .NET project's user secrets. If not specified, 
                          the default is "database".
EOF
}

while (( "$#" )); do
  case "$1" in
    -h|-help)
      show_help
      exit 0
      ;;
    -c|--configuration)
      configuration=$2
      shift 2
      ;;
    -n|--connectionStringName)
      connectionStringName=$2
      shift 2
      ;;
    *)
      echo "Error: Unsupported flag $1" >&2
      exit 1
      ;;
  esac
done

rootDir=$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)
config=$(cat "$rootDir/scripts/.project-settings.json")
webAppProjectFile="$rootDir/$(echo $config | jq -r '.webApp.projectFile')"
databaseProjectFile="$rootDir/$(echo $config | jq -r '.database.projectFile')"

if [ ! -f "$webAppProjectFile" ]; then
  echo "Web application project file not found at path: $webAppProjectFile" >&2
  exit 1
fi

if [ ! -f "$databaseProjectFile" ]; then
  echo "Database project file not found at path: $databaseProjectFile" >&2
  exit 1
fi

getConnectionStringFromUserSecrets() {
  secretsList=$(dotnet user-secrets list --project "$webAppProjectFile")
  if [ -z "$secretsList" ]; then
    return
  fi

  while IFS= read -r line; do
    key=$(echo $line | cut -d '=' -f 1 | xargs)
    value=$(echo $line | cut -d '=' -f 2 | xargs)
    if [[ "$key" == "connectionStrings:$connectionStringName" ]]; then
      echo $value
      return
    fi
  done <<< "$secretsList"
}

getConnectionStringFromEnvironment() {
  envVarName="CONNECTIONSTRINGS_${connectionStringName^^}"
  envVarName=${envVarName//-/_} # Replace hyphens with underscores
  echo ${!envVarName}
}

connectionString=$(getConnectionStringFromUserSecrets)
if [ -z "$connectionString" ]; then
  connectionString=$(getConnectionStringFromEnvironment)
fi
if [ -z "$connectionString" ]; then
  echo "Database connection string not found in user secrets or environment variables." >&2
  exit 1
fi

msBuild=$("${env:ProgramFiles(x86)}"/Microsoft Visual Studio/Installer/vswhere.exe -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild/**/Bin/MSBuild.exe)

"$msBuild" "$databaseProjectFile" /property:Configuration="$configuration"

projectDir=$(dirname "$databaseProjectFile")
projectName=$(basename "$databaseProjectFile" .sqlproj)

sqlpackage /a:publish \
  /TargetConnectionString:"$connectionString" \
  /SourceFile:"$projectDir/bin/$configuration/$projectName.dacpac"
