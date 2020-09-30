using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace Horrors
{
    public class IncidentWorker_HorrorInfestation : IncidentWorker
    {
        private const float HivePoints = 400f;
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            var target = parms.target;
            Map map = (Map)target;
            IntVec3 intVec;
            return base.CanFireNowSub(parms) && HorrorHivesUtility.TotalSpawnedHivesCount(map) < 30 && InfestationCellFinder.TryFindCell(out intVec, map);
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            HorrorHive t = null;
            int num;
            for (int i = Mathf.Max(GenMath.RoundRandom(parms.points / HivePoints), 1); i > 0; i -= num)
            {
                num = Mathf.Min(3, i);
                t = this.SpawnHiveCluster(num, map);
            }
            base.SendStandardLetter(parms, t);
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }
        private HorrorHive SpawnHiveCluster(int hiveCount, Map map)
        {
            IntVec3 loc;
            if (!InfestationCellFinder.TryFindCell(out loc, map))
            {
                return null;
            }
            HorrorHive hive = (HorrorHive)GenSpawn.Spawn(ThingMaker.MakeThing(ThingDef.Named("HorrorHive"), null), loc, map);
            hive.SetFaction(Find.FactionManager.FirstFactionOfDef(FactionDef.Named("Horrors")));

            return hive;
        }
    }
}
