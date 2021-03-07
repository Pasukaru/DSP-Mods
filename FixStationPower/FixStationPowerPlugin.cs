using System.Security;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[module: UnverifiableCode]

namespace Pasukaru.DSP.FixStationPower
{
    [BepInPlugin("pasukaru.dsp.FixStationPower", "FixStationPower", "1.0.0")]
    [BepInProcess("DSPGAME.exe")]
    public class FixStationPowerPlugin : BaseUnityPlugin
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public new static ManualLogSource Logger;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            Logger = base.Logger;
            Pasukaru.DSP.FixStationPower.Config.Init(Config);
            var harmony = new Harmony("pasukaru.dsp.FixStationPower");
            harmony.PatchAll(typeof(GameSavePatch));
            Logger.LogInfo("Loaded!");
        }
    }
}