#!/bin/bash

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
config=$(cat "$rootDir/scripts/.project-settings.json")
solutionFile="$rootDir/$(echo "$config" | jq -r '.solutionFile')"
configuration="Debug"
version="1.0.0"

case "$(uname -s)" in
	Linux)
		RED="\e[31m"
		NO_COLOR="\e[0m"
		;;
	Darwin)
		RED="\033[31m"
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
    -v|--version)
      if [ -n "$2" ] && [ ${2:0:1} != "-" ]; then
        version=$2
        shift 2
      else
        echo "${RED}Error: Argument for $1 is missing${NO_COLOR}" >&2
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

dotnet clean "$solutionFile" --configuration "$configuration"

dotnet build "$solutionFile" \
  --configuration "$configuration" \
  /p:Version="$version" \
