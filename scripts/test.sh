#!/usr/bin/env bash

rootDir=$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)
configuration="Debug"
testProjects=$(find "$rootDir/test" -name "*.csproj")
coverageDir="$rootDir/.test-coverage"
binDir="$rootDir/.bin"
status=0

case "$(uname -s)" in
	Linux)
    BLUE="\e[34m"
    GREEN="\e[32m"
		NO_COLOR="\e[0m"
		;;
	Darwin)
    BLUE="\033[34m"
    GREEN="\033[32m"
		NO_COLOR="\033[m"
		;;
esac

# Parse command-line options
while (( "$#" )); do
  case "$1" in
    --configuration|-c)
      if [ -n "$2" ] && [ ${2:0:1} != "-" ]; then
        configuration=$2
        shift 2
      else
        echo "Error: Argument for $1 is missing. Provide configuration name (e.g. Release, Debug)." >&2
        exit 1
      fi
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

# reminder: add a comma at the beginning of subsequent exclusions for a project
declare -A exclusions
exclusions["SocialMedia.Domain"]="SocialMedia.Domain.Models.*"

exclusions["SocialMedia.Persistence.Auth0"]="SocialMedia.Persistence.Auth0.Configuration.*"
exclusions["SocialMedia.Persistence.Auth0"]+=",SocialMedia.Persistence.Auth0.Models.*"

exclusions["SocialMedia.Persistence.SqlServer"]="SocialMedia.Persistence.SqlServer.Migrations.*"
exclusions["SocialMedia.Persistence.SqlServer"]+=",SocialMedia.Persistence.SqlServer.Models.*"

exclusions["SocialMedia.WebAPI"]="Program"
exclusions["SocialMedia.WebAPI"]+=",SocialMedia.WebAPI.Configuration.*"
exclusions["SocialMedia.WebAPI"]+=",SocialMedia.WebAPI.Formatters.*"

rm -rf "$coverageDir"

for testProject in $testProjects
do
  projectName=$(basename "$(dirname "$testProject")")
  projectName=${projectName%%.Tests}

  IFS=","
  excludeArray=(${exclusions[$projectName]})
  exclude=""
  for pattern in "${excludeArray[@]}"
  do
    exclude+=",[$projectName]$pattern"
  done
  exclude=${exclude#","}  # Remove the leading comma

  echo -e "${BLUE}Executing tests for $testProject...${NC}"

  dotnet test "$testProject" --no-build -c "$configuration" \
    --results-directory "$coverageDir" \
    --collect:"XPlat Code Coverage" \
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude="$exclude"

  if [ $? -eq 0 ]
  then
    status=$?
  fi

  coverageFile=$(find "$coverageDir" -name 'coverage.cobertura.xml' -print0 | xargs -0 ls -t | head -n 1)
  
  $binDir/ccr --coverage-file "$coverageFile" --package "$projectName"

  if [ -n "${exclude}" ]; then
    echo -e "${BLUE}Coverage Exclusions:${NC}"
    for exclusion in "${exclusions[$projectName]}"; do
      # Substitutes the "[projectName]" part of the string with an empty string, effectively removing it
      exclusion="${exclusion#"\[$projectName\]"}"
      # Split the exclusion string on commas and print each one on a new line
      IFS=',' read -ra split_exclusions <<< "$exclusion"
      for i in "${split_exclusions[@]}"; do
        echo "  $i"
      done
    done
    echo
  fi

done

exit $status
