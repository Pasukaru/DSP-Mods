﻿
# AutoStationConfig

[![GitHub](https://img.shields.io/github/license/pasukaru/DSP-Mods?style=for-the-badge)](https://github.com/Pasukaru/DSP-Mods/tree/main/AutoStationConfig)

## Features

This mod automatically configures PLS (Planetary Logistics Stations) and ILS (Interstellar Logistics Station) when they are placed. 

This mod is intended for faster scaling of your production lines in end-game.

By default, PLS will be setup like this (all of this can be changed via config):
- Automatically **add Drones from inventory** (if you don't have enough, it will use all that are available)
- **Max Charging Power**: Max
- **Transport Range of Drones**: Max (180°)
- **Min Load of Drones**: 100%

By default, ILS will be setup like this (all of this can be changed via config):
- Automatically **add Drones & Vessels from inventory** (if you don't have enough, it will use all that are available)
- **Max Charging Power**: Max
- **Transport Range of Drones & Vessels**: Max (180° (Drones), Infinite (Vessels))
- **Distance to enable warp**: Min (0.5 AU)
- **Min Load of Drones & Vessels** 100%
- **Last item slot**: `Space Warper | 100 | Local Demand | Remote Storage`

When there are not enough drones or vessels in your inventory, a message will pop up and a sound will play. 

To change the config, enable the mod and start the game once. Once in the main menu, close the game. 
The available settings will then appear in the r2modman Config Editor (look for pasukaru.dsp.AutoStationConfig).

## Changelog

### 1.3.1
Compatibility with latest DSP version (2021-04-03).

Thanks to [fuhaiwei](https://github.com/fuhaiwei) for the PR!

### 1.3.0
Send a notification when there are not enough drones / vessels in inventory to auto fill.

### 1.2.1
Fixed Vessel insertion using configured drone percentage.

Thanks to [Veretragna](https://github.com/Veretragna) for finding and reporting it!

### 1.2.0
Added configurable mod settings.

Credits & Thanks to [3therios](https://github.com/3therios) for the heavy lifting!

### 1.1.0
Added settings for PLS

### 1.0.0
Initial Release
