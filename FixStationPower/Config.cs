using BepInEx.Configuration;

// ReSharper disable ConvertToConstant.Local
namespace Pasukaru.DSP.FixStationPower
{
    
    public static class Config
    {
        private static readonly string PLS_SECTION = "Planetary Logistics Station";
        private static readonly string ILS_SECTION = "Interstellar Logistics Station";

        public static class PLS
        {
            public static ConfigEntry<int> ChargingPowerInPercent;
        }

        public static class ILS
        {
            public static ConfigEntry<int> ChargingPowerInPercent;
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

        }
    }
}