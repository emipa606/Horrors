using Verse;

namespace Horrors;

public class CompProperties_SpawnerHorrorDen : CompProperties
{
    public readonly float HiveSpawnPreferredMinDist = 3.5f;

    public readonly float HiveSpawnRadius = 10f;
    public FloatRange HiveSpawnIntervalDays = new FloatRange(1.6f, 2.1f);

    public CompProperties_SpawnerHorrorDen()
    {
        compClass = typeof(CompSpawnerHorrorDen);
    }
}