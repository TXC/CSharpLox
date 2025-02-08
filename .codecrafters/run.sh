#!/bin/sh
#
# This script is used to run your program on CodeCrafters
#
# This runs after .codecrafters/compile.sh
#
# Learn more: https://codecrafters.io/program-interface

set -e # Exit on failure

ls -l /tmp/

ls -l /tmp/codecrafters-build-csharp/ 

exec /tmp/codecrafters-build-csharp/Interpreter "$@"
