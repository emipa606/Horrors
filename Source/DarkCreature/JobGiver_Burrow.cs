using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace Horrors
{
    public class JobGiver_Burrow : JobGiver_Wander
    {
        public JobGiver_Burrow()
        {
            this.wanderRadius = 7.5f;
            this.ticksBetweenWandersRange = new IntRange(125, 200);
        }



        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
                return pawn.Position;
        }



        protected override Job TryGiveJob(Pawn pawn)
        {


            Building b = GridsUtility.GetFirstBuilding(pawn.Position, pawn.Map);
            Log.Message("Attempting to burrow.");
            if (b != null)
            {
                Log.Message("There is a thing here.");
                return null;
            }
            else
            {
            Log.Message("There is nothing here, burrowing.");
            Thing e = GenSpawn.Spawn(ThingDef.Named("HorrorBurrow"), pawn.Position, pawn.Map);

            return new Job(JobDefOf.EnterCryptosleepCasket, (Building_CryptosleepCasket)e);
            }
        }
    }
}
