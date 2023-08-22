#!/bin/bash

if [ -z "$1" ]; then
    echo "Container name or ID is mandatory."
    exit 1
fi

containerNameOrId="$1"
timeoutSeconds=10
start_time=$(date +%s)

while :; do
    health_status=$(docker inspect --format='{{.State.Health.Status}}' "$containerNameOrId" 2>&1)

    if [ $? -ne 0 ]; then
        echo "Error: $health_status"
        exit 1
    fi

    if [ "$health_status" == "healthy" ]; then
        echo "Container is healthy."
        break
    fi

    current_time=$(date +%s)
    elapsed_time=$(( current_time - start_time ))

    if [ $elapsed_time -gt $timeoutSeconds ]; then
        echo "Timeout exceeded waiting for container to become healthy."
        exit 1
    fi

    sleep 2
done
