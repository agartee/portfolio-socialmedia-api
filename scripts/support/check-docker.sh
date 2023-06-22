#!/usr/bin/env bash

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

currentVer=$(docker --version 2> /dev/null)
if [ -z "$currentVer" ]
    then echo -e "${RED}Docker installation not found.${NC}"
    exit 1
fi

currentVer="${currentVer/Docker version /""}"

echo -e "${GREEN}Docker installation found: ${currentVer}.${NC}"
