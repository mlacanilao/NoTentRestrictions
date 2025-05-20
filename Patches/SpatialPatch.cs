namespace NoTentRestrictions.Patches
{
    public static class SpatialPatch
    {
        public static bool isExternalZonePrefix(Spatial __instance, ref bool __result)
        {
            bool enablePlaceTent = NoTentRestrictionsConfig.EnablePlaceTent?.Value ?? true;

            if (enablePlaceTent == false)
            {
                return true;
            }
            
            if (__instance is Zone_Tent == false)
            {
                return true;
            }
            
            __result = false;
            return false;
        }
    }
}