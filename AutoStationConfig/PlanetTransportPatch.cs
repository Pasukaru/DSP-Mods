using HarmonyLib;

namespace Pasukaru.DSP.AutoStationConfig
{
    [HarmonyPatch(typeof(PlanetTransport))]
    public class PlanetTransportPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("NewStationComponent")]
        public static void NewStationComponent(
            PlanetTransport __instance,
            StationComponent __result
        )
        {
            var component = __result;
            if (!component.isStellar) return;
            
            var itemProto = LDB.items.Select(__instance.factory.entityPool[component.entityId].protoId);
            var prefabDesc = itemProto.prefabDesc;
            var maxEnergyPerTick = itemProto.prefabDesc.workEnergyPerTick * 5L;

            __instance.factory.powerSystem.consumerPool[component.pcId].workEnergyPerTick = maxEnergyPerTick;
            
            component.tripRangeDrones = -1;
            component.deliveryDrones = 100;
            
            component.tripRangeShips = Util.LY(10000);
            component.warpEnableDist = Util.AU(0.5);

            var drones = GameMain.mainPlayer.package.TakeItem(5001, prefabDesc.stationMaxDroneCount);
            component.idleDroneCount = drones;
            
            var vessels = GameMain.mainPlayer.package.TakeItem(5002, prefabDesc.stationMaxShipCount);
            component.idleShipCount = vessels;
            
            __instance.SetStationStorage(component.id, component.storage.Length-1, 1210, 100, ELogisticStorage.Demand, ELogisticStorage.None, GameMain.mainPlayer.package);
            __instance.gameData.galacticTransport.RefreshTraffic(component.gid);
        }
    }
}