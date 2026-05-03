using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace NoTentRestrictions;

internal static class TraitTeleporterPatch
{
    internal static IEnumerable<CodeInstruction> TryTeleportTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo? getZoneGetter = AccessTools.PropertyGetter(
            type: typeof(EClass),
            name: "_zone");
        MethodInfo? preserveTeleportZoneInTent = AccessTools.Method(
            type: typeof(TraitTeleporterPatch),
            name: nameof(PreserveTeleportZoneInTent));

        if (getZoneGetter == null ||
            preserveTeleportZoneInTent == null)
        {
            NoTentRestrictions.LogError(message: "TraitTeleporter.TryTeleport transpiler: required method lookup failed");
            return instructions;
        }

        var codeMatcher = new CodeMatcher(instructions: instructions);

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(opcode: OpCodes.Isinst, operand: typeof(Zone_Tent)),
            new CodeMatch(predicate: instruction => instruction.opcode == OpCodes.Brtrue || instruction.opcode == OpCodes.Brtrue_S),
            new CodeMatch(predicate: instruction => CallsMethod(instruction: instruction, method: getZoneGetter)),
            new CodeMatch(opcode: OpCodes.Isinst, operand: typeof(Zone_Tent)),
            new CodeMatch(predicate: instruction => instruction.opcode == OpCodes.Brfalse || instruction.opcode == OpCodes.Brfalse_S),
            new CodeMatch(opcode: OpCodes.Ldnull),
            new CodeMatch(predicate: IsStloc)
        });

        if (codeMatcher.IsValid == false)
        {
            NoTentRestrictions.LogError(message: "TraitTeleporter.TryTeleport transpiler: tent zone null pattern not matched");
            return codeMatcher.Instructions();
        }

        codeMatcher.Advance(offset: 6);
        CodeInstruction storeZone = codeMatcher.Instruction;
        CodeInstruction? loadZone = CreateMatchingLdlocInstruction(instruction: storeZone);

        if (loadZone == null)
        {
            NoTentRestrictions.LogError(message: "TraitTeleporter.TryTeleport transpiler: matched store local could not be loaded");
            return codeMatcher.Instructions();
        }

        codeMatcher.Advance(offset: -1);
        // Preserve branch labels by mutating the matched ldnull instruction in place.
        codeMatcher.Instruction.opcode = loadZone.opcode;
        codeMatcher.Instruction.operand = loadZone.operand;
        codeMatcher.Advance(offset: 1);
        codeMatcher.Insert(instructions: new[]
        {
            new CodeInstruction(opcode: OpCodes.Call, operand: preserveTeleportZoneInTent)
        });

        return codeMatcher.Instructions();
    }

    private static Zone? PreserveTeleportZoneInTent(Zone? zone)
    {
        if (NoTentRestrictionsConfig.EnableTeleporter.Value == false)
        {
            return null;
        }

        return zone;
    }

    private static bool CallsMethod(CodeInstruction instruction, MethodInfo method)
    {
        return (instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt) &&
               Equals(objA: instruction.operand, objB: method);
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

    private static CodeInstruction? CreateMatchingLdlocInstruction(CodeInstruction instruction)
    {
        if (instruction.opcode == OpCodes.Stloc_0)
        {
            return new CodeInstruction(opcode: OpCodes.Ldloc_0);
        }

        if (instruction.opcode == OpCodes.Stloc_1)
        {
            return new CodeInstruction(opcode: OpCodes.Ldloc_1);
        }

        if (instruction.opcode == OpCodes.Stloc_2)
        {
            return new CodeInstruction(opcode: OpCodes.Ldloc_2);
        }

        if (instruction.opcode == OpCodes.Stloc_3)
        {
            return new CodeInstruction(opcode: OpCodes.Ldloc_3);
        }

        if (instruction.opcode == OpCodes.Stloc)
        {
            return new CodeInstruction(opcode: OpCodes.Ldloc, operand: instruction.operand);
        }

        if (instruction.opcode == OpCodes.Stloc_S)
        {
            return new CodeInstruction(opcode: OpCodes.Ldloc_S, operand: instruction.operand);
        }

        return null;
    }
}
