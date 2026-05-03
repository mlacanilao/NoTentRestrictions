using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace NoTentRestrictions;

internal static class HotItemHeldPatch
{
    internal static IEnumerable<CodeInstruction> TrySetActTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo? getZoneGetter = AccessTools.PropertyGetter(
            type: typeof(EClass),
            name: "_zone");
        MethodInfo? shouldAllowTentBuildActionInTent = AccessTools.Method(
            type: typeof(HotItemHeldPatch),
            name: nameof(ShouldAllowTentBuildActionInTent));

        if (getZoneGetter == null ||
            shouldAllowTentBuildActionInTent == null)
        {
            NoTentRestrictions.LogError(message: "HotItemHeld.TrySetAct transpiler: required method lookup failed");
            return instructions;
        }

        var codeMatcher = new CodeMatcher(instructions: instructions);

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(opcode: OpCodes.Isinst, operand: typeof(TraitTent)),
            new CodeMatch(predicate: instruction => instruction.opcode == OpCodes.Brfalse || instruction.opcode == OpCodes.Brfalse_S),
            new CodeMatch(opcode: OpCodes.Call, operand: getZoneGetter),
            new CodeMatch(opcode: OpCodes.Isinst, operand: typeof(Zone_Tent)),
            new CodeMatch(predicate: instruction => instruction.opcode == OpCodes.Brfalse || instruction.opcode == OpCodes.Brfalse_S),
            new CodeMatch(opcode: OpCodes.Ldc_I4_0),
            new CodeMatch(predicate: IsStloc),
            new CodeMatch(predicate: IsLdloc),
            new CodeMatch(predicate: instruction => instruction.opcode == OpCodes.Brfalse || instruction.opcode == OpCodes.Brfalse_S)
        });

        if (codeMatcher.IsValid == false)
        {
            NoTentRestrictions.LogError(message: "HotItemHeld.TrySetAct transpiler: tent build flag pattern not matched");
            return codeMatcher.Instructions();
        }

        codeMatcher.Advance(offset: 5);
        codeMatcher.SetInstruction(instruction: new CodeInstruction(opcode: OpCodes.Call, operand: shouldAllowTentBuildActionInTent));

        return codeMatcher.Instructions();
    }

    private static bool ShouldAllowTentBuildActionInTent()
    {
        if (NoTentRestrictionsConfig.EnablePlaceTent.Value == false)
        {
            return false;
        }

        return true;
    }

    private static bool IsStloc(CodeInstruction instruction)
    {
        return instruction.opcode == OpCodes.Stloc_0 ||
               instruction.opcode == OpCodes.Stloc_1 ||
               instruction.opcode == OpCodes.Stloc_2 ||
               instruction.opcode == OpCodes.Stloc_3 ||
               instruction.opcode == OpCodes.Stloc ||
               instruction.opcode == OpCodes.Stloc_S;
    }

    private static bool IsLdloc(CodeInstruction instruction)
    {
        return instruction.opcode == OpCodes.Ldloc_0 ||
               instruction.opcode == OpCodes.Ldloc_1 ||
               instruction.opcode == OpCodes.Ldloc_2 ||
               instruction.opcode == OpCodes.Ldloc_3 ||
               instruction.opcode == OpCodes.Ldloc ||
               instruction.opcode == OpCodes.Ldloc_S;
    }
}
