#!/bin/bash

GREEN="\033[0;32m"
NC="\033[0m" # No Color

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
binDir="$rootDir/.bin"

if [[ -f "$binDir/ccr" ]]; then
  echo -e "${GREEN}Test coverage reporter tool found. Skipping download.${NC}"
else
  ccrVersion="v1.3.0"
  
  osName=$(uname)
  case "$osName" in
    Linux)
      # Detect the specific Linux distribution
      distroName=$(lsb_release -si)
      distroVersion=$(lsb_release -sr)
      
      if [[ "$distroName" == "Ubuntu" && "$distroVersion" == "20.04" ]]; then
        toolSrc="https://github.com/agartee/cobertura-console-reporter/releases/download/${ccrVersion}/ccr_ubuntu-20.04_${ccrVersion}_amd64.tar.gz"
      elif [[ "$distroName" == "Ubuntu" && "$distroVersion" == "22.04" ]]; then
        toolSrc="https://github.com/agartee/cobertura-console-reporter/releases/download/${ccrVersion}/ccr_ubuntu-22.04_${ccrVersion}_amd64.tar.gz"
      else
        echo "Unsupported Linux distribution or version"
        exit 1
      fi
      ;;
    Darwin)
      toolSrc="https://github.com/agartee/cobertura-console-reporter/releases/download/${ccrVersion}/ccr_macos_${ccrVersion}_amd64.tar.gz"
      ;;
    *)
      echo "Unsupported OS"
      exit 1
      ;;
  esac

  toolDest="$binDir/ccr.tar.gz"

  if [[ ! -d "$binDir" ]]; then
    mkdir -p "$binDir"
  fi

  curl -L $toolSrc --output $toolDest
  tar -xzvf $toolDest -C $binDir
  rm $toolDest

  echo -e "${GREEN}Test coverage console tool downloaded to $binDir.${NC}"
fi
