using Verse;
using Verse.AI;

namespace Horrors;

public class ThinkNode_ConditionalHighCover : ThinkNode_Conditional
{
    public readonly float min = 0.4f;

    protected override bool Satisfied(Pawn pawn)
    {
        var coverValue = CoverUtility.TotalSurroundingCoverScore(pawn.Position, pawn.Map);
        return coverValue >= min;
    }
}