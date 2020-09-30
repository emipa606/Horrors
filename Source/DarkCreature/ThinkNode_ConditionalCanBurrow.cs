using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace Horrors
{
    public class ThinkNode_ConditionalCanBurrow : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            String defName = pawn.def.defName;

            if (defName == "Prowler" && !pawn.mindState.anyCloseHostilesRecently)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
