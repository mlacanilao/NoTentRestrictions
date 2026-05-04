using System.Diagnostics.CodeAnalysis;

namespace NoTentRestrictions;

internal static class ThingPatch
{
    internal static void SelfWeightPostfix(Thing __instance, ref int __result)
    {
        if (ShouldMakeTentWeightless(thing: __instance) == false)
        {
            return;
        }

        __result = 0;
    }

    internal static void RefreshWeightlessTentWeightCache()
    {
        if (TryGetPlayerChara(pc: out Chara? pc) == false)
        {
            return;
        }

        pc.SetDirtyWeight();
        LayerInventory.SetDirtyAll();
    }

    private static bool ShouldMakeTentWeightless(Thing thing)
    {
        if (thing.trait is TraitTent == false)
        {
            return false;
        }

        if (NoTentRestrictionsConfig.EnableWeightlessTents.Value == false)
        {
            return false;
        }

        if (TryGetPlayerChara(pc: out Chara? pc) == false)
        {
            return false;
        }

        if (thing.GetRootCard() != pc)
        {
            return false;
        }

        return true;
    }

    private static bool TryGetPlayerChara([NotNullWhen(returnValue: true)] out Chara? pc)
    {
        pc = null;

        if (EClass.core == null ||
            EClass.core.IsGameStarted == false)
        {
            return false;
        }

        pc = EClass.pc;
        if (pc == null)
        {
            return false;
        }

        return true;
    }
}
