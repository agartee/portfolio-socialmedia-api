#!/bin/bash

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
scriptName=$(echo "${0##*/}" | sed 's/\(.*\)\..*/\1/')
config=$(cat "$rootDir/scripts/.project-settings.json")
imageName=$(echo "$config" | jq -r '.docker.imageName')
containerName=$(echo "$config" | jq -r '.docker.containerName')
tagName=$(echo "$config" | jq -r '.docker.tagName')
userSecretsId=$(echo "$config" | jq -r '.userSecretsId')
databaseConnectionStringName=$(echo "$config" | jq -r --arg scriptName "$scriptName" '.scripts[$scriptName].databaseConnectionStringName')
webAppName=$(echo "$config" | jq -r '.webApp.name')
configuration="Debug"

case "$(uname -s)" in
	Linux)
		RED="\e[31m"
		GREEN="\e[32m"
		NO_COLOR="\e[0m"
		;;
	Darwin)
		RED="\033[31m"
		GREEN="\033[32m"
		NO_COLOR="\033[m"
		;;
esac

while (( "$#" )); do
  case "$1" in
    --configuration|-c)
      if [ -n "$2" ] && [ ${2:0:1} != "-" ]; then
        configuration=$2
        shift 2
      else
        echo "Error: Argument for $1 is missing. Provide configuration name (e.g., Release, Debug)." >&2
        exit 1
      fi
      ;;
    -*|--*=)
      echo "${RED}Error: Unsupported flag $1${NO_COLOR}" >&2
      exit 1
      ;;
    *) # preserve positional arguments
      PARAMS="$PARAMS $1"
      shift
      ;;
  esac
done

docker container rm "$containerName" --force &>/dev/null

# Read SSL_PFX_PATH and SSL_PFX_PASSWORD from .env file
while read -r line || [[ -n "$line" ]]; do
    if [[ $line == SSL_PFX_PATH=* ]]; then
        pfxPath="${line#*=}"
    elif [[ $line == SSL_PFX_PASSWORD=* ]]; then
        pfxPassword="${line#*=}"
    fi
done < "$rootDir/.env"

pfxDir=$(dirname "$pfxPath")
pfxFile=$(basename "$pfxPath")
secretsDir="$HOME/.microsoft/usersecrets/$userSecretsId"

docker container run \
    --name $containerName \
    --publish 5000:80 --publish 5001:443 \
    --env "ASPNETCORE_ENVIRONMENT=Development" \
    --env "ASPNETCORE_URLS=https://+:443;http://+:80" \
    --env "ASPNETCORE_Kestrel__Certificates__Default__Path=/https/$pfxFile" \
    --env "ASPNETCORE_Kestrel__Certificates__Default__Password=$pfxPassword" \
    --volume "$secretsDir:/root/.microsoft/usersecrets/$userSecretsId:ro" \
    --volume "$pfxDir:/https:ro" \
    --entrypoint dotnet \
    --detach \
    "$imageName:$tagName" \
    /app/$webAppName.dll --d $databaseConnectionStringName

if [ $? -eq 0 ]; then
  "$rootDir/scripts/support/wait-for-healthy-container.sh" $containerName
fi
