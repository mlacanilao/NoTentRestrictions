namespace NoTentRestrictions.Patches
{
    public static class TraitTentPatch
    {
        public static bool CanBeDroppedPrefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}