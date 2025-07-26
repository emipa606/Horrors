using Verse;
using Verse.AI;

namespace Horrors;

public class JobGiver_WebArea : JobGiver_Wander
{
    public JobGiver_WebArea()
    {
        wanderRadius = 7.5f;
        ticksBetweenWandersRange = new IntRange(125, 200);
    }

    protected override IntVec3 GetWanderRoot(Pawn pawn)
    {
        return pawn.mindState.duty.focus.Thing is not HorrorHive { Spawned: true } hive ? pawn.Position : hive.Position;
    }

    protected override Job TryGiveJob(Pawn pawn)
    {
        if (pawn.Position.GetPlant(pawn.Map) == null)
        {
            GenSpawn.Spawn(ThingDef.Named("HorrorWeb"), pawn.Position, pawn.Map);
        }

        return null;
    }
}