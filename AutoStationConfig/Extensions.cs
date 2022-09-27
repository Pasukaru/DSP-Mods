using System;
using System.Collections.Generic;
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
            var maxEnergy = prefabDesc.workEnergyPerTick * 5;
            var percent = component.isStellar
                ? AspConfig.ILS.ChargingPowerInPercent.Value
                : AspConfig.PLS.ChargingPowerInPercent.Value;

            var workPerTick = maxEnergy * percent / 100;
            planetTransport.factory.powerSystem.consumerPool[component.pcId].workEnergyPerTick = workPerTick;
        }

        public static void SetTransportRanges(this StationComponent component)
        {
            component.tripRangeDrones = component.isStellar
                ? Util.ConvertDegreesToDroneRange(AspConfig.ILS.DroneTransportRange.Value)
                : Util.ConvertDegreesToDroneRange(AspConfig.PLS.DroneTransportRange.Value);

            if (!component.isStellar) return;
            component.tripRangeShips = Util.LY(AspConfig.ILS.VesselTransportRange.Value);
        }

        public static void SetMinWarpDistance(this StationComponent component)
        {
            if (!component.isStellar) return;
            component.warpEnableDist = Util.AU(AspConfig.ILS.MinWarpDistance.Value);
        }

        public static void SetTransportLoads(
            this StationComponent component
        )
        {
            if (component.isStellar)
            {
                component.deliveryDrones = AspConfig.ILS.MinDroneLoad.Value;
                component.deliveryShips = AspConfig.ILS.MinVesselLoad.Value;
            }
            else
            {
                component.deliveryDrones = AspConfig.PLS.MinDroneLoad.Value;
            }
        }

        public static void SetToggles(
            this StationComponent component
        )
        {
            component.warperNecessary = AspConfig.ILS.MustEquipWarp.Value;
            component.includeOrbitCollector = AspConfig.ILS.UseOrbitalCollectors.Value;
        }

        public static void SetStackCount(this StationComponent component)
        {
            if (component.isCollector) return;
            if (component.isVeinCollector)
                component.pilerCount = AspConfig.AMM.StackCount.Value;
            else
                component.pilerCount = component.isStellar ? AspConfig.ILS.StackCount.Value : AspConfig.PLS.StackCount.Value;
        }

        public static void AddDronesFromInventory(this StationComponent component, PrefabDesc prefabDesc)
        {
            if (AspConfig.General.EnableAutoReplenish.Value)
            {
                component.droneAutoReplenish = true;
                return;
            }
            var percentage = component.isStellar
                ? AspConfig.ILS.DroneInsertPercentage.Value
                : AspConfig.PLS.DroneInsertPercentage.Value;

            var maxToTake = Convert.ToInt32(Math.Floor(prefabDesc.stationMaxDroneCount * percentage));
            var numAvailable = GameMain.mainPlayer.package.TakeItem(ItemIds.Drone, maxToTake, out _);
            component.idleDroneCount = numAvailable;
            if (AspConfig.General.NotifyWhenDroneOrVesselMissing.Value && numAvailable < maxToTake)
            {
                UIRealtimeTip.PopupAhead("Not enough Logistics Drones in inventory!".Translate(),
                    AspConfig.General.PlaySoundWhenDroneOrVesselMissing.Value);
            }
        }

        public static void AddVesselsFromInventory(this StationComponent component, PrefabDesc prefabDesc)
        {
            if (!component.isStellar) return;
            if (AspConfig.General.EnableAutoReplenish.Value)
            {
                component.shipAutoReplenish = true;
                return;
            }
            var percentage = AspConfig.ILS.VesselInsertPercentage.Value;
            var maxToTake = Convert.ToInt32(Math.Floor(prefabDesc.stationMaxShipCount * percentage));
            var numAvailable = GameMain.mainPlayer.package.TakeItem(ItemIds.Vessel, maxToTake, out _);
            component.idleShipCount = numAvailable;
            if (AspConfig.General.NotifyWhenDroneOrVesselMissing.Value && numAvailable < maxToTake)
            {
                UIRealtimeTip.PopupAhead("Not enough Logistics Vessels in inventory!".Translate(),
                    AspConfig.General.PlaySoundWhenDroneOrVesselMissing.Value);
            }
        }

        public static void AddWarperRequestToLastSlot(
            this StationComponent component,
            PlanetTransport planetTransport
        )
        {
            if (!component.isStellar) return;
            if (!AspConfig.ILS.WarperInLastItemSlot.Value) return;
            if (component.HasItemInAnySlot(ItemIds.Warper)) return;

            planetTransport.SetStationStorage(
                component.id,
                component.storage.Length - 1,
                ItemIds.Warper,
                AspConfig.ILS.WarperDemand.Value * 100,
                AspConfig.ILS.WarperLocalMode.Value,
                AspConfig.ILS.WarperRemoteMode.Value,
                GameMain.mainPlayer
            );

            planetTransport.gameData.galacticTransport.RefreshTraffic(component.gid);
        }

        private static bool HasItemInAnySlot(this StationComponent component, int itemId) 
            => component.storage.Any(storage => storage.itemId == itemId);

        public static bool FixDuplicateWarperStores(this StationComponent component, int stationId, PlanetFactory factory)
        {
            if (!component.isStellar) return false;

            var duplicateStores = component.storage
                .Select((storage, idx) => new KeyValuePair<int, StationStore>(idx, storage))
                .Where(kv => kv.Value.itemId == ItemIds.Warper)
                .Skip(1)
                .ToArray();

            if (duplicateStores.Length < 1) return false;

            foreach (var kv in duplicateStores)
            {
                factory.transport.SetStationStorage(
                    stationId,
                    kv.Key,
                    0,
                    0,
                    ELogisticStorage.None,
                    ELogisticStorage.None,
                    factory.gameData.mainPlayer
                );
            }

            factory.transport.RefreshStationTraffic();
            return true;
        }

        public static void SetGatheringSpeed(this StationComponent component, PlanetFactory factory)
        {
            factory.factorySystem.minerPool[component.minerId].speed =
                AspConfig.AMM.GatheringSpeedInPercent.Value * 100;
        }
 
        public static void SetChargingPower(
            this DispenserComponent component,
            PlanetFactory factory,
            PrefabDesc prefabDesc
        )
        {
            var maxEnergy = prefabDesc.workEnergyPerTick * 5;
            var percent = AspConfig.LD.ChargingPowerInPercent.Value;

            var workPerTick = maxEnergy * percent / 100;
            factory.powerSystem.consumerPool[component.pcId].workEnergyPerTick =
                workPerTick / 5000 * 5000; /* align power to multiples of 5kW per tick */
        }

        public static void AddBotsFromInventory(this DispenserComponent component, PrefabDesc prefabDesc)
        {
            if (AspConfig.General.EnableAutoReplenish.Value)
            {
                component.courierAutoReplenish = true;
                return;
            }
            var maxToTake = Convert.ToInt32(Math.Floor(prefabDesc.dispenserMaxCourierCount * AspConfig.LD.BotInsertPercentage.Value));
            var numAvailable = GameMain.mainPlayer.package.TakeItem(ItemIds.Bot, maxToTake, out _);
            component.idleCourierCount = numAvailable;
            if (AspConfig.General.NotifyWhenDroneOrVesselMissing.Value && numAvailable < maxToTake)
            {
                UIRealtimeTip.PopupAhead("Not enough Logistics Drones in inventory!".Translate(),
                    AspConfig.General.PlaySoundWhenDroneOrVesselMissing.Value);
            }
        }

        public static void ApplyGuessFilter(this DispenserComponent component, PlanetFactory factory)
        {
            if (!AspConfig.LD.AlwaysGuessFilter.Value) return;
            /* store old filter for later use */
            var oldFilter = component.filter;
            /* try re-guess */
            component.filter = 0;
            component.GuessFilter(factory);
            /* revert to old filter if nothing is guessed out */
            if (component.filter == 0) component.filter = oldFilter;
        }
    }
}