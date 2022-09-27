using Verse;

namespace Horrors;

public class CompProperties_SpawnerHorrorHives : CompProperties
{
    public FloatRange HiveSpawnIntervalDays = new FloatRange(1.6f, 2.1f);

    public float HiveSpawnPreferredMinDist = 3.5f;

    public float HiveSpawnRadius = 10f;

    public CompProperties_SpawnerHorrorHives()
    {
        compClass = typeof(CompSpawnerHorrorHives);
    }
}