using RimWorld.Planet;
using Verse;

namespace Horrors
{
    [StaticConstructorOnStartup]
    public class HorrorsHive : Settlement
    {
        public override string GetInspectString()
        {
            return "A hive of Horrors.";
        }

        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            alsoRemoveWorldObject = false;
            return false;
        }
    }
}