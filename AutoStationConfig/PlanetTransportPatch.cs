using System;
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
            if (component.isCollector) return;
            
            var planetTransport = __instance;
            var itemProto = LDB.items.Select(planetTransport.factory.entityPool[component.entityId].protoId);
            var prefabDesc = itemProto.prefabDesc;

            component.deliveryDrones = 100; // Min Load of Drones (in percent)
            component.tripRangeDrones = -1; // Drone range. -1 = Max (180°)
            component.SetMaxPower(planetTransport, prefabDesc);
            component.AddDronesFromInventory(prefabDesc);

            if (!component.isStellar) return;
            component.deliveryShips = 100; // Min Load of Vessels (in percent)
            component.tripRangeShips = Util.LY(10000);
            component.warpEnableDist = Util.AU(0.5);
            component.AddVesselsFromInventory(prefabDesc);
            component.AddWarperRequestToLastSlot(planetTransport);
        }
    }

    public static class Extensions
    {
        public static void SetMaxPower(
            this StationComponent component,
            PlanetTransport planetTransport,
            PrefabDesc prefabDesc
        )
        {
            var maxEnergyPerTick = prefabDesc.workEnergyPerTick * 5L;
            planetTransport.factory.powerSystem.consumerPool[component.pcId].workEnergyPerTick = maxEnergyPerTick;
        }

        public static void AddDronesFromInventory(this StationComponent component, PrefabDesc prefabDesc)
        {
            var numAvailable = GameMain.mainPlayer.package.TakeItem(5001, prefabDesc.stationMaxDroneCount);
            component.idleDroneCount = numAvailable;
        }

        public static void AddVesselsFromInventory(this StationComponent component, PrefabDesc prefabDesc)
        {
            var numAvailable = GameMain.mainPlayer.package.TakeItem(5002, prefabDesc.stationMaxShipCount);
            component.idleShipCount = numAvailable;
        }

        public static void AddWarperRequestToLastSlot(
            this StationComponent component,
            PlanetTransport planetTransport
        )
        {
            planetTransport.SetStationStorage(
                component.id,
                component.storage.Length - 1,
                1210,
                100,
                ELogisticStorage.Demand,
                ELogisticStorage.None, GameMain.mainPlayer.package
            );
            planetTransport.gameData.galacticTransport.RefreshTraffic(component.gid);
        }
    }
}