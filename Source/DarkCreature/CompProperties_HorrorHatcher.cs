﻿using Verse;

namespace Horrors;

public class CompProperties_HorrorHatcher : CompProperties
{
    public readonly float hatcherDaystoHatch = 1f;

    public PawnKindDef hatcherPawn;

    public CompProperties_HorrorHatcher()
    {
        compClass = typeof(CompHorrorHatcher);
    }
}