namespace NoTentRestrictions;

internal static class TraitTentPatch
{
    internal static void CanBeDroppedPostfix(ref bool __result)
    {
        if (__result == true)
        {
            return;
        }

        if (NoTentRestrictionsConfig.EnablePlaceTent.Value == false)
        {
            return;
        }

        if (EClass._zone is Zone_Tent == false)
        {
            return;
        }

        __result = true;
    }
}
