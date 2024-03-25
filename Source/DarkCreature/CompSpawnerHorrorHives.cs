using RimWorld;
using UnityEngine;
using Verse;

namespace Horrors;

public class CompSpawnerHorrorHives : ThingComp
{
    public const int MaxHivesPerMap = 30;

    private int nextHiveSpawnTick = -1;

    private bool CanSpawnChildHive => HorrorHivesUtility.TotalSpawnedHivesCount(parent.Map) < 10;

    private CompProperties_SpawnerHorrorHives Props => (CompProperties_SpawnerHorrorHives)props;

    public void CalculateNextHiveSpawnTick()
    {
        var room = parent.GetRoom();
        var num = 0;
        var num2 = GenRadial.NumCellsInRadius(9f);
        for (var i = 0; i < num2; i++)
        {
            var intVec = parent.Position + GenRadial.RadialPattern[i];
            if (!intVec.InBounds(parent.Map))
            {
                continue;
            }

            if (intVec.GetRoom(parent.Map) != room)
            {
                continue;
            }

            if (intVec.GetThingList(parent.Map).Any(t => t is Hive))
            {
                num++;
            }
        }

        var num3 = GenMath.LerpDouble(0f, 7f, 1f, 0.35f, Mathf.Clamp(num, 0, 7));
        nextHiveSpawnTick = Find.TickManager.TicksGame + (int)(Props.HiveSpawnIntervalDays.RandomInRange * 60000f /
                                                               (num3 * Find.Storyteller.difficulty
                                                                   .enemyReproductionRateFactor));
    }

    public override string CompInspectStringExtra()
    {
        string text = null;
        if (CanSpawnChildHive)
        {
            text = "HiveReproducesIn".Translate() + ": " +
                   (nextHiveSpawnTick - Find.TickManager.TicksGame).ToStringTicksToPeriod();
        }

        return text;
    }

    public override void CompTickRare()
    {
        if (parent is HorrorHive { active: false } || Find.TickManager.TicksGame < nextHiveSpawnTick)
        {
            return;
        }

        if (TrySpawnChildHive(false, out var hive2))
        {
            hive2.nextPawnSpawnTick = Find.TickManager.TicksGame + Rand.Range(150, 350);
            Messages.Message("MessageHiveReproduced".Translate(), hive2, MessageTypeDefOf.NegativeEvent);
        }
        else
        {
            CalculateNextHiveSpawnTick();
        }
    }

    public override void PostExposeData()
    {
        Scribe_Values.Look(ref nextHiveSpawnTick, "nextHiveSpawnTick");
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        if (!respawningAfterLoad)
        {
            CalculateNextHiveSpawnTick();
        }
    }

    private bool CanSpawnHiveAt(IntVec3 c, float minDist, bool ignoreRoofedRequirement)
    {
        if (!ignoreRoofedRequirement && !c.Roofed(parent.Map) || !c.Standable(parent.Map) ||
            minDist != 0f && c.DistanceToSquared(parent.Position) < minDist * minDist)
        {
            return false;
        }

        for (var i = 0; i < 8; i++)
        {
            var c2 = c + GenAdj.AdjacentCells[i];
            if (!c2.InBounds(parent.Map))
            {
                continue;
            }

            var thingList = c2.GetThingList(parent.Map);
            foreach (var thing in thingList)
            {
                if (thing is HorrorHive)
                {
                    return false;
                }
            }
        }

        var thingList2 = c.GetThingList(parent.Map);
        foreach (var thing in thingList2)
        {
            if (thing.def.category is ThingCategory.Item or ThingCategory.Building &&
                GenSpawn.SpawningWipes(parent.def, thing.def))
            {
                return false;
            }
        }

        return true;
    }

    private bool TrySpawnChildHive(bool ignoreRoofedRequirement, out HorrorHive newHive)
    {
        if (!CanSpawnChildHive)
        {
            newHive = null;
            return false;
        }

        var invalid = IntVec3.Invalid;
        for (var i = 0; i < 3; i++)
        {
            var minDist = Props.HiveSpawnPreferredMinDist;
            switch (i)
            {
                case 1:
                    minDist = 0f;
                    break;
                case 2:
                    newHive = null;
                    return false;
            }

            if (CellFinder.TryFindRandomReachableCellNearPosition(parent.Position, parent.Position, parent.Map,
                    Props.HiveSpawnRadius,
                    TraverseParms.For(TraverseMode.NoPassClosedDoors),
                    c => CanSpawnHiveAt(c, minDist, ignoreRoofedRequirement), null, out invalid))
            {
                break;
            }
        }

        newHive = (HorrorHive)GenSpawn.Spawn(parent.def, invalid, parent.Map);
        if (newHive.Faction != parent.Faction)
        {
            newHive.SetFaction(parent.Faction);
        }

        if (parent is HorrorHive hive)
        {
            newHive.active = hive.active;
        }

        CalculateNextHiveSpawnTick();
        return true;
    }
}