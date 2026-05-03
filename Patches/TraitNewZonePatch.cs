using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace NoTentRestrictions;

internal static class TraitNewZonePatch
{
    internal static IEnumerable<CodeInstruction> MoveZoneTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo? getZoneGetter = AccessTools.PropertyGetter(
            type: typeof(EClass),
            name: "_zone");
        MethodInfo? isExternalZoneGetter = AccessTools.PropertyGetter(
            type: typeof(Spatial),
            name: nameof(Spatial.isExternalZone));
        MethodInfo? shouldTreatCurrentZoneAsExternalForNewZone = AccessTools.Method(
            type: typeof(TraitNewZonePatch),
            name: nameof(ShouldTreatCurrentZoneAsExternalForNewZone));

        if (getZoneGetter == null ||
            isExternalZoneGetter == null ||
            shouldTreatCurrentZoneAsExternalForNewZone == null)
        {
            NoTentRestrictions.LogError(message: "TraitNewZone.MoveZone transpiler: required method lookup failed");
            return instructions;
        }

        var codeMatcher = new CodeMatcher(instructions: instructions);

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(predicate: instruction => CallsMethod(instruction: instruction, method: getZoneGetter)),
            new CodeMatch(predicate: instruction => CallsMethod(instruction: instruction, method: isExternalZoneGetter))
        });

        if (codeMatcher.IsValid == false)
        {
            NoTentRestrictions.LogError(message: "TraitNewZone.MoveZone transpiler: external-zone check not matched");
            return codeMatcher.Instructions();
        }

        codeMatcher.SetInstruction(instruction: new CodeInstruction(opcode: OpCodes.Ldarg_0));
        codeMatcher.Advance(offset: 1);
        codeMatcher.SetInstruction(instruction: new CodeInstruction(opcode: OpCodes.Call, operand: shouldTreatCurrentZoneAsExternalForNewZone));

        return codeMatcher.Instructions();
    }

    private static bool ShouldTreatCurrentZoneAsExternalForNewZone(TraitNewZone trait)
    {
        Zone zone = EClass._zone;
        if (zone == null)
        {
            return true;
        }

        if (trait is TraitTent == false)
        {
            return zone.isExternalZone;
        }

        if (zone is Zone_Tent == false)
        {
            return zone.isExternalZone;
        }

        if (NoTentRestrictionsConfig.EnablePlaceTent.Value == false)
        {
            return zone.isExternalZone;
        }

        return false;
    }

    private static bool CallsMethod(CodeInstruction instruction, MethodInfo method)
    {
        return (instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt) &&
               Equals(objA: instruction.operand, objB: method);
    }
}
