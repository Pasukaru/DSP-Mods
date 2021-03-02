using System.Security;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;

[module: UnverifiableCode]

namespace Pasukaru.DSP.AutoStationConfig
{
    [BepInPlugin("pasukaru.dsp.AutoStationConfig", "AutoStationConfig", "1.1.0")]
    [BepInProcess("DSPGAME.exe")]
    public class AutoStationConfigPlugin : BaseUnityPlugin
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public new static ManualLogSource Logger;

        // ReSharper disable once ArrangeTypeMemberModifiers

        // Config variables
        public static ConfigEntry<double> PowerLoad;
        public static ConfigEntry<int> DroneTransportRange;
        public static ConfigEntry<int> VesselTransportRange;
        public static ConfigEntry<bool> UseOrbitalCollectors;
        public static ConfigEntry<double> MinWarpDistance;
        public static ConfigEntry<bool> MustEquipWarp;
        public static ConfigEntry<int> DroneLoad;
        public static ConfigEntry<int> VesselLoad;

        void InitConfig()
        {
            PowerLoad = Config.Bind("General", "PowerLoad", 5.0, "Multiplier to set the power load to. 5 is full power.");
            // TODO: Determine if the values here are meant to be degrees?
            DroneTransportRange = Config.Bind("General", "DroneTransportRange", -1, "Planetary Drone range in degrees (-1 = Max 180°). Harcoded to 180 - Not supported at the moment.");
            VesselTransportRange = Config.Bind("General", "VesselTransportRange", -1, "Interplanetary Drone range in AUs. -1 or > 60 sets it to Infinity. 1 to 60 otherwise.");
            UseOrbitalCollectors = Config.Bind("General.Toggles", "UseOrbitalCollectors", true, "Toggle to retrieve from Orbital collectors.");
            MinWarpDistance = Config.Bind("General", "MinWarpDistance", 0.5, "Minimum distance to enable warp in AUs. 0.5 to 60");
            MustEquipWarp = Config.Bind("General.Toggles", "MustEquipWarp", true, "Toggle to only depart if Warp is available.");
            DroneLoad = Config.Bind("General", "DroneLoad", 100, "Planetary Drone load (in percentage)");
            VesselLoad = Config.Bind("General", "VesselLoad", 100, "Interplanetary Vessel load (in percentage)");
        }


        void Start()
        {
            InitConfig();
            Logger = base.Logger;
            Harmony.CreateAndPatchAll(typeof(PlanetTransportPatch));
            Logger.LogInfo("Loaded!");
        }

    }
}