#!/bin/bash

show_help() {
  cat << EOF

Cleans up temporary directories and files from the project.

Usage: clean.ps1
EOF
}

rootDir=$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)

case "$(uname -s)" in
	Linux)
    BLUE="\e[34m"
		NO_COLOR="\e[0m"
		;;
	Darwin)
    BLUE="\033[34m"
		NO_COLOR="\033[m"
		;;
esac

while (( "$#" )); do
  case "$1" in
    -h|-help)
      show_help
      exit 0
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

binDir="$rootDir/.bin"
if [ -d "$binDir" ]; then
  echo -e "${BLUE}Removing $binDir...${NO_COLOR}"
  rm -rf "$binDir"
fi

publishDir="$rootDir/.publish"
if [ -d "$publishDir" ]; then
  echo -e "${BLUE}Removing $publishDir...${NO_COLOR}"
  rm -rf "$publishDir"
fi

testCoverageDir="$rootDir/.test-coverage"
if [ -d "$testCoverageDir" ]; then
  echo -e "${BLUE}Removing $testCoverageDir...${NO_COLOR}"
  rm -rf "$testCoverageDir"
fi

sslDir="$rootDir/.ssl"
if [ -d "$sslDir" ]; then
  echo -e "${BLUE}Removing $sslDir...${NO_COLOR}"
  rm -rf "$sslDir"
fi
