using NoTentRestrictions.Patches;
using HarmonyLib;

namespace NoTentRestrictions
{
    public class Patcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Zone_Tent), methodName: nameof(Zone_Tent.MaxSoil), methodType: MethodType.Getter)]
        public static bool Zone_TentMaxSoil(ref int __result)
        {
            return Zone_TentPatch.MaxSoilPrefix(__result: ref __result);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Zone_Tent), methodName: nameof(Zone_Tent.AllowNewZone), methodType: MethodType.Getter)]
        public static bool Zone_TentAllowNewZone(ref bool __result)
        {
            return Zone_TentPatch.AllowNewZonePrefix(ref __result);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(TraitTent), methodName: nameof(TraitTent.CanBeDropped), methodType: MethodType.Getter)]
        public static bool TraitTentCanBeDropped(ref bool __result)
        {
            return TraitTentPatch.CanBeDroppedPrefix(__result: ref __result);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(TraitDeliveryChest), methodName: nameof(TraitDeliveryChest.CanOpenContainer), methodType: MethodType.Getter)]
        public static bool TraitDeliveryChestCanOpenContainer(TraitDeliveryChest __instance, ref bool __result)
        {
            return TraitDeliveryChestPatch.CanOpenContainerPrefix(__instance: __instance, __result: ref __result);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(TraitMagicChest), methodName: nameof(TraitMagicChest.CanOpenContainer), methodType: MethodType.Getter)]
        public static bool TraitMagicChestCanOpenContainer(TraitMagicChest __instance, ref bool __result)
        {
            return TraitMagicChestPatch.CanOpenContainerPrefix(__instance: __instance, __result: ref __result);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(TraitShippingChest), methodName: nameof(TraitShippingChest.CanOpenContainer), methodType: MethodType.Getter)]
        public static bool TraitShippingChestCanOpenContainer(TraitShippingChest __instance, ref bool __result)
        {
            return TraitShippingChestPatch.CanOpenContainerPrefix(__instance: __instance, __result: ref __result);
        }
    }
}