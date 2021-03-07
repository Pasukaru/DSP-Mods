using HarmonyLib;

namespace Pasukaru.DSP.FixStationPower
{
    
    [HarmonyPatch(typeof(GameSave))]
    public class GameSavePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("LoadCurrentGame")]
        public static void LoadCurrentGamePatch(bool __result)
        {
            if (!__result) return;
            FixStationPowerPlugin.Logger.LogInfo("Save loaded, patching stations...");
            var count = 0;
            foreach (var star in GameMain.galaxy.stars)
            {
                foreach (var planet in star.planets)
                {
                    var planetTransport = planet?.factory?.transport;
                    var stationPool = planetTransport?.stationPool;
                    if (stationPool == null) continue;
                    if (stationPool.Length == 0) continue;
                    var factory = planetTransport.factory;
                    var consumerPool = factory?.powerSystem?.consumerPool;
                    if (consumerPool == null) continue;
                    var entityPool = factory.entityPool;
                    if (entityPool == null) continue;
                    foreach (var component in stationPool)
                    {
                        if (component == null || component.isCollector) continue;
                        if (entityPool.Length < component.entityId) continue;
                        var entity = entityPool[component.entityId];
                        var ldb = LDB.items.Select(entity.protoId);
                        if (ldb == null) continue;
                        var prefab = ldb.prefabDesc;
                        if (consumerPool.Length < component.pcId) continue;
                        
                        var maxEnergy = prefab.workEnergyPerTick * 5;
                        var percent = component.isStellar
                            ? Config.ILS.ChargingPowerInPercent.Value
                            : Config.PLS.ChargingPowerInPercent.Value;
            
                        var workPerTick = maxEnergy * percent / 100;
                        consumerPool[component.pcId].workEnergyPerTick = workPerTick;
                        count++;
                    }
                }
            }
            FixStationPowerPlugin.Logger.LogInfo($"Fixed power of {count} Stations!");
        }
    }
}