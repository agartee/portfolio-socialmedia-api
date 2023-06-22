#!/usr/bin/env bash

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
appEnv="local"
configuration="Debug"

case "$(uname -s)" in
	Linux)
		RED='\e[31m'
		GREEN="\e[32m"
		NO_COLOR="\e[0m"
		;;
	Darwin)
		RED='\033[31m'
		GREEN="\033[32m"
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
        echo "Error: Argument for $1 is missing" >&2
        exit 1
      fi
      ;;
    *)
      echo "Unknown parameter passed: $1"
      exit 1
  esac
done

if [ "$appEnv" = "local" ]; then
  bash $rootDir/scripts/support/start-server-local.sh --configuration "$configuration"
fi

if [ "$appEnv" = "docker" ]; then
  bash $rootDir/scripts/support/start-server-docker.sh --configuration "$configuration"
fi
