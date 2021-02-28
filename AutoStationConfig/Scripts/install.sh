#!/usr/bin/bash

set -euo pipefail

cd "$(dirname "$0")"

cd ..

RELEASE_FILE="$(pwd)/Release/AutoStationConfig.zip"
MOD_DIR="/c/Users/Pasukaru/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/DSP/BepInEx/plugins/Pasukaru-AutoStationConfig"

rm -rf "$MOD_DIR"
mkdir -p "$MOD_DIR"

"/c/Program Files/7-Zip/7z.exe" e "$RELEASE_FILE" -o"$MOD_DIR" -r
