# AutoStationConfig

[![GitHub](https://img.shields.io/github/license/pasukaru/DSP-Mods?style=for-the-badge)](https://github.com/Pasukaru/DSP-Mods/tree/main/AutoStationConfig)

## Features

This mod automatically configures PLS (Planetary Logistics Stations) ILS (Interstellar Logistics Station) when they are placed. 
Currently, the mod itself is not configurable.

This mod is intended for faster scaling of your production lines in end-game.

PLS will be setup like this:
- Automatically **add Drones from inventory** (if you don't have enough, it will use all that are available)
- **Max Charging Power**: Max
- **Transport Range of Drones**: Max (180°)
- **Min Load of Drones**: 100%

ILS will be setup like this:
- Automatically **add Drones & Vessels from inventory** (if you don't have enough, it will use all that are available)
- **Max Charging Power**: Max
- **Transport Range of Drones & Vessels**: Max (180° (Drones), Infinite (Vessels))
- **Distance to enable warp**: Min (0.5 AU)
- **Min Load of Drones & Vessels** 100%
- **Last item slot**: `Space Warper | 100 | Local Demand | Remote Storage`

## Changelog

### 1.1.0
Added settings for PLS

### 1.0.0
Initial Release
