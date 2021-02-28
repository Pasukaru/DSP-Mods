#!/usr/bin/bash

set -euo pipefail

cd "$(dirname "$0")"

RELEASE_DIR="$(pwd)/AutoStationConfig/bin/Release"
MODS_DIR="/d/Steam/steamapps/common/Dyson Sphere Program/BepInEx/plugins"
FILE_NAME="Pasukaru.AutoStationConfig.dll"

rm -f "$MODS_DIR/$FILE_NAME"
cp "$RELEASE_DIR/Pasukaru.AutoStationConfig.dll" "$MODS_DIR/"

echo "MODS:"
ls -l "$MODS_DIR"