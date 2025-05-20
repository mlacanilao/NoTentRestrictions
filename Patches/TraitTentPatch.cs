namespace NoTentRestrictions.Patches
{
    public static class TraitTentPatch
    {
        public static bool CanBeDroppedPrefix(ref bool __result)
        {
            bool enablePlaceTent = NoTentRestrictionsConfig.EnablePlaceTent?.Value ?? true;
            
            if (enablePlaceTent == false)
            {
                return true;
            }
            
            __result = true;
            return false;
        }
    }
}