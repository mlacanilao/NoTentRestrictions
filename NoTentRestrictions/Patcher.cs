using System.Collections.Generic;
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
        
        [HarmonyTranspiler]
        [HarmonyPatch(declaringType: typeof(TraitWrench), methodName: nameof(TraitWrench.IsValidTarget))]
        internal static IEnumerable<CodeInstruction> TraitWrenchIsValidTarget(IEnumerable<CodeInstruction> instructions)
        {
            return TraitWrenchPatch.IsValidTargetTranspiler(instructions: instructions);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Zone), methodName: nameof(Zone.BaseElectricity), methodType: MethodType.Getter)]
        public static bool ZoneBaseElectricity(ref int __result)
        {
            return ZonePatch.BaseElectricityPrefix(__result: ref __result);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Zone), methodName: nameof(Zone.AllowInvest), methodType: MethodType.Getter)]
        public static bool ZoneAllowInvest(ref bool __result)
        {
            return ZonePatch.AllowInvestPrefix(__result: ref __result);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Zone), methodName: nameof(Zone.ShouldAutoRevive), methodType: MethodType.Getter)]
        public static bool ZoneShouldAutoRevive(ref bool __result)
        {
            return ZonePatch.ShouldAutoRevivePrefix(__result: ref __result);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Zone_Tent), methodName: nameof(Zone_Tent.AllowNewZone), methodType: MethodType.Getter)]
        public static bool Zone_TentAllowNewZone(ref bool __result)
        {
            return Zone_TentPatch.AllowNewZonePrefix(__result: ref __result);
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
        [HarmonyPatch(declaringType: typeof(TraitMagicChest), methodName: nameof(TraitMagicChest.CanSearchContent), methodType: MethodType.Getter)]
        public static bool TraitMagicChestCanSearchContent(TraitMagicChest __instance, ref bool __result)
        {
            return TraitMagicChestPatch.CanSearchContentPrefix(__instance: __instance, __result: ref __result);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(TraitShippingChest), methodName: nameof(TraitShippingChest.CanOpenContainer), methodType: MethodType.Getter)]
        public static bool TraitShippingChestCanOpenContainer(TraitShippingChest __instance, ref bool __result)
        {
            return TraitShippingChestPatch.CanOpenContainerPrefix(__instance: __instance, __result: ref __result);
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(declaringType: typeof(HotItemHeld), methodName: nameof(HotItemHeld.TrySetAct))]
        internal static IEnumerable<CodeInstruction> HotItemHeldTrySetActTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            return HotItemHeldPatch.TrySetActTranspiler(instructions: instructions);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Spatial), methodName: nameof(Spatial.isExternalZone), methodType: MethodType.Getter)]
        public static bool SpatialisExternalZone(Spatial __instance, ref bool __result)
        {
            return SpatialPatch.isExternalZonePrefix(__instance: __instance, __result: ref __result);
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(declaringType: typeof(TraitTeleporter), methodName: nameof(TraitTeleporter.TryTeleport))]
        internal static IEnumerable<CodeInstruction> TraitTeleporterTryTeleport(IEnumerable<CodeInstruction> instructions)
        {
            return TraitTeleporterPatch.TryTeleportTranspiler(instructions: instructions);
        }
    }
}