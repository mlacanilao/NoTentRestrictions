namespace NoTentRestrictions;

internal static class Zone_TentPatch
{
    private const int MaxTentSoil = 99999;

    internal static bool MaxSoilPrefix(ref int __result)
    {
        if (NoTentRestrictionsConfig.EnableMaxFertility.Value == false)
        {
            return true;
        }

        __result = MaxTentSoil;
        return false;
    }

    internal static bool AllowNewZonePrefix(ref bool __result)
    {
        if (NoTentRestrictionsConfig.EnablePlaceTent.Value == false)
        {
            return true;
        }

        __result = true;
        return false;
    }
}
