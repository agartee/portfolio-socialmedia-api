#!/bin/bash

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
appEnv="local"
configuration="Debug"

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
    -l|--local)
      appEnv="local"
      shift
      ;;
    -d|--docker)
      appEnv="docker"
      shift
      ;;
    -c|--configuration)
      if [ -n "$2" ] && [ ${2:0:1} != "-" ]; then
        configuration=$2
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

if [ "$appEnv" = "local" ]; then
  bash $rootDir/scripts/support/start-server-local.sh --configuration "$configuration"
fi

if [ "$appEnv" = "docker" ]; then
  bash "$rootDir/scripts/support/build-docker.sh" -c "$configuration"
  bash $rootDir/scripts/support/start-server-docker.sh --configuration "$configuration"
fi
