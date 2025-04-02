namespace NoTentRestrictions.Patches
{
    public static class SpatialPatch
    {
        public static bool isExternalZonePrefix(Spatial __instance, ref bool __result)
        {
            if (__instance is Zone_Tent == false)
            {
                return true;
            }
            
            __result = false;
            return false;
        }
    }
}