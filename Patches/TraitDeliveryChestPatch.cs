namespace NoTentRestrictions;

internal static class TraitDeliveryChestPatch
{
    internal static bool CanOpenContainerPrefix(TraitDeliveryChest __instance, ref bool __result)
    {
        if (ShouldAllowInstalledDeliveryBoxInTent(__instance: __instance) == false)
        {
            return true;
        }

        __result = true;
        return false;
    }

    private static bool ShouldAllowInstalledDeliveryBoxInTent(TraitDeliveryChest __instance)
    {
        return TentContainerAccessUtility.ShouldAllowInstalledContainerInTent(
            containerOwner: __instance.owner,
            isEnabled: NoTentRestrictionsConfig.EnableDeliveryBox.Value);
    }
}
