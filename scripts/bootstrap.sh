#!/usr/bin/env bash

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
appEnv="local"

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
    -l|--local)
      appEnv="local"
      shift
      ;;
    -c|--ci)
      appEnv="ci"
      shift
      ;;
    -*|--*=)
      echo "Error: Unsupported flag $1" >&2
      exit 1
      ;;
    *) # preserve positional arguments
      PARAMS="$PARAMS $1"
      shift
      ;;
  esac
done

bash $rootDir/scripts/support/check-dotnet.sh
bash $rootDir/scripts/support/check-ccr.sh

if [ "$appEnv" = "local" ]; then
  bash $rootDir/scripts/support/check-docker.sh
  bash $rootDir/scripts/support/check-ssl.sh
fi
