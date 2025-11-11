#!/usr/bin/env bash

# Stop on errors
set -e

# Check if an argument was provided
if [ -z "$1" ]; then
  echo "Usage: $0 {run|build}"
  exit 1
fi

case "$1" in
  run)
    echo "Building ..."
    dotnet build .
    ./bin/Debug/net8.0/IceSaw2
    ;;
  build)
    echo "Building ..."
    dotnet build .
    ;;
  *)
    echo "Unknown command: $1"
    echo "Usage: $0 {run|build}"
    exit 1
    ;;
esac
    exit 1
    ;;
esac