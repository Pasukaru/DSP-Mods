using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace Pasukaru.DSP.AutoStationConfig
{
    [HarmonyPatch(typeof(BuildingParameters))]
    public class BuildingParametersPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ApplyPrebuildParametersToEntity")]
        public static void ApplyPrebuildParametersToEntity(
            int entityId,
            int recipeId,
            int filterId,
            int[] parameters,
            PlanetFactory factory
        ){
            var entityPool = factory.entityPool;
            var stationId = entityPool[entityId].stationId;
            if (stationId == 0) return;
            
            var component = factory.transport.stationPool[stationId];
            if (component == null || !component.isStellar) return;

            var duplicateStores = component.storage
                .Select((storage, idx) => new KeyValuePair<int, StationStore>(idx, storage))
                .Where(kv => kv.Value.itemId == ItemIds.Warper)
                .Skip(1)
                .ToArray();

            if (duplicateStores.Length < 1) return;

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

            factory.transport.RefreshTraffic();
        }
    }

}