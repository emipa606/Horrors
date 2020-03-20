using System;
using Verse;
using RimWorld;

namespace Horrors
{
    public class CompProperties_HorrorHatcher : CompProperties
    {
        public float hatcherDaystoHatch = 1f;
        public PawnKindDef hatcherPawn;
        public CompProperties_HorrorHatcher()
        {
            this.compClass = typeof(CompHorrorHatcher);
        }
    }
}
