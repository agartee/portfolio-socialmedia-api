#!/bin/bash

ccrVersion="v1.3.0"

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
binDir="$rootDir/.bin"
toolDest="$binDir/ccr.tar.gz"

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

if [[ -f "$binDir/ccr" ]]; then
  echo -e "${GREEN}Test coverage reporter tool found. Skipping download.${NO_COLOR}"
else
  osName=$(uname)
  case "$osName" in
    Linux)
      distroName=$(lsb_release -si)
      distroVersion=$(lsb_release -sr)
      
      if [[ "$distroName" == "Ubuntu" && "$distroVersion" == "20.04" ]]; then
        toolSrc="https://github.com/agartee/cobertura-console-reporter/releases/download/${ccrVersion}/ccr_ubuntu-20.04_${ccrVersion}_amd64.tar.gz"
      elif [[ "$distroName" == "Ubuntu" && "$distroVersion" == "22.04" ]]; then
        toolSrc="https://github.com/agartee/cobertura-console-reporter/releases/download/${ccrVersion}/ccr_ubuntu-22.04_${ccrVersion}_amd64.tar.gz"
      else
        echo "${RED}Unsupported Linux distribution or version${NO_COLOR}"
        exit 1
      fi
      ;;
    Darwin)
      toolSrc="https://github.com/agartee/cobertura-console-reporter/releases/download/${ccrVersion}/ccr_macos_${ccrVersion}_amd64.tar.gz"
      ;;
    *)
      echo "${RED}Unsupported OS${NO_COLOR}"
      exit 1
      ;;
  esac

  if [[ ! -d "$binDir" ]]; then
    mkdir -p "$binDir"
  fi

  curl -L $toolSrc --output $toolDest
  tar -xzvf $toolDest -C $binDir
  rm $toolDest

  echo -e "${GREEN}Test coverage console tool downloaded to $binDir.${NO_COLOR}"
fi
