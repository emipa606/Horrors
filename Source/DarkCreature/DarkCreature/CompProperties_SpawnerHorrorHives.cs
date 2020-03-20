using System;
using Verse;
using RimWorld;
namespace Horrors
{
    public class CompProperties_SpawnerHorrorHives : CompProperties
    {
        public float HiveSpawnPreferredMinDist = 3.5f;
        public float HiveSpawnRadius = 10f;
        public FloatRange HiveSpawnIntervalDays = new FloatRange(1.6f, 2.1f);
        public CompProperties_SpawnerHorrorHives()
        {
            this.compClass = typeof(CompSpawnerHorrorHives);
        }
    }
}
