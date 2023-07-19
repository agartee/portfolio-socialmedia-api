#!/bin/bash

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

if ! type -t jq &> /dev/null
    then echo -e "${RED}jq installation not found.${NO_COLOR}"
    exit 1
fi

echo -e "${GREEN}jq installation found.${NO_COLOR}" 
