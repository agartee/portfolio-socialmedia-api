#!/bin/bash

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
config=$(cat "$rootDir/scripts/scripts.json")
projectFile="$rootDir/$(echo "$config" | jq -r '.server.local.projectFile')"
configuration="Debug"

while (( "$#" )); do
  case "$1" in
    --configuration|-c)
      if [ -n "$2" ] && [ ${2:0:1} != "-" ]; then
        configuration=$2
        shift 2
      else
        echo "Error: Argument for $1 is missing. Provide configuration name (e.g. Release, Debug)." >&2
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

dotnet run --project $projectFile --launch-profile https
