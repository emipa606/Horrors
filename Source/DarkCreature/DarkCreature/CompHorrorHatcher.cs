using RimWorld.Planet;
using System;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.AI.Group;

namespace Horrors
{
    public class CompHorrorHatcher : ThingComp
    {
        private float gestateProgress;
        public Faction hatcheeFaction;
        public CompProperties_HorrorHatcher Props
        {
            get
            {
                return (CompProperties_HorrorHatcher)this.props;
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.gestateProgress, "gestateProgress", 0f, false);
            Scribe_References.Look<Faction>(ref this.hatcheeFaction, "hatcheeFaction", false);
        }
        public override void CompTickRare()
        {
            float num = 1f / (this.Props.hatcherDaystoHatch * 300f);
            this.gestateProgress += num;
            if (this.gestateProgress > 1f)
            {
                this.Hatch();
            }
        }
        public override void CompTick()
        {
            float num = 1f / (this.Props.hatcherDaystoHatch * 60000f);
            this.gestateProgress += num;
            if (this.gestateProgress > 1f)
            {
                this.Hatch();
            }
        }
        public void Hatch()
        {
            PawnGenerationRequest request = new PawnGenerationRequest(this.Props.hatcherPawn, this.hatcheeFaction, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false, false, false, 0, null, 1, null, null, null, null, null);
            Pawn hatchling = PawnGenerator.GeneratePawn(request);
            hatchling.SetFactionDirect(Find.FactionManager.FirstFactionOfDef(FactionDef.Named("Horrors")));
            PawnUtility.TrySpawnHatchedOrBornPawn(hatchling, this.parent);
            PawnInventoryGenerator.GenerateInventoryFor(hatchling, request);
            PawnWeaponGenerator.TryGenerateWeaponFor(hatchling, request);
            Lord lord = CreateNewLord();
            lord.AddPawn(hatchling);
            this.parent.Destroy(DestroyMode.Vanish);
        }
        public override string CompInspectStringExtra()
        {
            return "EggProgress".Translate() + ": " + this.gestateProgress.ToStringPercent() + "\n" + "Gestating: " + this.Props.hatcherPawn.defName;
        }

        private Lord CreateNewLord()
        {
            return LordMaker.MakeNewLord(Find.FactionManager.FirstFactionOfDef(FactionDef.Named("Horrors")), new LordJob_DefendPoint(this.parent.Position), this.parent.Map, null);
        }
    }
}
