using System.Reflection;
using HarmonyLib;
using Verse;

namespace Horrors;

[StaticConstructorOnStartup]
internal static class HarmonyPatches
{
    static HarmonyPatches()
    {
        new Harmony("mlie.horrors").PatchAll(Assembly.GetExecutingAssembly());
    }
}