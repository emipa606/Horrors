using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using RimWorld.Planet;

namespace Horrors
{
    [StaticConstructorOnStartup]
	public class HorrorsHive : Settlement
    {

        public override void ExposeData()
        {
            base.ExposeData();
        }

		public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
		{
			alsoRemoveWorldObject = false;
            return false;
		}

        public override void Tick()
        {
            base.Tick();
        }

        public override string GetInspectString()
        {
            return "A hive of Horrors.";
        }

	}
}
