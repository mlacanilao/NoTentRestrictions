using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace NoTentRestrictions.Patches
{
    public class HotItemHeldPatch
    {
        internal static IEnumerable<CodeInstruction> TrySetActTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions: instructions);
            
            codeMatcher.MatchStartForward(matches: new[]
            {
                new CodeMatch(opcode: OpCodes.Ldc_I4_0),  // ldc.i4.0 (Load constant 0, representing 'false')
                new CodeMatch(opcode: OpCodes.Stloc_1),   // stloc.1 (Store constant 0 in the flag variable)
                new CodeMatch(opcode: OpCodes.Ldloc_1),   // ldloc.1 (Load flag)
                new CodeMatch(opcode: OpCodes.Brfalse)    // brfalse (Conditional branch based on flag value)
            });
            
            if (codeMatcher.IsValid)
            {
                // Remove only the 'ldc.i4.0' and 'stloc.1' instructions (2 lines)
                codeMatcher.RemoveInstructions(count: 2);
            }
            
            return codeMatcher.Instructions();
        }
    }
}