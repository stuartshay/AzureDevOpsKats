#!/usr/bin/env bash

set -e

FILES="variables.tf"

ROOT_DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

for F in $FILES; do
	ln -sf $1$F $F
done
