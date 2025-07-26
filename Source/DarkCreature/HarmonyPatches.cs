using System.Reflection;
using HarmonyLib;
using Verse;

namespace Horrors;

[StaticConstructorOnStartup]
internal class HarmonyPatches
{
    static HarmonyPatches()
    {
        new Harmony("mlie.horrors").PatchAll(Assembly.GetExecutingAssembly());
    }
}