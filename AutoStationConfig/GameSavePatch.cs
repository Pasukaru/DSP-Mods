using HarmonyLib;

namespace Pasukaru.DSP.AutoStationConfig
{
    
    [HarmonyPatch(typeof(GameSave))]
    public class GameSavePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("LoadCurrentGame")]
        public static void LoadCurrentGamePatch(bool __result)
        {
            if (!__result) return;
            AutoStationConfigPlugin.Logger.LogInfo("Save loaded, patching stations...");
            var count = 0;
            foreach (var star in GameMain.galaxy.stars)
            {
                foreach (var planet in star.planets)
                {
                    var factory = planet?.factory;
                    if (factory == null) continue;
                    var planetTransport = planet.factory.transport;
                    var stationPool = planetTransport?.stationPool;
                    if (stationPool == null) continue;
                    if (stationPool.Length == 0) continue;
                    for (var stationId = 0; stationId < stationPool.Length; stationId++)
                    {
                        var component = stationPool[stationId];
                        if(component == null) continue;
                        if(component.FixDuplicateWarperStores(stationId, factory))
                            count++;
                    }
                }
            }
            AutoStationConfigPlugin.Logger.LogInfo($"Fixed warper config of {count} stations!");
        }
    }
}