namespace NoTentRestrictions.Patches
{
    public static class TraitShippingChestPatch
    {
        public static bool CanOpenContainerPrefix(TraitShippingChest __instance, ref bool __result)
        {
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