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

            component.SetChargingPower(planetTransport, prefabDesc);
            component.SetTransportRanges();
            component.SetTransportLoads();
            component.AddDronesFromInventory(prefabDesc);

            // Extra configuration if ILS.
            if (!component.isStellar) return;
            component.SetToggles();
            component.SetMinWarpDistance();
            component.AddVesselsFromInventory(prefabDesc);
            component.AddWarperRequestToLastSlot(planetTransport);
        }
    }

    public static class Extensions
    {
        public static void SetChargingPower(
            this StationComponent component,
            PlanetTransport planetTransport,
            PrefabDesc prefabDesc
        )
        {
            if (component.isCollector) return;
            
            var maxEnergy = prefabDesc.workEnergyPerTick * 5;
            var percent = component.isStellar
                ? Config.ILS.ChargingPowerInPercent.Value
                : Config.PLS.ChargingPowerInPercent.Value;
            
            var workPerTick = maxEnergy * percent / 100;
            planetTransport.factory.powerSystem.consumerPool[component.pcId].workEnergyPerTick = workPerTick;
        }

        public static void SetTransportRanges(this StationComponent component)
        {
            component.tripRangeDrones = component.isStellar
                ? Util.ConvertDegreesToDroneRange(Config.ILS.DroneTransportRange.Value)
                : Util.ConvertDegreesToDroneRange(Config.PLS.DroneTransportRange.Value);

            if (!component.isStellar) return;
            component.tripRangeShips = Util.LY(Config.ILS.VesselTransportRange.Value);
        }

        public static void SetMinWarpDistance(this StationComponent component)
        {
            if (!component.isStellar) return;
            component.warpEnableDist = Util.AU(Config.ILS.MinWarpDistance.Value);
        }

        public static void SetTransportLoads(
            this StationComponent component
        )
        {
            if (component.isStellar)
            {
                component.deliveryDrones = Config.ILS.MinDroneLoad.Value;
                component.deliveryShips =  Config.ILS.MinVesselLoad.Value;
            }
            else
            {
                component.deliveryDrones = Config.PLS.MinDroneLoad.Value;
            }
        }

        public static void SetToggles(
            this StationComponent component
        )
        {
            component.warperNecessary = Config.ILS.MustEquipWarp.Value;
            component.includeOrbitCollector = Config.ILS.UseOrbitalCollectors.Value;
        }

        public static void AddDronesFromInventory(this StationComponent component, PrefabDesc prefabDesc)
        {
            var percentage = component.isStellar
                ? Config.ILS.DroneInsertPercentage.Value
                : Config.PLS.DroneInsertPercentage.Value;

            var maxToTake = (int)Math.Round(prefabDesc.stationMaxDroneCount * percentage);
            var numAvailable = GameMain.mainPlayer.package.TakeItem(5001, maxToTake);
            component.idleDroneCount = numAvailable;
        }

        public static void AddVesselsFromInventory(this StationComponent component, PrefabDesc prefabDesc)
        {
            var percentage = component.isStellar
                ? Config.ILS.DroneInsertPercentage.Value
                : Config.PLS.DroneInsertPercentage.Value;
            var maxToTake = (int)Math.Round(prefabDesc.stationMaxShipCount * percentage);
            var numAvailable = GameMain.mainPlayer.package.TakeItem(5002, maxToTake);
            component.idleShipCount = numAvailable;
        }

        public static void AddWarperRequestToLastSlot(
            this StationComponent component,
            PlanetTransport planetTransport
        )
        {
            if (!component.isStellar) return;
            if (!Config.ILS.WarperInLastItemSlot.Value) return;
            
            planetTransport.SetStationStorage(
                component.id,
                component.storage.Length - 1,
                1210, // Item ID for Warpers
                100,
                Config.ILS.WarperLocalMode.Value,
                Config.ILS.WarperRemoteMode.Value,
                GameMain.mainPlayer.package
            );

            planetTransport.gameData.galacticTransport.RefreshTraffic(component.gid);
        }
    }
}