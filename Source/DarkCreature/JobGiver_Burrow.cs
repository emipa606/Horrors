using RimWorld;
using Verse;
using Verse.AI;

namespace Horrors;

public class JobGiver_Burrow : JobGiver_Wander
{
    public JobGiver_Burrow()
    {
        wanderRadius = 7.5f;
        ticksBetweenWandersRange = new IntRange(125, 200);
    }

    protected override IntVec3 GetWanderRoot(Pawn pawn)
    {
        return pawn.Position;
    }

    protected override Job TryGiveJob(Pawn pawn)
    {
        var b = pawn.Position.GetFirstBuilding(pawn.Map);
        if (b != null)
        {
            return null;
        }

        var e = GenSpawn.Spawn(ThingDef.Named("HorrorBurrow"), pawn.Position, pawn.Map);

        return new Job(JobDefOf.EnterCryptosleepCasket, (Building_CryptosleepCasket)e);
    }
}