using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Horrors;

public class HorrorDen : ThingWithComps
{
    private const int InitialPawnSpawnDelay = 420;

    private const int InitialPawnsPoints = 200;

    private const float MaxSpawnedPawnsPoints = 800f;

    private const int PawnSpawnRadius = 4;

    private static readonly string MemoAttackedByEnemy = "HiveAttacked";

    private static readonly string MemoBurnedBadly = "HiveBurnedBadly";

    private static readonly string MemoDestroyed = "HiveDestroyed";

    private static readonly FloatRange PawnSpawnIntervalDays = new(0.85f, 1.1f);

    public bool active = true;

    public int nextPawnSpawnTick = -1;

    private List<Pawn> spawnedPawns = [];

    private int ticksToSpawnInitialPawns = -1;

    private Lord Lord
    {
        get
        {
            var foundPawn = spawnedPawns.Find(HasDefendHiveLord);
            if (!Spawned)
            {
                return null;
            }

            if (foundPawn == null)
            {
                RegionTraverser.BreadthFirstTraverse(
                    this.GetRegion(),
                    (_, _) => true,
                    delegate(Region r)
                    {
                        var list = r.ListerThings.ThingsOfDef(ThingDef.Named("HorrorDen"));
                        foreach (var thing in list)
                        {
                            if (thing == this)
                            {
                                continue;
                            }

                            if (thing.Faction != Faction)
                            {
                                continue;
                            }

                            foundPawn = ((HorrorDen)thing).spawnedPawns.Find(HasDefendHiveLord);
                            if (foundPawn != null)
                            {
                                return true;
                            }
                        }

                        return false;
                    },
                    20);
            }

            return foundPawn?.GetLord();

            bool HasDefendHiveLord(Pawn x)
            {
                var lord = x.GetLord();
                return lord?.LordJob is LordJob_DefendAndExpandHive;
            }
        }
    }

    private float SpawnedPawnsPoints
    {
        get
        {
            FilterOutUnspawnedPawns();
            var num = 0f;
            foreach (var pawn in spawnedPawns)
            {
                num += pawn.kindDef.combatPower;
            }

            return num;
        }
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        var map = Map;
        base.DeSpawn(mode);
        var lords = map.lordManager.lords;
        foreach (var lord in lords)
        {
            lord.ReceiveMemo(MemoDestroyed);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref active, "active");
        Scribe_Values.Look(ref nextPawnSpawnTick, "nextPawnSpawnTick");
        Scribe_Collections.Look(ref spawnedPawns, "spawnedPawns", LookMode.Reference);
        Scribe_Values.Look(ref ticksToSpawnInitialPawns, "ticksToSpawnInitialPawns");
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            spawnedPawns.RemoveAll(x => x == null);
        }
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var g in base.GetGizmos())
        {
            yield return g;
        }
    }

    public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
    {
        if (dinfo.Def.ExternalViolenceFor(this) && dinfo.Instigator?.Faction != null)
        {
            if (ticksToSpawnInitialPawns > 0)
            {
                SpawnInitialPawnsNow();
            }

            var lord = Lord;
            lord?.ReceiveMemo(MemoAttackedByEnemy);
        }

        if (dinfo.Def == DamageDefOf.Flame && HitPoints < MaxHitPoints * 0.3f)
        {
            var lord2 = Lord;
            lord2?.ReceiveMemo(MemoBurnedBadly);
        }

        base.PostApplyDamage(dinfo, totalDamageDealt);
    }

    public override bool PreventPlayerSellingThingsNearby(out string reason)
    {
        if (spawnedPawns.Count > 0)
        {
            if (spawnedPawns.Any(p => !p.Downed))
            {
                reason = def.label;
                return true;
            }
        }

        reason = null;
        return false;
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        if (Faction == null)
        {
            SetFaction(Find.FactionManager.FirstFactionOfDef(FactionDef.Named("Horrors")));
        }

        if (!respawningAfterLoad)
        {
            ticksToSpawnInitialPawns = 420;
        }
    }

    public override void TickRare()
    {
        base.TickRare();
        if (!Spawned)
        {
            return;
        }

        FilterOutUnspawnedPawns();
        if (!active && !Position.Fogged(Map))
        {
            Activate();
        }

        if (!active)
        {
            return;
        }

        if (ticksToSpawnInitialPawns > 0)
        {
            ticksToSpawnInitialPawns -= 250;
            if (ticksToSpawnInitialPawns <= 0)
            {
                SpawnInitialPawnsNow();
            }
        }

        if (Find.TickManager.TicksGame < nextPawnSpawnTick)
        {
            return;
        }

        if (SpawnedPawnsPoints < 500f)
        {
            if (TrySpawnPawn(out var pawn))
            {
                pawn.caller?.DoCall();
            }
        }

        CalculateNextPawnSpawnTick();
    }

    private void Activate()
    {
        active = true;
        nextPawnSpawnTick = Find.TickManager.TicksGame + Rand.Range(200, 400);
        var comp = GetComp<CompSpawnerHorrorDen>();
        comp?.CalculateNextHiveSpawnTick();
    }

    private void CalculateNextPawnSpawnTick()
    {
        var num = GenMath.LerpDouble(0f, 5f, 1f, 0.5f, spawnedPawns.Count);
        nextPawnSpawnTick = Find.TickManager.TicksGame + (int)(PawnSpawnIntervalDays.RandomInRange * 60000f /
                                                               (num * Find.Storyteller.difficulty
                                                                   .enemyReproductionRateFactor));
    }

    private Lord CreateNewLord()
    {
        return LordMaker.MakeNewLord(Faction, new LordJob_DefendAndExpandHive(), Map);
    }

    private void FilterOutUnspawnedPawns()
    {
        for (var i = spawnedPawns.Count - 1; i >= 0; i--)
        {
            if (!spawnedPawns[i].Spawned)
            {
                spawnedPawns.RemoveAt(i);
            }
        }
    }

    private void SpawnInitialPawnsNow()
    {
        ticksToSpawnInitialPawns = -1;
        while (SpawnedPawnsPoints < 200f)
        {
            if (!TrySpawnPawn(out _))
            {
                return;
            }
        }

        CalculateNextPawnSpawnTick();
    }

    private bool TrySpawnPawn(out Pawn pawn)
    {
        var list = new List<PawnKindDef> { PawnKindDef.Named("Terrorworm"), PawnKindDef.Named("Visceral") };
        var curPoints = SpawnedPawnsPoints;
        var source = from x in list where curPoints + x.combatPower <= 500f select x;
        if (!source.TryRandomElement(out var kindDef))
        {
            pawn = null;
            return false;
        }

        pawn = PawnGenerator.GeneratePawn(kindDef, Faction);
        PawnUtility.TrySpawnHatchedOrBornPawn(pawn, this);

        spawnedPawns.Add(pawn);
        var lord = Lord;
        if (lord == null)
        {
            lord = CreateNewLord();
        }

        lord.AddPawn(pawn);
        return true;
    }
}