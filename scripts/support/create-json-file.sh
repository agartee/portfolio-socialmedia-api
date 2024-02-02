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

while (( "$#" )); do
  case "$1" in

    --path|-p)
      if [ -n "$2" ] && [ ${2:0:1} != "-" ]; then
        path=$2
        shift 2
      else
        echo "${RED}Error: Argument for $1 is missing${NO_COLOR}" >&2
        exit 1
      fi
      ;;

    --key|-k)
      if [ -n "$2" ] && [ ${2:0:1} != "-" ]; then
        key=$2
        shift 2
      else
        echo "${RED}Error: Argument for $1 is missing${NO_COLOR}" >&2
        exit 1
      fi
      ;;

    --value|-v)
      if [ -n "$2" ] && [ ${2:0:1} != "-" ]; then
        value=$2
        shift 2
      else
        echo "${RED}Error: Argument for $1 is missing${NO_COLOR}" >&2
        exit 1
      fi
      ;;

    *) # preserve positional arguments
      PARAMS="$PARAMS $1"
      shift
      ;;
  esac
done

if [ -f "$path" ]; then
    jq --arg key "$key" --arg value "$value" '.[$key] = $value' "$path" > tmp.$$.json && mv tmp.$$.json "$path"
else
    jq -n --arg key "$key" --arg value "$value" '{$key: $value}' > "$path"
fi
