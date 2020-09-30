using System;
using Verse;
using Verse.AI.Group;
using RimWorld;
using System.Collections.Generic;

namespace Horrors
{
	public class RaidStrategyWorker_Harvest : RaidStrategyWorker
	{
        protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
		{
			return new LordJob_AssaultColony(parms.faction, true, true, true, true, false);
		}
		public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
		{
			return base.CanUseWith(parms, groupKind) && parms.faction.def.canUseAvoidGrid && parms.faction.def.defName == "Horrors";
		}
    }
}
