using Verse;

namespace Horrors;

public class CompProperties_SpawnerHorrorHives : CompProperties
{
    public readonly float HiveSpawnPreferredMinDist = 3.5f;

    public readonly float HiveSpawnRadius = 10f;
    public FloatRange HiveSpawnIntervalDays = new(1.6f, 2.1f);

    public CompProperties_SpawnerHorrorHives()
    {
        compClass = typeof(CompSpawnerHorrorHives);
    }
}