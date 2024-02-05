#!/bin/bash

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
config=$(cat "$rootDir/scripts/.project-settings.json")
imageName=$(echo "$config" | jq -r '.docker.imageName')
tagName=$(echo "$config" | jq -r '.docker.tagName')
configuration="Debug"
version="1.0.0"

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

docker image build \
  --tag "${imageName}:${tagName}" \
  --build-arg CONFIG="$configuration" \
  --build-arg VERSION="$version" \
  "$rootDir"
