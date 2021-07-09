using Verse;
using Verse.AI;

namespace Horrors
{
    public class JobGiver_WebArea : JobGiver_Wander
    {
        public JobGiver_WebArea()
        {
            wanderRadius = 7.5f;
            ticksBetweenWandersRange = new IntRange(125, 200);
        }

        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            if (!(pawn.mindState.duty.focus.Thing is HorrorHive {Spawned: true} hive))
            {
                return pawn.Position;
            }

            return hive.Position;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            // Log.Message("Attempting to create horror web.");
            if (pawn.Position.GetPlant(pawn.Map) != null)
            {
                // Log.Message("There is something here.");
                if (pawn.Position.GetPlant(pawn.Map).def.defName == "HorrorWeb")
                {
                    // Log.Message("Already webbed.");
                    return null;
                }
            }
            else
            {
                // Log.Message("There is no web or plant here.");
                GenSpawn.Spawn(ThingDef.Named("HorrorWeb"), pawn.Position, pawn.Map);
                return null;
            }

            return null;
        }
    }
}