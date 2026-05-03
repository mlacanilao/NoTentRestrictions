namespace NoTentRestrictions;

internal static class ZonePatch
{
    private const int MaxTentElectricity = 99999;

    internal static bool BaseElectricityPrefix(Zone __instance, ref int __result)
    {
        if (NoTentRestrictionsConfig.EnableMaxElectricity.Value == false)
        {
            return true;
        }

        if (EClass.core?.IsGameStarted == false ||
            __instance is Zone_Tent == false)
        {
            return true;
        }

        __result = MaxTentElectricity;
        return false;
    }

    internal static bool AllowInvestPrefix(Zone __instance, ref bool __result)
    {
        if (EClass.core?.IsGameStarted == false ||
            __instance is Zone_Tent == false)
        {
            return true;
        }

        __result = true;
        return false;
    }

    internal static bool ShouldAutoRevivePrefix(Zone __instance, ref bool __result)
    {
        if (EClass.core?.IsGameStarted == false ||
            __instance is Zone_Tent == false)
        {
            return true;
        }

        __result = true;
        return false;
    }
}
