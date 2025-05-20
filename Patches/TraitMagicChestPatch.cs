namespace NoTentRestrictions.Patches
{
    public static class TraitMagicChestPatch
    {
        public static bool CanOpenContainerPrefix(TraitMagicChest __instance, ref bool __result)
        {
            bool enableStorageChest = NoTentRestrictionsConfig.EnableStorageChest?.Value ?? true;

            if (enableStorageChest == false)
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
        
        public static bool CanSearchContentPrefix(TraitMagicChest __instance, ref bool __result)
        {
            bool enableStorageChest = NoTentRestrictionsConfig.EnableStorageChest?.Value ?? true;

            if (enableStorageChest == false)
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