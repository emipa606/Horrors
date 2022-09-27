using Verse;

namespace Horrors;

public static class HorrorHivesUtility
{
    public static int TotalSpawnedDensCount(Map map)
    {
        var num = 0;
        var allThings = map.listerThings.AllThings;
        foreach (var thing in allThings)
        {
            if (thing is HorrorDen)
            {
                num++;
            }
        }

        return num;
    }

    public static int TotalSpawnedHivesCount(Map map)
    {
        var num = 0;
        var allThings = map.listerThings.AllThings;
        foreach (var thing in allThings)
        {
            if (thing is HorrorHive)
            {
                num++;
            }
        }

        return num;
    }
}