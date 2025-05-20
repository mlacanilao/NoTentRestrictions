using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace NoTentRestrictions.Patches
{
    public static class TraitWrenchPatch
    {
        internal static IEnumerable<CodeInstruction> IsValidTargetTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            bool enableNoSoilUpgradeLimit = NoTentRestrictionsConfig.EnableNoSoilUpgradeLimit?.Value ?? false;
            
            if (enableNoSoilUpgradeLimit == false)
            {
                return instructions;
            }
            
            var codeMatcher = new CodeMatcher(instructions: instructions);
            
            codeMatcher.MatchStartForward(matches: new[]
            {
                new CodeMatch(opcode: OpCodes.Ldloc_0),                                   // ldloc.0         (load local variable 0 — the Zone object)
                new CodeMatch(opcode: OpCodes.Ldc_I4, operand: 2200),                     // ldc.i4 2200     (push constant 2200 — the element ID)
                new CodeMatch(opcode: OpCodes.Callvirt, operand: typeof(Zone).GetMethod(  // callvirt        (call Zone.Evalue(int))
                    name: "Evalue",
                    types: new[] { typeof(int) })),
                new CodeMatch(opcode: OpCodes.Ldc_I4_S, operand: (sbyte)10),              // ldc.i4.s 10     (push constant 10 — max allowed upgrades)
                new CodeMatch(opcode: OpCodes.Clt),                                       // clt             (compare if Evalue < 10)
            });

            if (codeMatcher.IsValid)
            {
                // Replace ldc.i4.s 10 with ldc.i4 int.MaxValue
                codeMatcher.Advance(offset: 3); // Move to ldc.i4.s 10
                codeMatcher.Set(opcode: OpCodes.Ldc_I4, operand: int.MaxValue);
            }
            
            return codeMatcher.Instructions();
        }
    }
}