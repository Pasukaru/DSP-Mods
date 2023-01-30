using HarmonyLib;

namespace Pasukaru.DSP.AutoStationConfig
{
    [HarmonyPatch(typeof(PlanetTransport))]
    public class PlanetTransportPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("NewStationComponent")]
        public static void NewStationComponent(int _entityId, int _pcId, PrefabDesc _desc, PlanetTransport __instance, StationComponent __result)
        {
            var component = __result;
            if (component.isCollector || component.isVeinCollector) return;

            var planetTransport = __instance;

            component.SetChargingPower(planetTransport, _desc);
            component.SetTransportRanges();
            component.SetTransportLoads();
            component.AddDronesFromInventory(_desc);

            // Extra configuration if ILS.
            if (!component.isStellar) return;
            component.SetToggles();
            component.SetMinWarpDistance();
            component.AddVesselsFromInventory(_desc);
            component.AddWarperRequestToLastSlot(planetTransport);
        }
    }
}