#!/usr/bin/env bash

rootDir=$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)
config=$(jq '.' "$rootDir/scripts/scripts.json")
testProjects=$(find "$rootDir/test" -name "*.csproj")
coverageDir="$rootDir/.test-coverage"
binDir="$rootDir/.bin"
configuration="Debug"
status=0

case "$(uname -s)" in
	Linux)
    RED="\e[31m"
    BLUE="\e[34m"
		NO_COLOR="\e[0m"
		;;
	Darwin)
    RED="\033[31m"
    BLUE="\033[34m"
		NO_COLOR="\033[m"
		;;
esac

declare -A exclusions

exclusionLength=$(echo "$config" | jq '.test.exclusions | length')

for (( i=0; i<$exclusionLength; i++ ))
do
    project=$(echo "$config" | jq -r ".test.exclusions[$i].project")
    excludeLength=$(echo "$config" | jq ".test.exclusions[$i].exclude | length")
    exclusions["$project"]=""
    for (( j=0; j<$excludeLength; j++ ))
    do
        exclude=$(echo "$config" | jq -r ".test.exclusions[$i].exclude[$j]")
        if [ -n "${exclusions[$project]}" ]; then
            exclusions["$project"]+=","
        fi
        exclusions["$project"]+="$exclude"
    done
done

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
      echo "${RED}Error: Unsupported flag $1${NO_COLOR}" >&2
      exit 1
      ;;
    *) # preserve positional arguments
      PARAMS="$PARAMS $1"
      shift
      ;;
  esac
done

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
  exclude=${exclude#","}

  echo -e "${BLUE}Executing tests for $testProject...${NO_COLOR}"

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
    echo -e "${BLUE}Coverage Exclusions:${NO_COLOR}"
    for exclusion in "${exclusions[$projectName]}"; do
      exclusion="${exclusion#"\[$projectName\]"}"
      IFS=',' read -ra split_exclusions <<< "$exclusion"
      for i in "${split_exclusions[@]}"; do
        echo "  $i"
      done
    done
    echo
  fi

done

exit $status
