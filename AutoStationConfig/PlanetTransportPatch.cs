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
}