using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace NoTentRestrictions;

internal static class FoodEffectPatch
{
    internal static IEnumerable<CodeInstruction> ProcTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo? getZoneGetter = AccessTools.PropertyGetter(
            type: typeof(EClass),
            name: "_zone");
        MethodInfo? isPcFactionGetter = AccessTools.PropertyGetter(
            type: typeof(Zone),
            name: nameof(Zone.IsPCFaction));
        MethodInfo? allowsDiningSpotBonusInZone = AccessTools.Method(
            type: typeof(FoodEffectPatch),
            name: nameof(AllowsDiningSpotBonusInZone));

        if (getZoneGetter == null ||
            isPcFactionGetter == null ||
            allowsDiningSpotBonusInZone == null)
        {
            NoTentRestrictions.LogError(message: "FoodEffect.Proc transpiler: required method lookup failed");
            return instructions;
        }

        var codeMatcher = new CodeMatcher(instructions: instructions);

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(predicate: instruction => CallsMethod(instruction: instruction, method: getZoneGetter)),
            new CodeMatch(predicate: instruction => CallsMethod(instruction: instruction, method: isPcFactionGetter)),
            new CodeMatch(predicate: instruction => instruction.opcode == OpCodes.Brfalse || instruction.opcode == OpCodes.Brfalse_S)
        });

        if (codeMatcher.IsValid == false)
        {
            NoTentRestrictions.LogError(message: "FoodEffect.Proc transpiler: dining spot faction pattern not matched");
            return codeMatcher.Instructions();
        }

        codeMatcher.Advance(offset: 1);
        codeMatcher.Instruction.opcode = OpCodes.Call;
        codeMatcher.Instruction.operand = allowsDiningSpotBonusInZone;

        return codeMatcher.Instructions();
    }

    private static bool AllowsDiningSpotBonusInZone(Zone zone)
    {
        if (zone == null)
        {
            return false;
        }

        if (zone.IsPCFaction == true)
        {
            return true;
        }

        if (NoTentRestrictionsConfig.EnableDiningSpotSign.Value == false)
        {
            return false;
        }

        if (zone is Zone_Tent == false)
        {
            return false;
        }

        return true;
    }

    private static bool CallsMethod(CodeInstruction instruction, MethodInfo method)
    {
        return (instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt) &&
               Equals(objA: instruction.operand, objB: method);
    }
}
