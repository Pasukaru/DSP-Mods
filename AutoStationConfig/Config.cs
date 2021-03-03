using BepInEx.Configuration;

// ReSharper disable ConvertToConstant.Local
namespace Pasukaru.DSP.AutoStationConfig
{
    
    public static class Config
    {
        private static readonly string PLS_SECTION = "Planetary Logistics Station";
        private static readonly string ILS_SECTION = "Interstellar Logistics Station";

        public static class PLS
        {
            public static ConfigEntry<int> ChargingPowerInPercent;

            public static ConfigEntry<int> DroneTransportRange;
            public static ConfigEntry<int> MinDroneLoad;
            public static ConfigEntry<double> DroneInsertPercentage;
        }

        public static class ILS
        {
            public static ConfigEntry<int> ChargingPowerInPercent;

            public static ConfigEntry<int> DroneTransportRange;
            public static ConfigEntry<int> MinDroneLoad;
            public static ConfigEntry<double> DroneInsertPercentage;

            public static ConfigEntry<int> VesselTransportRange;
            public static ConfigEntry<int> MinVesselLoad;
            public static ConfigEntry<double> VesselInsertPercentage;

            public static ConfigEntry<double> MinWarpDistance;
            public static ConfigEntry<bool> WarperInLastItemSlot;
            public static ConfigEntry<ELogisticStorage> WarperLocalMode;
            public static ConfigEntry<ELogisticStorage> WarperRemoteMode;

            public static ConfigEntry<bool> UseOrbitalCollectors;
            public static ConfigEntry<bool> MustEquipWarp;
        }


        internal static void Init(ConfigFile config)
        {
            ////////////////
            // PLS Config //
            ////////////////

            // Slider in Game UI uses prefabDesc.workEnergyPerTick*5 as MAX
            // prefabDesc.workEnergyPerTick/2 as MIN.
            // Which means, MIN = 10%
            // Slider increments in 1%, so int range 10-100 is a perfect fit
            PLS.ChargingPowerInPercent = config.Bind(PLS_SECTION, "Charging Power", 100,
                new ConfigDescription(
                    "Maximum power load in percent. For a vanilla PLS, 10% = 6MW, 100% = 60MW",
                    new AcceptableValueRange<int>(10, 100),
                    new { }
                )
            );

            // Slider in Game UI uses 20° - 180° Which will be converted to an internal representation. See: Util.ConvertDegreesToDroneRange
            // Slider increments in 1°, so int range 20-180 is a perfect fit
            PLS.DroneTransportRange = config.Bind(PLS_SECTION, "Drone Transport Range", 180,
                new ConfigDescription(
                    "Planetary Drone range in degrees (°).",
                    new AcceptableValueRange<int>(20, 180),
                    new { }
                )
            );

            // Slider allows 1, 10, 20, 30, ...100%
            PLS.MinDroneLoad = config.Bind(PLS_SECTION, "Min Load of Drones", 100,
                new ConfigDescription(
                    "Min. Load of Drones in percent.",
                    new AcceptableValueList<int>(1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100),
                    new { }
                )
            );

            PLS.DroneInsertPercentage = config.Bind(PLS_SECTION, "Drone Insert Percentage", 1d,
                new ConfigDescription(
                    "Amount of drones to insert. For vanilla ILS, 0.05 = 5% = 50/100*0.5 = 2.5, rounded down to 2.",
                    new AcceptableValueRange<double>(0, 1),
                    new { }
                )
            );

            ////////////////
            // ILS Config //
            ////////////////

            ILS.ChargingPowerInPercent = config.Bind(ILS_SECTION, "Charging Power", 100,
                new ConfigDescription(
                    "Maximum power load in percent. For a vanilla ILS, 10% = 30MW, 100% = 300MW",
                    new AcceptableValueRange<int>(10, 100),
                    new { }
                )
            );

            ILS.DroneTransportRange = config.Bind(ILS_SECTION, "Drone Transport Range", 180,
                new ConfigDescription(
                    "Planetary Drone range in degrees (°).",
                    new AcceptableValueRange<int>(20, 180),
                    new { }
                )
            );

            ILS.MinDroneLoad = config.Bind(ILS_SECTION, "Min Load of Drones", 100,
                new ConfigDescription(
                    "Min. Load of Drones in percent.",
                    new AcceptableValueList<int>(1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100),
                    new { }
                )
            );

            ILS.DroneInsertPercentage = config.Bind(ILS_SECTION, "Drone Insert Percentage", 1d,
                new ConfigDescription(
                    "Amount of drones to insert. For vanilla ILS, 0.05 = 5% = 50/100*0.5 = 2.5, rounded down to 2.",
                    new AcceptableValueRange<double>(0, 1),
                    new { }
                )
            );

            ILS.VesselTransportRange = config.Bind(ILS_SECTION, "Vessel Transport Range", 10000,
                new ConfigDescription(
                    "Interstellar Vessel range in Light Years (LY). 10000 = infinite",
                    new AcceptableValueList<int>(1, 2, 3, 4, 5, 7, 8, 9, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32,
                        34, 36, 38, 40, 42, 44, 46, 48, 50, 52, 54, 56, 58, 60, 10000),
                    new { }
                )
            );
            
            ILS.MinVesselLoad = config.Bind(ILS_SECTION, "Min Load of Vessels", 100,
                new ConfigDescription(
                    "Min. Load of Vessels in percent.",
                    new AcceptableValueList<int>(1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100),
                    new { }
                )
            );
            
            ILS.VesselInsertPercentage = config.Bind(ILS_SECTION, "Vessel Insert Percentage", 1d,
                new ConfigDescription(
                    "Amount of vessels to insert. 0.01 = 1%. For vanilla ILS, 0.15 => 10/100*15 = 1.5, rounded down to 1.",
                    new AcceptableValueRange<double>(0, 1),
                    new { }
                )
            );

            ILS.MinWarpDistance = config.Bind(ILS_SECTION, "Distance to enable Warp", 0.5, new ConfigDescription(
                    "Minimum distance to enable warp in AUs. 0.5 to 60",
                    new AcceptableValueList<double>(0.5, 1, 1.5, 2, 2.5, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 18,
                        20, 60),
                    new { }
                )
            );
            
            
            ILS.WarperInLastItemSlot = config.Bind(ILS_SECTION, "Add Warpers in last Slot", true,
                    "If true, the last item slot will automatically select Space Warpers"
            );
            
            ILS.WarperLocalMode = config.Bind(ILS_SECTION, "Warper Local Mode", ELogisticStorage.Demand,
                "Local logistics mode of the Warpers when \"Add Warpers in last Slot\" is true"
            );
            
            ILS.WarperRemoteMode = config.Bind(ILS_SECTION, "Warper Remote Mode", ELogisticStorage.None,
                "Remote logistics mode of the Warpers when \"Add Warpers in last Slot\" is true"
            );
            
            ILS.UseOrbitalCollectors = config.Bind(ILS_SECTION, "Pull from Orbital Collectors?", true,
                "Toggle to retrieve from Orbital collectors.");
            
            ILS.MustEquipWarp = config.Bind(ILS_SECTION, "Must Equip Warpers", true,
                "Toggle for must equip warpers.");
        }
    }
}