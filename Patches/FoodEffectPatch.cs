using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace NoTentRestrictions.Patches
{
    public class FoodEffectPatch
    {
        internal static IEnumerable<CodeInstruction> ProcTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            bool enableDiningSpotSign = NoTentRestrictionsConfig.EnableDiningSpotSign?.Value ?? false;
            
            if (enableDiningSpotSign == false)
            {
                return instructions;
            }
            
            var codeMatcher = new CodeMatcher(instructions: instructions);
            
            codeMatcher.MatchStartForward(matches: new[]
            {
                new CodeMatch(opcode: OpCodes.Call, operand: typeof(EClass).GetMethod(name: "get__zone", bindingAttr: BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)),
                new CodeMatch(opcode: OpCodes.Callvirt, operand: typeof(Zone).GetProperty(name: "IsPCFaction").GetMethod),
                new CodeMatch(predicate: i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S),
            });

            if (codeMatcher.IsValid)
            {
                // overwrite call get__zone → ldc.i4.1
                codeMatcher.Set(opcode: OpCodes.Ldc_I4_1, operand: null);

                // overwrite callvirt get_IsPCFaction → nop
                codeMatcher.Advance(offset: 1);
                codeMatcher.Set(opcode: OpCodes.Nop, operand: null);
            }
            
            return codeMatcher.Instructions();
        }
    }
}