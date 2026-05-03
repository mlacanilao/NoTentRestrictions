namespace NoTentRestrictions;

internal static class TentContainerAccessUtility
{
    internal static bool ShouldAllowInstalledContainerInTent(Card containerOwner, bool isEnabled)
    {
        if (isEnabled == false)
        {
            return false;
        }

        if (EClass.core?.IsGameStarted == false)
        {
            return false;
        }

        if (EClass._zone is Zone_Tent == false)
        {
            return false;
        }

        if (containerOwner?.IsInstalled != true)
        {
            return false;
        }

        return true;
    }
}
