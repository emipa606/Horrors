using Verse;
using Verse.AI;

namespace Horrors;

public class ThinkNode_ConditionalCanBurrow : ThinkNode_Conditional
{
    protected override bool Satisfied(Pawn pawn)
    {
        var defName = pawn.def.defName;

        return defName == "Prowler" && !pawn.mindState.anyCloseHostilesRecently;
    }
}