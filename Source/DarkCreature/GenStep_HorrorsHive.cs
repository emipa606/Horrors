using RimWorld.BaseGen;
using System;
using Verse;
using RimWorld;
using Horrors;

namespace Horrors
{


    public class GenStep_HorrorsHive : GenStep_Scatterer
    {

        private static readonly IntRange FactionBaseSizeRange = new IntRange(22, 23);

        public override int SeedPart
        {
            get
            {
                return 414596103;
            }
        }

        protected override bool CanScatterAt(IntVec3 c, Map map)
        {
            return base.CanScatterAt(c, map) && c.Standable(map) && !c.Roofed(map) && map.reachability.CanReachMapEdge(c, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false));
        }

        protected override void ScatterAt(IntVec3 c, Map map, GenStepParams parms, int stackCount = 1)
        {
            Log.Message("Horrors: Initiating Hive Spawn");
            TurboNoise caveNoise = new TurboNoise();
            double[ , ] noiseArray = new double[map.Size.x, map.Size.z];

            caveNoise = GeneratePerlinNoiseForCaves(map);
            Log.Message("Horrors: Noise Generated Successfully");

            Log.Message("Horrors: Stepping through noise grid");
            for (int i = 0; i < map.Size.x; i++)
            {
                for (int n = 0; n < map.Size.z; n++)
                {
                    noiseArray[i, n] = caveNoise.layeredNoise(Convert.ToDouble(i) / 10, Convert.ToDouble(n) / 10);
                }
            }

            Log.Message("Horrors: Digging out tunnels");
            for (int i = 0; i < map.Size.x; i++)
            {
                for (int n = 0; n < map.Size.z; n++)
                {
                    IntVec3 loc = new IntVec3(i, 0, n);

                    if (CoverUtility.TotalSurroundingCoverScore(loc, map) > 0.2)
	                {
		                GenSpawn.Spawn(ThingDef.Named("HorrorWeb"), loc, map);
	                }

                    if (noiseArray[i,n] > 0.5)
                    {
                        if (GridsUtility.GetEdifice(loc, map) != null)
                        {
                            GridsUtility.GetEdifice(loc, map).Destroy(DestroyMode.KillFinalize);
                        }

                        if (map.fogGrid.IsFogged(loc))
                        {
                            map.fogGrid.Unfog(loc);
                        }
                        
                        //map.terrainGrid.SetTerrain(loc, TerrainDef.Named("FloorWebs"));
                    }
                }
            }

            Log.Message("Horrors: Generating Hive Rect");
            int randomInRange = GenStep_HorrorsHive.FactionBaseSizeRange.RandomInRange;
            int randomInRange2 = GenStep_HorrorsHive.FactionBaseSizeRange.RandomInRange;
            CellRect rect = new CellRect(0, 0, 0, 0);
            Faction faction;
            if (map.info.parent == null || map.info.parent.Faction == null || map.info.parent.Faction == Faction.OfPlayer)
            {
                faction = Find.FactionManager.RandomEnemyFaction(false, false);
            }
            else
            {
                faction = map.info.parent.Faction;
            }

            rect = rect.ExpandedBy(4);

            rect.ClipInsideMap(map);
            ResolveParams resolveParams = default(ResolveParams);
            resolveParams.rect = rect;
            resolveParams.faction = faction;
            BaseGen.globalSettings.map = map;

            Log.Message("Horrors: Validating Spawns");

            Predicate<IntVec3> validator = (IntVec3 p) => DropCellFinder.IsGoodDropSpot(p, map, true, true);
            IntVec3 cellReturned;

            // Pawns
            Pawn pawnToSpawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("Pirate"), faction);
            Pawn queenPawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("Pirate"), faction); 


            Log.Message("Horrors: Spawning Sinkhole");
            CellFinder.TryFindRandomCellNear(map.Center, map, 20, validator, out cellReturned);
            GenSpawn.Spawn(ThingDef.Named("SinkHole"), map.Center, map);

            Log.Message("Horrors: Spawning Sinkhole");
            CellFinder.TryFindRandomCellNear(map.Center, map, 20, validator, out cellReturned);
            GenSpawn.Spawn(ThingDef.Named("SinkHole"), cellReturned, map);

            Log.Message("Horrors: Spawning Sinkhole");
            CellFinder.TryFindRandomCellNear(map.Center, map, 20, validator, out cellReturned);
            GenSpawn.Spawn(ThingDef.Named("SinkHole"), cellReturned, map);

            Log.Message("Horrors: Spawning Sinkhole");
            CellFinder.TryFindRandomCellNear(map.Center, map, 20, validator, out cellReturned);
            GenSpawn.Spawn(ThingDef.Named("SinkHole"), cellReturned, map);

            Log.Message("Horrors: Spawning Sinkhole");
            CellFinder.TryFindRandomCellNear(map.Center, map, 20, validator, out cellReturned);
         
            Thing queen = GenSpawn.Spawn(queenPawn, cellReturned, map);

            pawnToSpawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("BroodLord"), faction);
            
            GenSpawn.Spawn(pawnToSpawn, cellReturned, map);
            pawnToSpawn.training.Train(TrainableDefOf.Obedience, (Pawn)queen);

        }

        private TurboNoise GeneratePerlinNoiseForCaves(Map map)
        {
            TurboNoise noiseToOutput = new TurboNoise();
            noiseToOutput.addLayer(map.GetHashCode(), 2.0, 0.1);

            Log.Message("Horrors: Generating Hive Perlin Noise");

            return noiseToOutput;
        }
    }
}
