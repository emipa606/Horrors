using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Horrors
{
    public class RaidStrategyWorker_Harvest : RaidStrategyWorker
    {
        public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            return base.CanUseWith(parms, groupKind) && parms.faction.def.canUseAvoidGrid &&
                   parms.faction.def.defName == "Horrors";
        }

        protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
        {
            return new LordJob_AssaultColony(parms.faction, true, true, true, true, false);
        }
    }
}