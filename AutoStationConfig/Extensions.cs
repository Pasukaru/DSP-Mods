using System;
using System.Linq;

namespace Pasukaru.DSP.AutoStationConfig
{
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
                component.deliveryShips = Config.ILS.MinVesselLoad.Value;
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

            var maxToTake = Convert.ToInt32(Math.Floor(prefabDesc.stationMaxDroneCount * percentage));
            var numAvailable = GameMain.mainPlayer.package.TakeItem(ItemIds.Drone, maxToTake, out _);
            component.idleDroneCount = numAvailable;
            if (Config.General.NotifyWhenDroneOrVesselMissing.Value && numAvailable < maxToTake)
            {
                UIRealtimeTip.PopupAhead("Not enough Logistics Drones in inventory!".Translate(),
                    Config.General.PlaySoundWhenDroneOrVesselMissing.Value);
            }
        }

        public static void AddVesselsFromInventory(this StationComponent component, PrefabDesc prefabDesc)
        {
            if (!component.isStellar) return;
            var percentage = Config.ILS.VesselInsertPercentage.Value;
            var maxToTake = Convert.ToInt32(Math.Floor(prefabDesc.stationMaxShipCount * percentage));
            var numAvailable = GameMain.mainPlayer.package.TakeItem(ItemIds.Vessel, maxToTake, out _);
            component.idleShipCount = numAvailable;
            if (Config.General.NotifyWhenDroneOrVesselMissing.Value && numAvailable < maxToTake)
            {
                UIRealtimeTip.PopupAhead("Not enough Logistics Vessels in inventory!".Translate(),
                    Config.General.PlaySoundWhenDroneOrVesselMissing.Value);
            }
        }

        public static void AddWarperRequestToLastSlot(
            this StationComponent component,
            PlanetTransport planetTransport
        )
        {
            if (!component.isStellar) return;
            if (!Config.ILS.WarperInLastItemSlot.Value) return;
            if (component.HasItemInAnySlot(ItemIds.Warper)) return;

            planetTransport.SetStationStorage(
                component.id,
                component.storage.Length - 1,
                ItemIds.Warper,
                100,
                Config.ILS.WarperLocalMode.Value,
                Config.ILS.WarperRemoteMode.Value,
                GameMain.mainPlayer
            );

            planetTransport.gameData.galacticTransport.RefreshTraffic(component.gid);
        }

        private static bool HasItemInAnySlot(this StationComponent component, int itemId) 
            => component.storage.Any(storage => storage.itemId == itemId);
    }
}