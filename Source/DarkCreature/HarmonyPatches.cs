﻿using HarmonyLib;
using RimWorld;
using Verse;

namespace Horrors;

internal class HarmonyPatches
{
    [StaticConstructorOnStartup]
    internal static class HarmonyStart
    {
        static HarmonyStart()
        {
            new Harmony("mlie.horrors").PatchAll();
        }
    }

    [HarmonyPatch(typeof(FactionGenerator), "GenerateFactionsIntoWorld")]
    private class Patch
    {
        private static void Postfix()
        {
            var settlements = Find.WorldObjects.SettlementBases;
            foreach (var settlement in settlements)
            {
                Log.Message(
                    $"{settlement.Faction.def.defName} owns tile {settlement.Tile} with biome {Find.WorldGrid[settlement.Tile].biome.defName}");
                if (settlement.Faction.def.defName != "Horrors")
                {
                    continue;
                }

                var settlementTile = Find.WorldGrid[settlement.Tile];
                settlementTile.biome = BiomeDef.Named("HorrorWastes");
            }
        }
    }
}