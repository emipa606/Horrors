using System;
using Verse;
using RimWorld;
using UnityEngine;

namespace Horrors
{
    public class Fume : Thing
    {
        public int destroyTick;
        public float graphicRotation;
        public float graphicRotationSpeed;
        float num = 0.05f;
        int counter;
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            while (true)
            {
                Thing gas = base.Position.GetGas(map);
                if (gas == null)
                {
                    break;
                }
                gas.Destroy(DestroyMode.Vanish);
            }
            counter = 0;
            base.SpawnSetup(map, respawningAfterLoad);
            this.destroyTick = Find.TickManager.TicksGame + this.def.gas.expireSeconds.RandomInRange.SecondsToTicks();
            this.graphicRotationSpeed = Rand.Range(-this.def.gas.rotationSpeed, this.def.gas.rotationSpeed) / 60f;
        }
        public override void Tick()
        {
            counter += 1;

            if (counter > 100)
            {
                var pawn = GridsUtility.GetFirstPawn(this.Position, this.Map);

                if (pawn != null)
                {
                    if (pawn.Faction != null)
                    {
                        if (pawn.Faction.def.defName != "Horrors")
                        {
                            num *= pawn.GetStatValue(StatDefOf.ToxicSensitivity, true);
                            float num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(pawn.thingIDNumber ^ 74374237));
                            num *= num2;
                            HealthUtility.AdjustSeverity(pawn, HediffDefOf.ToxicBuildup, num);
                        }
                    }
                }
                counter = 0;
            }

            if (this.destroyTick <= Find.TickManager.TicksGame)
            {
                this.Destroy(DestroyMode.Vanish);
            }
            this.graphicRotation += this.graphicRotationSpeed;
            
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.destroyTick, "destroyTick", 0, false);
        }
    }
}
