#!/bin/bash

show_help() {
  cat << EOF

Initiates a build of the project.

Usage: build.ps1 [[-local] | [docker]] [-configuration <value>] [-version <value>]

Options:
-local|-l           Builds the application locally. This is the default script 
                    behavior.
-docker|-d          Builds the application in a Docker image.
-configuration|-c   Specifies the configuration name for the build. Common 
                    values are "Release" or "Debug". If not specified, the 
                    default is "Debug".
-version|-v         Specifies the version for the build outputs. If not 
                    specified, the default is "1.0.0".
EOF
}

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
appEnv="local"
configuration="Debug"
version="1.0.0"

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
    -h|-help)
      show_help
      exit 0
      ;;
    -l|-local)
      appEnv="local"
      shift
      ;;
    -d|-docker)
      appEnv="docker"
      shift
      ;;
    -c|-configuration)
      if [ -n "$2" ] && [ ${2:0:1} != "-" ]; then
        configuration=$2
        shift 2
      else
        echo "${RED}Error: Argument for $1 is missing${NO_COLOR}" >&2
        exit 1
      fi
      ;;
    -v|-version)
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

if [ "$appEnv" = "local" ]; then
  bash $rootDir/scripts/support/build-local.sh \
    --configuration "$configuration" \
    --version "$version"
fi

if [ "$appEnv" = "docker" ]; then
  bash $rootDir/scripts/support/build-docker.sh \
    --configuration "$configuration" \
    --version "$version"
fi
