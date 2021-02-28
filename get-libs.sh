#!/usr/bin/bash

set -euo pipefail

cd "$(dirname "$0")"

LIBS_DIR="$(pwd)/BuildDeps"
GAME_DIR="/d/Steam/steamapps/common/Dyson Sphere Program/DSPGAME_Data/Managed/"
MODMAN_DIR="/c/Users/Pasukaru/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/DSP/BepInEx/core"

rm -rf "$LIBS_DIR"
mkdir -p "$LIBS_DIR"

cp "$MODMAN_DIR/0Harmony.dll" "$LIBS_DIR/"
cp "$MODMAN_DIR/BepInEx.dll" "$LIBS_DIR/"
cp "$MODMAN_DIR/BepInEx.Harmony.dll" "$LIBS_DIR/"
cp "$GAME_DIR/Assembly-CSharp.dll" "$LIBS_DIR/"
cp "$GAME_DIR/UnityEngine.dll" "$LIBS_DIR/"
cp "$GAME_DIR/UnityEngine.CoreModule.dll" "$LIBS_DIR/"