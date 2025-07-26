using RimWorld;
using UnityEngine;
using Verse;

namespace Horrors;

public class Fume : Thing
{
    private int counter;
    private int destroyTick;

    public float graphicRotation;

    private float graphicRotationSpeed;

    private float num = 0.05f;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref destroyTick, "destroyTick");
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        while (true)
        {
            Thing gas = Position.GetGas(map);
            if (gas == null)
            {
                break;
            }

            gas.Destroy();
        }

        counter = 0;
        base.SpawnSetup(map, respawningAfterLoad);
        destroyTick = Find.TickManager.TicksGame + def.gas.expireSeconds.RandomInRange.SecondsToTicks();
        graphicRotationSpeed = Rand.Range(-def.gas.rotationSpeed, def.gas.rotationSpeed) / 60f;
    }

    protected override void Tick()
    {
        counter += 1;

        if (counter > 100)
        {
            var pawn = Position.GetFirstPawn(Map);

            if (pawn is { Faction: not null })
            {
                if (pawn.Faction.def.defName != "Horrors")
                {
                    num /= Mathf.Max(pawn.GetStatValue(StatDefOf.ToxicResistance), 0.001f);
                    var num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(pawn.thingIDNumber ^ 74374237));
                    num *= num2;
                    HealthUtility.AdjustSeverity(pawn, HediffDefOf.ToxicBuildup, num);
                }
            }

            counter = 0;
        }

        if (destroyTick <= Find.TickManager.TicksGame)
        {
            Destroy();
        }

        graphicRotation += graphicRotationSpeed;
    }
}