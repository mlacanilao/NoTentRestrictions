namespace NoTentRestrictions;

internal static class TraitMagicChestPatch
{
    internal static bool CanOpenContainerPrefix(TraitMagicChest __instance, ref bool __result)
    {
        if (ShouldAllowInstalledMagicChestInTent(__instance: __instance) == false)
        {
            return true;
        }

        __result = true;
        return false;
    }

    internal static bool CanSearchContentPrefix(TraitMagicChest __instance, ref bool __result)
    {
        if (ShouldAllowInstalledMagicChestInTent(__instance: __instance) == false)
        {
            return true;
        }

        __result = true;
        return false;
    }

    private static bool ShouldAllowInstalledMagicChestInTent(TraitMagicChest __instance)
    {
        return TentContainerAccessUtility.ShouldAllowInstalledContainerInTent(
            containerOwner: __instance.owner,
            isEnabled: NoTentRestrictionsConfig.EnableStorageChest.Value);
    }
}
