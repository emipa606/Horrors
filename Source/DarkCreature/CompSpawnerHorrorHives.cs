﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using RimWorld;

namespace Horrors
{
	public class CompSpawnerHorrorHives : ThingComp
	{
		public const int MaxHivesPerMap = 30;
		private int nextHiveSpawnTick = -1;
		private CompProperties_SpawnerHorrorHives Props
		{
			get
			{
				return (CompProperties_SpawnerHorrorHives)this.props;
			}
		}
		private bool CanSpawnChildHive
		{
			get
			{
				return HorrorHivesUtility.TotalSpawnedHivesCount(this.parent.Map) < 10;
			}
		}
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (!respawningAfterLoad)
			{
				this.CalculateNextHiveSpawnTick();
			}
		}
		public override void CompTickRare()
		{
            HorrorHive hive = this.parent as HorrorHive;
			if ((hive == null || hive.active) && Find.TickManager.TicksGame >= this.nextHiveSpawnTick)
			{
                HorrorHive hive2;
				if (this.TrySpawnChildHive(false, out hive2))
				{
					hive2.nextPawnSpawnTick = Find.TickManager.TicksGame + Rand.Range(150, 350);
					Messages.Message("MessageHiveReproduced".Translate(), hive2, MessageTypeDefOf.NegativeEvent);
				}
				else
				{
					this.CalculateNextHiveSpawnTick();
				}
			}
		}
		public override string CompInspectStringExtra()
		{
			string text = null;
			if (this.CanSpawnChildHive)
			{
				text = text + "HiveReproducesIn".Translate() + ": " + (this.nextHiveSpawnTick - Find.TickManager.TicksGame).ToStringTicksToPeriod(true, false, true);
			}
			return text;
		}
		public void CalculateNextHiveSpawnTick()
		{
			Room room = this.parent.GetRoom(RegionType.Set_Passable);
			int num = 0;
			int num2 = GenRadial.NumCellsInRadius(9f);
			for (int i = 0; i < num2; i++)
			{
				IntVec3 intVec = this.parent.Position + GenRadial.RadialPattern[i];
				if (intVec.InBounds(this.parent.Map))
				{
					if (intVec.GetRoom(this.parent.Map, RegionType.Set_Passable) == room)
					{
						if (intVec.GetThingList(this.parent.Map).Any((Thing t) => t is Hive))
						{
							num++;
						}
					}
				}
			}
			float num3 = GenMath.LerpDouble(0f, 7f, 1f, 0.35f, (float)Mathf.Clamp(num, 0, 7));
			this.nextHiveSpawnTick = Find.TickManager.TicksGame + (int)(this.Props.HiveSpawnIntervalDays.RandomInRange * 60000f / (num3 * Find.Storyteller.difficulty.enemyReproductionRateFactor));
		}
        public bool TrySpawnChildHive(bool ignoreRoofedRequirement, out HorrorHive newHive)
		{
			if (!this.CanSpawnChildHive)
			{
				newHive = null;
				return false;
			}
			IntVec3 invalid = IntVec3.Invalid;
			for (int i = 0; i < 3; i++)
			{
				float minDist = this.Props.HiveSpawnPreferredMinDist;
				if (i == 1)
				{
					minDist = 0f;
				}
				else
				{
					if (i == 2)
					{
						newHive = null;
						return false;
					}
				}
				if (CellFinder.TryFindRandomReachableCellNear(this.parent.Position, this.parent.Map, this.Props.HiveSpawnRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), (IntVec3 c) => this.CanSpawnHiveAt(c, minDist, ignoreRoofedRequirement), null, out invalid, 999999))
				{
					break;
				}
			}
            newHive = (HorrorHive)GenSpawn.Spawn(this.parent.def, invalid, this.parent.Map);
			if (newHive.Faction != this.parent.Faction)
			{
				newHive.SetFaction(this.parent.Faction, null);
			}
            HorrorHive hive = this.parent as HorrorHive;
			if (hive != null)
			{
				newHive.active = hive.active;
			}
			this.CalculateNextHiveSpawnTick();
			return true;
		}
		private bool CanSpawnHiveAt(IntVec3 c, float minDist, bool ignoreRoofedRequirement)
		{
			if ((!ignoreRoofedRequirement && !c.Roofed(this.parent.Map)) || !c.Standable(this.parent.Map) || (minDist != 0f && (float)c.DistanceToSquared(this.parent.Position) < minDist * minDist))
			{
				return false;
			}
			for (int i = 0; i < 8; i++)
			{
				IntVec3 c2 = c + GenAdj.AdjacentCells[i];
				if (c2.InBounds(this.parent.Map))
				{
					List<Thing> thingList = c2.GetThingList(this.parent.Map);
					for (int j = 0; j < thingList.Count; j++)
					{
                        if (thingList[j] is HorrorHive)
						{
							return false;
						}
					}
				}
			}
			List<Thing> thingList2 = c.GetThingList(this.parent.Map);
			for (int k = 0; k < thingList2.Count; k++)
			{
				Thing thing = thingList2[k];
				if ((thing.def.category == ThingCategory.Item || thing.def.category == ThingCategory.Building) && GenSpawn.SpawningWipes(this.parent.def, thing.def))
				{
					return false;
				}
			}
			return true;
		}
		public override void PostExposeData()
		{
			Scribe_Values.Look<int>(ref this.nextHiveSpawnTick, "nextHiveSpawnTick", 0, false);
		}
	}
}
