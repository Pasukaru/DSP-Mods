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
            component?.FixDuplicateWarperStores(stationId, factory);
        }
    }

}