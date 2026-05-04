using HarmonyLib;
using System.Collections.Generic;

namespace NoTentRestrictions;

internal static class Patcher
{
    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(Zone_Tent), methodName: nameof(Zone_Tent.MaxSoil), methodType: MethodType.Getter)]
    internal static bool Zone_TentMaxSoil(ref int __result)
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
    internal static bool ZoneBaseElectricity(Zone __instance, ref int __result)
    {
        return ZonePatch.BaseElectricityPrefix(__instance: __instance, __result: ref __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(Zone), methodName: nameof(Zone.AllowInvest), methodType: MethodType.Getter)]
    internal static bool ZoneAllowInvest(Zone __instance, ref bool __result)
    {
        return ZonePatch.AllowInvestPrefix(__instance: __instance, __result: ref __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(Zone), methodName: nameof(Zone.ShouldAutoRevive), methodType: MethodType.Getter)]
    internal static bool ZoneShouldAutoRevive(Zone __instance, ref bool __result)
    {
        return ZonePatch.ShouldAutoRevivePrefix(__instance: __instance, __result: ref __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(Zone_Tent), methodName: nameof(Zone_Tent.AllowNewZone), methodType: MethodType.Getter)]
    internal static bool Zone_TentAllowNewZone(ref bool __result)
    {
        return Zone_TentPatch.AllowNewZonePrefix(__result: ref __result);
    }

    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(TraitTent), methodName: nameof(TraitTent.CanBeDropped), methodType: MethodType.Getter)]
    internal static void TraitTentCanBeDropped(ref bool __result)
    {
        TraitTentPatch.CanBeDroppedPostfix(__result: ref __result);
    }

    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(Thing), methodName: nameof(Thing.SelfWeight), methodType: MethodType.Getter)]
    internal static void ThingSelfWeight(Thing __instance, ref int __result)
    {
        ThingPatch.SelfWeightPostfix(__instance: __instance, __result: ref __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(TraitDeliveryChest), methodName: nameof(TraitDeliveryChest.CanOpenContainer), methodType: MethodType.Getter)]
    internal static bool TraitDeliveryChestCanOpenContainer(TraitDeliveryChest __instance, ref bool __result)
    {
        return TraitDeliveryChestPatch.CanOpenContainerPrefix(__instance: __instance, __result: ref __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(TraitMagicChest), methodName: nameof(TraitMagicChest.CanOpenContainer), methodType: MethodType.Getter)]
    internal static bool TraitMagicChestCanOpenContainer(TraitMagicChest __instance, ref bool __result)
    {
        return TraitMagicChestPatch.CanOpenContainerPrefix(__instance: __instance, __result: ref __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(TraitMagicChest), methodName: nameof(TraitMagicChest.CanSearchContent), methodType: MethodType.Getter)]
    internal static bool TraitMagicChestCanSearchContent(TraitMagicChest __instance, ref bool __result)
    {
        return TraitMagicChestPatch.CanSearchContentPrefix(__instance: __instance, __result: ref __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(TraitShippingChest), methodName: nameof(TraitShippingChest.CanOpenContainer), methodType: MethodType.Getter)]
    internal static bool TraitShippingChestCanOpenContainer(TraitShippingChest __instance, ref bool __result)
    {
        return TraitShippingChestPatch.CanOpenContainerPrefix(__instance: __instance, __result: ref __result);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(declaringType: typeof(HotItemHeld), methodName: nameof(HotItemHeld.TrySetAct))]
    internal static IEnumerable<CodeInstruction> HotItemHeldTrySetActTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return HotItemHeldPatch.TrySetActTranspiler(instructions: instructions);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(declaringType: typeof(TraitTeleporter), methodName: nameof(TraitTeleporter.TryTeleport))]
    internal static IEnumerable<CodeInstruction> TraitTeleporterTryTeleport(IEnumerable<CodeInstruction> instructions)
    {
        return TraitTeleporterPatch.TryTeleportTranspiler(instructions: instructions);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(declaringType: typeof(FoodEffect), methodName: nameof(FoodEffect.Proc))]
    internal static IEnumerable<CodeInstruction> FoodEffectProc(IEnumerable<CodeInstruction> instructions)
    {
        return FoodEffectPatch.ProcTranspiler(instructions: instructions);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(declaringType: typeof(TraitNewZone), methodName: nameof(TraitNewZone.MoveZone))]
    internal static IEnumerable<CodeInstruction> TraitNewZoneMoveZoneTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return TraitNewZonePatch.MoveZoneTranspiler(instructions: instructions);
    }
}
