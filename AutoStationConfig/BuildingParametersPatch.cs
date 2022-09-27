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
            if (stationId != 0)
            {
                var stationComponent = factory.transport.stationPool[stationId];
                if (stationComponent is null)
                    return;
                if (stationComponent.isCollector) return;
                /* Parameters[320] is power consumption, which is set to non-zero while copy-building */
                if (parameters == null || parameters[320] == 0)
                {
                    stationComponent.SetStackCount();
                    if (stationComponent.isVeinCollector)
                    {
                        stationComponent.SetGatheringSpeed(factory);
                    }
                }
                if (!stationComponent.isVeinCollector)
                {
                    stationComponent.FixDuplicateWarperStores(stationId, factory);
                }
                return;
            }

            var dispenserId = entityPool[entityId].dispenserId;
            if (dispenserId == 0) return;
            var dispenserComponent = factory.transport.dispenserPool[dispenserId];
            if (dispenserComponent == null) return;
            /* Don't deal with Dispensers with item filter set, which indicates a copy-build */
            dispenserComponent.ApplyGuessFilter(factory);
            if (filterId != 0) return;
            var itemProto = LDB.items.Select(entityPool[entityId].protoId);
            /* Check if any parameter is not the default value, which indicates a copy-build */
            if (parameters == null ||
                (parameters[0] == 2 && parameters[1] == 0 && parameters[2] == 30000 && parameters[3] == 0))
            {
                dispenserComponent.SetChargingPower(factory, itemProto.prefabDesc);
            }
            dispenserComponent.AddBotsFromInventory(itemProto.prefabDesc);
        }
    }

}