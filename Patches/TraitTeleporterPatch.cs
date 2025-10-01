using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace NoTentRestrictions.Patches
{
    public class TraitTeleporterPatch
    {
        internal static IEnumerable<CodeInstruction> TryTeleportTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            bool enableTeleporter = NoTentRestrictionsConfig.EnableTeleporter?.Value ?? false;
            
            if (enableTeleporter == false)
            {
                return instructions;
            }
            
            var getZoneGetter = typeof(EClass).GetMethod(name: "get__zone", bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            
            var codeMatcher = new CodeMatcher(instructions: instructions);
            
            codeMatcher.MatchStartForward(matches: new[]
            {
                new CodeMatch(opcode: OpCodes.Isinst,   operand: typeof(Zone_Tent)),
                new CodeMatch(predicate: i => i.opcode == OpCodes.Brtrue  || i.opcode == OpCodes.Brtrue_S),
                new CodeMatch(opcode: OpCodes.Call,    operand: getZoneGetter),
                new CodeMatch(opcode: OpCodes.Isinst,   operand: typeof(Zone_Tent)),
                new CodeMatch(predicate: i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S),
                new CodeMatch(opcode: OpCodes.Ldnull),
                new CodeMatch(predicate: i => IsStloc(i: i))
            });
            
            
            if (codeMatcher.IsValid)
            {
                // Move to ldnull and NOP it
                codeMatcher.Advance(offset: 5);
                codeMatcher.Set(opcode: OpCodes.Nop, operand: null);

                // Move to stloc.1 and NOP it
                codeMatcher.Advance(offset: 1);
                codeMatcher.Set(opcode: OpCodes.Nop, operand: null);
            }
            
            return codeMatcher.Instructions();
            
            bool IsStloc(CodeInstruction i)
                => i.opcode == OpCodes.Stloc_0 || i.opcode == OpCodes.Stloc_1 ||
                   i.opcode == OpCodes.Stloc_2 || i.opcode == OpCodes.Stloc_3 ||
                   i.opcode == OpCodes.Stloc || i.opcode == OpCodes.Stloc_S;
        }
    }
}