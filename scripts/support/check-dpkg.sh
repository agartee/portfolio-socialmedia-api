#!/usr/bin/env bash

RED="\033[0;31m"
GREEN="\033[0;32m"
NC="\033[0m" # No Color

if ! type -t dpkg &> /dev/null
    then echo -e "${RED}dpkg installation not found.${NC}"
    exit 1
fi

echo -e "${GREEN}dpkg installation found.${NC}" 
