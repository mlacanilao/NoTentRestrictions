using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace NoTentRestrictions;

internal static class TraitWrenchPatch
{
    private const int TentSoilElementId = 2200;
    private const int VanillaTentSoilUpgradeLimit = 10;

    internal static IEnumerable<CodeInstruction> IsValidTargetTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo? evalue = AccessTools.Method(
            type: typeof(Zone),
            name: nameof(Zone.Evalue),
            parameters: new[] { typeof(int) });
        MethodInfo? getTentSoilUpgradeLimit = AccessTools.Method(
            type: typeof(TraitWrenchPatch),
            name: nameof(GetTentSoilUpgradeLimit));

        if (evalue == null ||
            getTentSoilUpgradeLimit == null)
        {
            NoTentRestrictions.LogError(message: "TraitWrench.IsValidTarget transpiler: required method lookup failed");
            return instructions;
        }

        var codeMatcher = new CodeMatcher(instructions: instructions);

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(opcode: OpCodes.Ldc_I4, operand: TentSoilElementId),
            new CodeMatch(predicate: instruction => CallsMethod(instruction: instruction, method: evalue)),
            new CodeMatch(opcode: OpCodes.Ldc_I4_S, operand: (sbyte)VanillaTentSoilUpgradeLimit),
            new CodeMatch(opcode: OpCodes.Clt)
        });

        if (codeMatcher.IsValid == false)
        {
            NoTentRestrictions.LogError(message: "TraitWrench.IsValidTarget transpiler: tent soil limit pattern not matched");
            return codeMatcher.Instructions();
        }

        codeMatcher.Advance(offset: 2);
        codeMatcher.SetInstruction(instruction: new CodeInstruction(opcode: OpCodes.Call, operand: getTentSoilUpgradeLimit));

        return codeMatcher.Instructions();
    }

    private static int GetTentSoilUpgradeLimit()
    {
        if (NoTentRestrictionsConfig.EnableNoSoilUpgradeLimit.Value == false)
        {
            return VanillaTentSoilUpgradeLimit;
        }

        return int.MaxValue;
    }

    private static bool CallsMethod(CodeInstruction instruction, MethodInfo method)
    {
        return (instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt) &&
               Equals(objA: instruction.operand, objB: method);
    }
}
