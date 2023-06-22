#!/bin/bash

# Default configuration
configuration="Debug"

# Parse command-line options
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
    -*|--*=) # unsupported flags
      echo "Error: Unsupported flag $1" >&2
      exit 1
      ;;
    *) # preserve positional arguments
      PARAMS="$PARAMS $1"
      shift
      ;;
  esac
done

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
image_name="socialmedia-api"
tag_name="dev"

docker image build \
  --tag "${image_name}:${tag_name}" \
  --build-arg CONFIG=${configuration} \
  "$rootDir"
