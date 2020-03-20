using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace Horrors
{
    public class ThinkNode_ConditionalHighCover : ThinkNode_Conditional
    {
        public float min = 0.4f;

        protected override bool Satisfied(Pawn pawn)
        {
            float coverValue = CoverUtility.TotalSurroundingCoverScore(pawn.Position, pawn.Map);
            return coverValue >= this.min;
        }
    }
}
