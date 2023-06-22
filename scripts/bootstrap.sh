#!/usr/bin/env bash

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
appEnv="local"

case "$(uname -s)" in
	Linux)
		RED='\e[31m'
		GREEN='\e[32m'
		NC='\e[0m'
		;;
	Darwin)
		RED='\033[31m'
		GREEN='\033[32m'
		NC='\033[m'
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
    *)
      echo "Unknown parameter passed: $1"
      exit 1
  esac
done

bash $rootDir/scripts/support/check-dotnet.sh
bash $rootDir/scripts/support/check-ccr.sh

if [ "$appEnv" = "local" ]; then
  bash $rootDir/scripts/support/check-docker.sh
  bash $rootDir/scripts/support/check-ssl.sh
fi
