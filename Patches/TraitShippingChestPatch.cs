namespace NoTentRestrictions.Patches
{
    public static class TraitShippingChestPatch
    {
        public static bool CanOpenContainerPrefix(TraitShippingChest __instance, ref bool __result)
        {
            bool enableShippingChest = NoTentRestrictionsConfig.EnableShippingChest?.Value ?? true;

            if (enableShippingChest == false)
            {
                return true;
            }
            
            if (EClass.core?.IsGameStarted == false ||
                EClass._zone is Zone_Tent == false ||
                __instance.owner?.IsInstalled == false)
            {
                return true;
            }
            
            __result = true;
            return false;
        }
    }
}