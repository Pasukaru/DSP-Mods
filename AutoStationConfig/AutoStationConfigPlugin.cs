using System.Security;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[module: UnverifiableCode]

namespace Pasukaru.DSP.AutoStationConfig
{
    [BepInPlugin("pasukaru.dsp.AutoStationConfig", "AutoStationConfig", "1.3.0")]
    [BepInProcess("DSPGAME.exe")]
    public class AutoStationConfigPlugin : BaseUnityPlugin
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public new static ManualLogSource Logger;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            Logger = base.Logger;
            Pasukaru.DSP.AutoStationConfig.Config.Init(Config);
            var harmony = new Harmony("pasukaru.dsp.AutoStationConfig");
            harmony.PatchAll(typeof(PlanetTransportPatch));
            Logger.LogInfo("Loaded!");
        }
    }
}