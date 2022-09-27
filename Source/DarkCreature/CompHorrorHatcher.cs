using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Horrors;

public class CompHorrorHatcher : ThingComp
{
    private float gestateProgress;
    public Faction hatcheeFaction;

    public CompProperties_HorrorHatcher Props => (CompProperties_HorrorHatcher)props;

    public override string CompInspectStringExtra()
    {
        return "EggProgress".Translate() + ": " + gestateProgress.ToStringPercent() + "\n" + "Gestating: " +
               Props.hatcherPawn.defName;
    }

    public override void CompTick()
    {
        var num = 1f / (Props.hatcherDaystoHatch * 60000f);
        gestateProgress += num;
        if (gestateProgress > 1f)
        {
            Hatch();
        }
    }

    public override void CompTickRare()
    {
        var num = 1f / (Props.hatcherDaystoHatch * 300f);
        gestateProgress += num;
        if (gestateProgress > 1f)
        {
            Hatch();
        }
    }

    public void Hatch()
    {
        var request = new PawnGenerationRequest(Props.hatcherPawn, hatcheeFaction, PawnGenerationContext.NonPlayer,
            -1, false, true, false, false, true, false, 1f, false, true, true, false);
        var hatchling = PawnGenerator.GeneratePawn(request);
        hatchling.SetFactionDirect(Find.FactionManager.FirstFactionOfDef(FactionDef.Named("Horrors")));
        PawnUtility.TrySpawnHatchedOrBornPawn(hatchling, parent);
        PawnInventoryGenerator.GenerateInventoryFor(hatchling, request);
        PawnWeaponGenerator.TryGenerateWeaponFor(hatchling, request);
        var lord = CreateNewLord();
        lord.AddPawn(hatchling);
        parent.Destroy();
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref gestateProgress, "gestateProgress");
        Scribe_References.Look(ref hatcheeFaction, "hatcheeFaction");
    }

    private Lord CreateNewLord()
    {
        var faction = Find.FactionManager.FirstFactionOfDef(FactionDef.Named("Horrors"));
        return LordMaker.MakeNewLord(faction, new LordJob_DefendPoint(parent.Position, 5f), parent.Map);
    }
}