using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace Horrors
{
    public static class HorrorHivesUtility
    {
        public static int TotalSpawnedHivesCount(Map map)
        {
            int num = 0;
            List<Thing> allThings = map.listerThings.AllThings;
            for (int i = 0; i < allThings.Count; i++)
            {
                if (allThings[i] is HorrorHive)
                {
                    num++;
                }
            }
            return num;
        }
        public static int TotalSpawnedDensCount(Map map)
        {
            int num = 0;
            List<Thing> allThings = map.listerThings.AllThings;
            for (int i = 0; i < allThings.Count; i++)
            {
                if (allThings[i] is HorrorDen)
                {
                    num++;
                }
            }
            return num;
        }
    }
}
