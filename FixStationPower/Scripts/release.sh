﻿#!/usr/bin/bash

set -euo pipefail

cd "$(dirname "$0")"

cd ..

../get-libs.sh
../build.sh

FILE_NAME="FixStationPower.dll"
BUILD_DIR="$(pwd)/bin/Release"
RELEASE_DIR="$(pwd)/Release"
RELEASE_DLL="$RELEASE_DIR/$FILE_NAME"

rm -f "$RELEASE_DLL"
cp "$BUILD_DIR/$FILE_NAME" "$RELEASE_DIR/"

cd "$RELEASE_DIR"

RELEASE_FILE="FixStationPower.zip"
rm -f "$RELEASE_FILE"

"/c/Program Files/7-Zip/7z.exe" a -tzip "$RELEASE_FILE" '*'

rm -f "$RELEASE_DLL"
