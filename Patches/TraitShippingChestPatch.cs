namespace NoTentRestrictions;

internal static class TraitShippingChestPatch
{
    internal static bool CanOpenContainerPrefix(TraitShippingChest __instance, ref bool __result)
    {
        if (ShouldAllowInstalledShippingChestInTent(__instance: __instance) == false)
        {
            return true;
        }

        __result = true;
        return false;
    }

    private static bool ShouldAllowInstalledShippingChestInTent(TraitShippingChest __instance)
    {
        return TentContainerAccessUtility.ShouldAllowInstalledContainerInTent(
            containerOwner: __instance.owner,
            isEnabled: NoTentRestrictionsConfig.EnableShippingChest.Value);
    }
}
