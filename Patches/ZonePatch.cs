namespace NoTentRestrictions.Patches
{
    public static class ZonePatch
    {
        public static bool BaseElectricityPrefix(ref int __result)
        {
            bool enableMaxElectricity = NoTentRestrictionsConfig.EnableMaxElectricity?.Value ?? true;
            
            if (enableMaxElectricity == false)
            {
                return true;
            }
            
            if (EClass.core?.IsGameStarted == false ||
                EClass._zone is Zone_Tent == false)
            {
                return true;
            }

            __result = 99999;
            return false;
        }
    }
}