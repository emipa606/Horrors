using RimWorld;
using UnityEngine;
using Verse;

namespace Horrors;

public class IncidentWorker_HorrorInfestation : IncidentWorker
{
    private const float HivePoints = 400f;

    protected override bool CanFireNowSub(IncidentParms parms)
    {
        var target = parms.target;
        var map = (Map)target;
        return base.CanFireNowSub(parms) && HorrorHivesUtility.TotalSpawnedHivesCount(map) < 30 &&
               InfestationCellFinder.TryFindCell(out _, map);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        HorrorHive t = null;
        int num;
        for (var i = Mathf.Max(GenMath.RoundRandom(parms.points / HivePoints), 1); i > 0; i -= num)
        {
            num = Mathf.Min(3, i);
            t = SpawnHiveCluster(map);
        }

        SendStandardLetter(parms, t);
        Find.TickManager.slower.SignalForceNormalSpeedShort();
        return true;
    }

    private HorrorHive SpawnHiveCluster(Map map)
    {
        if (!InfestationCellFinder.TryFindCell(out var loc, map))
        {
            return null;
        }

        var hive = (HorrorHive)GenSpawn.Spawn(ThingMaker.MakeThing(ThingDef.Named("HorrorHive")), loc, map);
        hive.SetFaction(Find.FactionManager.FirstFactionOfDef(FactionDef.Named("Horrors")));

        return hive;
    }
}