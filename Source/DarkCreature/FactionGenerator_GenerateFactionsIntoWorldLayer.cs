using HarmonyLib;
using RimWorld;
using Verse;

namespace Horrors;

[HarmonyPatch(typeof(FactionGenerator), nameof(FactionGenerator.GenerateFactionsIntoWorldLayer))]
public class FactionGenerator_GenerateFactionsIntoWorldLayer
{
    private static void Postfix()
    {
        var settlements = Find.WorldObjects.SettlementBases;
        foreach (var settlement in settlements)
        {
            if (settlement.Faction.def.defName != "Horrors")
            {
                continue;
            }

            var settlementTile = Find.WorldGrid[settlement.Tile];
            settlementTile.PrimaryBiome = BiomeDef.Named("HorrorWastes");
        }
    }
}