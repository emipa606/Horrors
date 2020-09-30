﻿using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace Horrors
{
    public class JobGiver_WebArea : JobGiver_Wander
    {
        public JobGiver_WebArea()
        {
            this.wanderRadius = 7.5f;
            this.ticksBetweenWandersRange = new IntRange(125, 200);
        }
        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            HorrorHive hive = pawn.mindState.duty.focus.Thing as HorrorHive;
            if (hive == null || !hive.Spawned)
            {
                return pawn.Position;
            }
            return hive.Position;
        }
        protected override Job TryGiveJob(Pawn pawn)
        {

            //Log.Message("Attempting to create horror web.");
            if (pawn.Position.GetPlant(pawn.Map) != null)
            {
                //Log.Message("There is something here.");
                if (pawn.Position.GetPlant(pawn.Map).def.defName == "HorrorWeb")
                {
                    //Log.Message("Already webbed.");
                    return null;
                }
            }
            else
            {
            //Log.Message("There is no web or plant here.");
            GenSpawn.Spawn(ThingDef.Named("HorrorWeb"), pawn.Position, pawn.Map);
            return null;
            }

            return null;
        }
    }
}
