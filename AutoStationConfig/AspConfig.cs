using BepInEx.Configuration;

// ReSharper disable ConvertToConstant.Local
namespace Pasukaru.DSP.AutoStationConfig
{
    public static class AspConfig
    {
        private static readonly string GENERAL_SECTION = "General";
        private static readonly string PLS_SECTION = "Planetary Logistics Station";
        private static readonly string ILS_SECTION = "Interstellar Logistics Station";
        private static readonly string AMM_SECTION = "Advanced Mining Machine";
        private static readonly string LD_SECTION = "Logistics Distributor";

        public static class General
        {
            public static ConfigEntry<bool> NotifyWhenDroneOrVesselMissing;
            public static ConfigEntry<bool> PlaySoundWhenDroneOrVesselMissing;
            public static ConfigEntry<bool> PatchWarperConfigOnSaveLoad;
            public static ConfigEntry<bool> EnableAutoReplenish;
        }

        public static class PLS
        {
            public static ConfigEntry<int> ChargingPowerInPercent;

            public static ConfigEntry<int> DroneTransportRange;
            public static ConfigEntry<int> MinDroneLoad;
            public static ConfigEntry<double> DroneInsertPercentage;

            public static ConfigEntry<int> StackCount;
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
            public static ConfigEntry<int> WarperDemand;
            public static ConfigEntry<ELogisticStorage> WarperLocalMode;
            public static ConfigEntry<ELogisticStorage> WarperRemoteMode;

            public static ConfigEntry<bool> UseOrbitalCollectors;
            public static ConfigEntry<bool> MustEquipWarp;

            public static ConfigEntry<int> StackCount;
        }

        public static class AMM
        {
            public static ConfigEntry<int> GatheringSpeedInPercent;

            public static ConfigEntry<int> StackCount;
        }

        public static class LD
        {
            public static ConfigEntry<int> ChargingPowerInPercent;
            public static ConfigEntry<double> BotInsertPercentage;
            public static ConfigEntry<bool> AlwaysGuessFilter;
        }


        internal static void Init(ConfigFile config)
        {
            ////////////////////
            // General Config //
            ////////////////////
            General.NotifyWhenDroneOrVesselMissing =
                config.Bind(GENERAL_SECTION, "Notify when Drone or Vessel missing", true,
                    "Sends a notification when there are not enough drones or vessel in your inventory to auto fill");

            General.PlaySoundWhenDroneOrVesselMissing =
                config.Bind(GENERAL_SECTION, "Play sound when Drone or Vessel missing", true,
                    "Plays a sound along to the drone/vessel notification. Only works when the notification is turned on.");

            General.PatchWarperConfigOnSaveLoad =
                config.Bind(GENERAL_SECTION, "Patch ILS Warper config after loading a save", true,
                    "Patches broken warper configs from previous versions of this mod. See github ticket #12 for details: https://github.com/Pasukaru/DSP-Mods/issues/12\n\n" +
                    "It only needs to run once. So load up your save game, then save it and close the game. Then disable this setting for a possibly faster loading (on large save games).");

            General.EnableAutoReplenish =
                config.Bind(GENERAL_SECTION, "Enable AutoReplenishment", false,
                    "Enable auto-replenishment for PLS/ILS/LD by default, this will override use of Drone/Vessel/Bot Insert Percentage config entries");

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
                    new AcceptableValueList<int>(10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95,
                        100),
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
                    "Amount of drones to insert. For vanilla PLS, 0.05 = 5% = 50/100*5 = 2.5, rounded down to 2.",
                    new AcceptableValueRange<double>(0, 1),
                    new { }
                )
            );

            PLS.StackCount = config.Bind(PLS_SECTION, "Output Cargo Stack Count", 0,
                new ConfigDescription(
                    "Default output cargo stack count for PLS. 0 = Use tech limit",
                    new AcceptableValueRange<int>(0, 4),
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
                    "Amount of drones to insert. For vanilla ILS, 0.05 = 5% = 50/100*5 = 2.5, rounded down to 2.",
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
                    new AcceptableValueRange<int>(1, 100),
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

            ILS.WarperDemand = config.Bind(ILS_SECTION, "Warper Demand", 1, new ConfigDescription(
                    "Amount of warpers to request, divided by 100. Ex: Set demand to 500 warpers in ILS, 500/100 = 5",
                    new AcceptableValueRange<int>(1, 100),
                    new { }
                )
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

            ILS.StackCount = config.Bind(ILS_SECTION, "Output Cargo Stack Count", 0,
                new ConfigDescription(
                    "Default output cargo stack count for ILS. 0 = Use tech limit",
                    new AcceptableValueRange<int>(0, 4),
                    new { }
                )
            );

            ////////////////
            // AMM Config //
            ////////////////

            AMM.GatheringSpeedInPercent = config.Bind(AMM_SECTION, "Gathering Speed", 100,
                new ConfigDescription(
                    "Gathering speed in percent for AMM.",
                    new AcceptableValueList<int>(100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230,
                        240, 250, 260, 270, 280, 290, 300),
                    new { }
                )
            );

            AMM.StackCount = config.Bind(AMM_SECTION, "Output Cargo Stack Count", 0,
                new ConfigDescription(
                    "Default output cargo stack count for AMM. 0 = Use tech limit",
                    new AcceptableValueRange<int>(0, 4),
                    new { }
                )
            );

            ///////////////
            // LD Config //
            ///////////////

            LD.ChargingPowerInPercent = config.Bind(LD_SECTION, "Charging Power", 100,
                new ConfigDescription(
                    "Maximum power load in percent. For a vanilla LD, 10% = 900kW, 100% = 9MW",
                    new AcceptableValueRange<int>(10, 100),
                    new { }
                )
            );
 
            LD.BotInsertPercentage = config.Bind(LD_SECTION, "Bot Insert Percentage", 1d,
                new ConfigDescription(
                    "Amount of bots to insert. For vanilla LD, 0.25 = 10% = 10/100*15 = 2.5, rounded down to 2.",
                    new AcceptableValueRange<double>(0, 1),
                    new { }
                )
            );

            LD.AlwaysGuessFilter = config.Bind(LD_SECTION, "Always Guess Filter", false,
                "Always guess item filter even when copying LD. Note: Filter on copied LD will be retained if nothing can be guessed out.");
        }
    }
}