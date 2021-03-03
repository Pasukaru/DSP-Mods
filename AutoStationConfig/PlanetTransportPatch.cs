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

            component.SetMaxPower(planetTransport, prefabDesc);
            component.SetTransportRanges();
            component.SetTransportLoads();
            component.AddDronesFromInventory(prefabDesc);

            // Extra configuration if ILS.
            if (component.isStellar)
            {
                component.SetToggles();
                component.SetMinWarpDistance();
                component.AddVesselsFromInventory(prefabDesc);
                component.AddWarperRequestToLastSlot(planetTransport);
            }
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
            // Default is 60 MW, max is 300 MW (60 * 5) and min is 30 MW (60 * 0.5).
            var energyMultiplier = minMaxDeterminator(0.5, 5, AutoStationConfigPlugin.PowerLoad.Value);

            var maxEnergyPerTick = prefabDesc.workEnergyPerTick * energyMultiplier;

            // Required type is long.
            planetTransport.factory.powerSystem.consumerPool[component.pcId].workEnergyPerTick = Convert.ToInt64(maxEnergyPerTick);
        }

        public static void SetTransportRanges(
            this StationComponent component
            )
        {
            // Hardcoded to be 180° until conversion below can be determined.
            var droneTransportRange = -1;

            // TODO: Need to figure out how to convert a range of 20 and 180 degree value to the game equivalent.
            // var droneTransportRange = minMaxDeterminator(20, 180, AutoStationConfigPlugin.DroneTransportRange.Value);

            component.tripRangeDrones = droneTransportRange;

            if (component.isStellar)
            {
                // Using 61 as a fallback since above 60 the game jumps to infinity.
                var vesselTransportLoad = minMaxDeterminator(-1, 61, AutoStationConfigPlugin.VesselTransportRange.Value);
                component.tripRangeShips = vesselTransportLoad == -1 || vesselTransportLoad == 61 ? Util.LY(10000) : Util.LY(vesselTransportLoad);
            }

        }

        public static void SetMinWarpDistance(this StationComponent component)
        {
            // Minimum 0.5 AUs, Maximum 60 AUs.
            var minWarpDistance = minMaxDeterminator(0.5, 60, AutoStationConfigPlugin.MinWarpDistance.Value);
            component.warpEnableDist = Util.AU(minWarpDistance);
        }

        public static void SetTransportLoads(
           this StationComponent component
           )
        {
            // Minimum 1%, maximum 100%.
            var droneTransportLoad = minMaxDeterminator(1, 100, AutoStationConfigPlugin.DroneLoad.Value);
            component.deliveryDrones = Convert.ToInt32(droneTransportLoad);

            if (component.isStellar)
            {
                var vesselTransportLoad = minMaxDeterminator(1, 100, AutoStationConfigPlugin.VesselLoad.Value);
                component.deliveryShips = Convert.ToInt32(vesselTransportLoad);
            }

        }

        public static void SetToggles(
            this StationComponent component
            )
        {
            component.warperNecessary = AutoStationConfigPlugin.MustEquipWarp.Value;
            component.includeOrbitCollector = AutoStationConfigPlugin.UseOrbitalCollectors.Value;
        }

        public static void AddDronesFromInventory(this StationComponent component, PrefabDesc prefabDesc)
        {
            var droneAutofillCount = minMaxDeterminator(0, prefabDesc.stationMaxDroneCount, AutoStationConfigPlugin.DroneInsertCount.Value);
            var numAvailable = GameMain.mainPlayer.package.TakeItem(5001, Convert.ToInt32(droneAutofillCount));
            component.idleDroneCount = numAvailable;
        }

        public static void AddVesselsFromInventory(this StationComponent component, PrefabDesc prefabDesc)
        {
            var vesselAutofillCount = minMaxDeterminator(0, prefabDesc.stationMaxShipCount, AutoStationConfigPlugin.VesselInsertCount.Value);
            var numAvailable = GameMain.mainPlayer.package.TakeItem(5002, Convert.ToInt32(vesselAutofillCount));
            component.idleShipCount = numAvailable;
        }

        public static void AddWarperRequestToLastSlot(
            this StationComponent component,
            PlanetTransport planetTransport
        )
        {

            var storageType = AutoStationConfigPlugin.WarperAutoDemand.Value ? ELogisticStorage.Demand : ELogisticStorage.None;

            planetTransport.SetStationStorage(
                component.id,
                component.storage.Length - 1,
                1210, // Item ID for Warpers
                100,
                storageType,
                ELogisticStorage.None, GameMain.mainPlayer.package
            );
            planetTransport.gameData.galacticTransport.RefreshTraffic(component.gid);
        }

        /**
         * Utility method to safety check the ranges and of the configuration field.
         */
        private static double minMaxDeterminator(
            double min, double max, double testValue)
        {

            if (testValue < min)
            {
                return min;
            }

            else if (testValue > max)
            {
                return max;
            }

            return testValue;
        }
    }
}