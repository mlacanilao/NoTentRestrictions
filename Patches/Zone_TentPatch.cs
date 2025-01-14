namespace NoTentRestrictions.Patches
{
    public static class Zone_TentPatch
    {
        public static bool MaxSoilPrefix(ref int __result)
        {
            if (EClass.core?.IsGameStarted == false ||
                EClass._zone is Zone_Tent == false)
            {
                return true;
            }

            __result = 99999;
            return false;
        }
        
        public static bool AllowNewZonePrefix(ref bool __result)
        {
            if (EClass.core?.IsGameStarted == false ||
                EClass._zone is Zone_Tent == false)
            {
                return true;
            }
            
            __result = true;
            return false;
        }
    }
}