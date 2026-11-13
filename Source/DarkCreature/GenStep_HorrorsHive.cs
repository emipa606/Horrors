using System;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace Horrors;

public class GenStep_HorrorsHive : GenStep_Scatterer
{
    private static readonly IntRange FactionBaseSizeRange = new(22, 23);

    public override int SeedPart => 414596103;

    protected override bool CanScatterAt(IntVec3 loc, Map map)
    {
        return base.CanScatterAt(loc, map) && loc.Standable(map) && !loc.Roofed(map) &&
               map.reachability.CanReachMapEdge(loc, TraverseParms.For(TraverseMode.PassDoors));
    }

    protected override void ScatterAt(IntVec3 loc, Map map, GenStepParams parms, int count = 1)
    {
        var noiseArray = new double[map.Size.x, map.Size.z];

        var caveNoise = GeneratePerlinNoiseForCaves(map);

        for (var i = 0; i < map.Size.x; i++)
        {
            for (var n = 0; n < map.Size.z; n++)
            {
                noiseArray[i, n] = caveNoise.layeredNoise(Convert.ToDouble(i) / 10, Convert.ToDouble(n) / 10);
            }
        }

        for (var i = 0; i < map.Size.x; i++)
        {
            for (var n = 0; n < map.Size.z; n++)
            {
                var vec3 = new IntVec3(i, 0, n);

                if (CoverUtility.TotalSurroundingCoverScore(vec3, map) > 0.2)
                {
                    GenSpawn.Spawn(ThingDef.Named("HorrorWeb"), vec3, map);
                }

                if (!(noiseArray[i, n] > 0.5))
                {
                    continue;
                }

                if (vec3.GetEdifice(map) != null)
                {
                    vec3.GetEdifice(map).Destroy(DestroyMode.KillFinalize);
                }

                if (map.fogGrid.IsFogged(vec3))
                {
                    map.fogGrid.Unfog(vec3);
                }
            }
        }

        var rect = new CellRect(0, 0, 0, 0);
        Faction faction;
        if (map.info.parent?.Faction == null || map.info.parent.Faction == Faction.OfPlayer)
        {
            faction = Find.FactionManager.RandomEnemyFaction();
        }
        else
        {
            faction = map.info.parent.Faction;
        }

        rect = rect.ExpandedBy(4);

        rect.ClipInsideMap(map);
        BaseGen.globalSettings.map = map;


        // Pawns
        var queenPawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("Pirate"), faction);

        CellFinder.TryFindRandomCellNear(map.Center, map, 20, Validator, out var cellReturned);
        GenSpawn.Spawn(ThingDef.Named("SinkHole"), map.Center, map);

        CellFinder.TryFindRandomCellNear(map.Center, map, 20, Validator, out cellReturned);
        GenSpawn.Spawn(ThingDef.Named("SinkHole"), cellReturned, map);

        CellFinder.TryFindRandomCellNear(map.Center, map, 20, Validator, out cellReturned);
        GenSpawn.Spawn(ThingDef.Named("SinkHole"), cellReturned, map);

        CellFinder.TryFindRandomCellNear(map.Center, map, 20, Validator, out cellReturned);
        GenSpawn.Spawn(ThingDef.Named("SinkHole"), cellReturned, map);

        CellFinder.TryFindRandomCellNear(map.Center, map, 20, Validator, out cellReturned);

        var queen = GenSpawn.Spawn(queenPawn, cellReturned, map);

        var pawnToSpawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("BroodLord"), faction);

        GenSpawn.Spawn(pawnToSpawn, cellReturned, map);
        pawnToSpawn.training.Train(TrainableDefOf.Obedience, (Pawn)queen);
        return;

        bool Validator(IntVec3 p)
        {
            return DropCellFinder.IsGoodDropSpot(p, map, true, true);
        }
    }

    private static TurboNoise GeneratePerlinNoiseForCaves(Map map)
    {
        var noiseToOutput = new TurboNoise();
        noiseToOutput.addLayer(map.GetHashCode(), 2.0, 0.1);

        return noiseToOutput;
    }
}