﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
using Verse.AI.Group;
using RimWorld;

namespace Horrors
{
	public class HorrorDen : ThingWithComps
	{
		private const int InitialPawnSpawnDelay = 420;
		private const int PawnSpawnRadius = 4;
		private const float MaxSpawnedPawnsPoints = 800f;
		private const int InitialPawnsPoints = 200;
		public bool active = true;
		public int nextPawnSpawnTick = -1;
		private List<Pawn> spawnedPawns = new List<Pawn>();
		private int ticksToSpawnInitialPawns = -1;
		private static readonly FloatRange PawnSpawnIntervalDays = new FloatRange(0.85f, 1.1f);
		public static readonly string MemoAttackedByEnemy = "HiveAttacked";
		public static readonly string MemoDestroyed = "HiveDestroyed";
		public static readonly string MemoBurnedBadly = "HiveBurnedBadly";
		private Lord Lord
		{
			get
			{
				Predicate<Pawn> hasDefendHiveLord = delegate(Pawn x)
				{
					Lord lord = x.GetLord();
					return lord != null && lord.LordJob is LordJob_DefendAndExpandHive;
				};
				Pawn foundPawn = this.spawnedPawns.Find(hasDefendHiveLord);
				if (base.Spawned)
				{
					if (foundPawn == null)
					{
						RegionTraverser.BreadthFirstTraverse(this.GetRegion(RegionType.Set_Passable), (Region from, Region to) => true, delegate(Region r)
						{
							List<Thing> list = r.ListerThings.ThingsOfDef(ThingDef.Named("HorrorDen"));
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i] != this)
								{
									if (list[i].Faction == this.Faction)
									{
										foundPawn = ((HorrorDen)list[i]).spawnedPawns.Find(hasDefendHiveLord);
										if (foundPawn != null)
										{
											return true;
										}
									}
								}
							}
							return false;
						}, 20, RegionType.Set_Passable);
					}
					if (foundPawn != null)
					{
						return foundPawn.GetLord();
					}
				}
				return null;
			}
		}
		private float SpawnedPawnsPoints
		{
			get
			{
				this.FilterOutUnspawnedPawns();
				float num = 0f;
				for (int i = 0; i < this.spawnedPawns.Count; i++)
				{
					num += this.spawnedPawns[i].kindDef.combatPower;
				}
				return num;
			}
		}
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (base.Faction == null)
			{
				this.SetFaction(Find.FactionManager.FirstFactionOfDef(FactionDef.Named("Horrors")));
			}
			if (!respawningAfterLoad)
			{
				this.ticksToSpawnInitialPawns = 420;
			}
		}
		private void SpawnInitialPawnsNow()
		{
			this.ticksToSpawnInitialPawns = -1;
			while (this.SpawnedPawnsPoints < 200f)
			{
				Pawn pawn;
				if (!this.TrySpawnPawn(out pawn))
				{
					return;
				}
			}
			this.CalculateNextPawnSpawnTick();
		}
		public override void TickRare()
		{
			base.TickRare();
			if (base.Spawned)
			{
				this.FilterOutUnspawnedPawns();
				if (!this.active && !base.Position.Fogged(base.Map))
				{
					this.Activate();
				}
				if (this.active)
				{
					if (this.ticksToSpawnInitialPawns > 0)
					{
						this.ticksToSpawnInitialPawns -= 250;
						if (this.ticksToSpawnInitialPawns <= 0)
						{
							this.SpawnInitialPawnsNow();
						}
					}
					if (Find.TickManager.TicksGame >= this.nextPawnSpawnTick)
					{
						if (this.SpawnedPawnsPoints < 500f)
						{
							Pawn pawn;
							bool flag = this.TrySpawnPawn(out pawn);
							if (flag && pawn.caller != null)
							{
								pawn.caller.DoCall();
							}
						}
						this.CalculateNextPawnSpawnTick();
					}
				}
			}
		}
		public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
		{
			Map map = base.Map;
			base.DeSpawn();
			List<Lord> lords = map.lordManager.lords;
			for (int i = 0; i < lords.Count; i++)
			{
				lords[i].ReceiveMemo(HorrorDen.MemoDestroyed);
			}
		}
		public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			if (dinfo.Def.ExternalViolenceFor(this) && dinfo.Instigator != null && dinfo.Instigator.Faction != null)
			{
				if (this.ticksToSpawnInitialPawns > 0)
				{
					this.SpawnInitialPawnsNow();
				}
				Lord lord = this.Lord;
				if (lord != null)
				{
                    lord.ReceiveMemo(HorrorDen.MemoAttackedByEnemy);
				}
			}
			if (dinfo.Def == DamageDefOf.Flame && (float)this.HitPoints < (float)base.MaxHitPoints * 0.3f)
			{
				Lord lord2 = this.Lord;
				if (lord2 != null)
				{
                    lord2.ReceiveMemo(HorrorDen.MemoBurnedBadly);
				}
			}
			base.PostApplyDamage(dinfo, totalDamageDealt);
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.active, "active", false, false);
			Scribe_Values.Look<int>(ref this.nextPawnSpawnTick, "nextPawnSpawnTick", 0, false);
			Scribe_Collections.Look<Pawn>(ref this.spawnedPawns, "spawnedPawns", LookMode.Reference, new object[0]);
			Scribe_Values.Look<int>(ref this.ticksToSpawnInitialPawns, "ticksToSpawnInitialPawns", 0, false);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				this.spawnedPawns.RemoveAll((Pawn x) => x == null);
			}
		}
		private void Activate()
		{
			this.active = true;
			this.nextPawnSpawnTick = Find.TickManager.TicksGame + Rand.Range(200, 400);
			CompSpawnerHorrorDen comp = base.GetComp<CompSpawnerHorrorDen>();
			if (comp != null)
			{
				comp.CalculateNextHiveSpawnTick();
			}
		}
		private void CalculateNextPawnSpawnTick()
		{
			float num = GenMath.LerpDouble(0f, 5f, 1f, 0.5f, (float)this.spawnedPawns.Count);
			this.nextPawnSpawnTick = Find.TickManager.TicksGame + (int)(HorrorDen.PawnSpawnIntervalDays.RandomInRange * 60000f / (num * Find.Storyteller.difficulty.enemyReproductionRateFactor));
		}
		private void FilterOutUnspawnedPawns()
		{
			for (int i = this.spawnedPawns.Count - 1; i >= 0; i--)
			{
				if (!this.spawnedPawns[i].Spawned)
				{
					this.spawnedPawns.RemoveAt(i);
				}
			}
		}
		private bool TrySpawnPawn(out Pawn pawn)
		{
			List<PawnKindDef> list = new List<PawnKindDef>();
            list.Add(PawnKindDef.Named("Terrorworm"));
            list.Add(PawnKindDef.Named("Visceral"));
			float curPoints = this.SpawnedPawnsPoints;
			IEnumerable<PawnKindDef> source = 
				from x in list
				where curPoints + x.combatPower <= 500f
				select x;
			PawnKindDef kindDef;
			if (!source.TryRandomElement(out kindDef))
			{
				pawn = null;
				return false;
			}
			pawn = PawnGenerator.GeneratePawn(kindDef, this.Faction);
            PawnUtility.TrySpawnHatchedOrBornPawn(pawn, this);
			//GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(base.Position, base.Map, 4, null), base.Map);
			this.spawnedPawns.Add(pawn);
			Lord lord = this.Lord;
			if (lord == null)
			{
				lord = this.CreateNewLord();
			}
			lord.AddPawn(pawn);
			return true;
		}
		public override bool PreventPlayerSellingThingsNearby(out string reason)
		{
			if (this.spawnedPawns.Count > 0)
			{
				if (this.spawnedPawns.Any((Pawn p) => !p.Downed))
				{
					reason = this.def.label;
					return true;
				}
			}
			reason = null;
			return false;
		}
		private Lord CreateNewLord()
		{
			return LordMaker.MakeNewLord(base.Faction, new LordJob_DefendAndExpandHive(), base.Map, null);
		}

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
            {
                yield return g;
            }
        }
	}
}