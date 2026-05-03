using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
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
        MethodInfo? isPcFactionOrTentGetter = AccessTools.PropertyGetter(
            type: typeof(Zone),
            name: nameof(Zone.IsPCFactionOrTent));
        MethodInfo? allowsDiningSpotBonusInZone = AccessTools.Method(
            type: typeof(FoodEffectPatch),
            name: nameof(AllowsDiningSpotBonusInZone));

        LogTranspilerLookupResult(
            getZoneGetter: getZoneGetter,
            isPcFactionGetter: isPcFactionGetter,
            isPcFactionOrTentGetter: isPcFactionOrTentGetter,
            allowsDiningSpotBonusInZone: allowsDiningSpotBonusInZone);

        if (getZoneGetter == null ||
            (isPcFactionGetter == null && isPcFactionOrTentGetter == null) ||
            allowsDiningSpotBonusInZone == null)
        {
            NoTentRestrictions.LogError(message: "FoodEffect.Proc transpiler: required method lookup failed");
            return instructions;
        }

        var codeMatcher = new CodeMatcher(instructions: instructions);

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(predicate: instruction => CallsMethod(instruction: instruction, method: getZoneGetter)),
            new CodeMatch(predicate: instruction => CallsEitherMethod(
                instruction: instruction,
                firstMethod: isPcFactionGetter,
                secondMethod: isPcFactionOrTentGetter)),
            new CodeMatch(predicate: instruction => instruction.opcode == OpCodes.Brfalse || instruction.opcode == OpCodes.Brfalse_S)
        });

        if (codeMatcher.IsValid == false)
        {
            NoTentRestrictions.LogError(message: "FoodEffect.Proc transpiler: dining spot faction pattern not matched");
            LogTranspilerInstructions(instructions: codeMatcher.Instructions());
            return codeMatcher.Instructions();
        }

        NoTentRestrictions.LogDebug(message: $"FoodEffect.Proc transpiler: dining spot faction pattern matched at IL index {codeMatcher.Pos}");
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

    private static bool CallsEitherMethod(
        CodeInstruction instruction,
        MethodInfo? firstMethod,
        MethodInfo? secondMethod)
    {
        if (firstMethod != null &&
            CallsMethod(instruction: instruction, method: firstMethod))
        {
            return true;
        }

        if (secondMethod != null &&
            CallsMethod(instruction: instruction, method: secondMethod))
        {
            return true;
        }

        return false;
    }

    private static void LogTranspilerLookupResult(
        MethodInfo? getZoneGetter,
        MethodInfo? isPcFactionGetter,
        MethodInfo? isPcFactionOrTentGetter,
        MethodInfo? allowsDiningSpotBonusInZone)
    {
        NoTentRestrictions.LogDebug(
            message: "FoodEffect.Proc transpiler lookups: " +
                     $"{nameof(EClass._zone)}={(getZoneGetter != null ? getZoneGetter.FullDescription() : "missing")}, " +
                     $"{nameof(Zone.IsPCFaction)}={(isPcFactionGetter != null ? isPcFactionGetter.FullDescription() : "missing")}, " +
                     $"{nameof(Zone.IsPCFactionOrTent)}={(isPcFactionOrTentGetter != null ? isPcFactionOrTentGetter.FullDescription() : "missing")}, " +
                     $"{nameof(AllowsDiningSpotBonusInZone)}={(allowsDiningSpotBonusInZone != null ? allowsDiningSpotBonusInZone.FullDescription() : "missing")}");
    }

    private static void LogTranspilerInstructions(IReadOnlyList<CodeInstruction> instructions)
    {
        var messageBuilder = new StringBuilder(value: "FoodEffect.Proc transpiler IL scan:");

        for (int i = 0; i < instructions.Count; i++)
        {
            CodeInstruction instruction = instructions[index: i];

            if (IsDiagnosticInstruction(instruction: instruction) == false)
            {
                continue;
            }

            messageBuilder.AppendLine();
            messageBuilder.Append(value: i);
            messageBuilder.Append(value: ": ");
            messageBuilder.Append(value: instruction.opcode);

            if (instruction.operand != null)
            {
                messageBuilder.Append(value: " ");
                messageBuilder.Append(value: instruction.operand);
            }
        }

        NoTentRestrictions.LogInfo(message: messageBuilder.ToString());
    }

    private static bool IsDiagnosticInstruction(CodeInstruction instruction)
    {
        if (instruction.opcode == OpCodes.Call ||
            instruction.opcode == OpCodes.Callvirt ||
            instruction.opcode == OpCodes.Brfalse ||
            instruction.opcode == OpCodes.Brfalse_S ||
            instruction.opcode == OpCodes.Brtrue ||
            instruction.opcode == OpCodes.Brtrue_S)
        {
            return true;
        }

        return false;
    }
}
