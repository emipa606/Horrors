using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;
using RimWorld;
using Verse.AI;

namespace Horrors
{
    public class Building_Burrow : Building_CryptosleepCasket, IOpenable, IThingHolder
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            if (this.Faction == null)
            {
                this.SetFactionDirect(FactionUtility.DefaultFactionFrom(FactionDef.Named("Horrors")));
            }

            if (base.Faction != null && base.Faction.IsPlayer)
            {
                this.contentsKnown = true;
            }
        }

        bool TryFindNewTarget()
        {
            
            Pawn target = (Pawn)GenClosest.ClosestThingReachable(this.Position, this.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 10, new Predicate<Thing>(this.IsValidTarget), null, 0, -1, false, RegionType.Set_Passable, false);
            if (target != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsValidTarget(Thing t)
        {
            Pawn pawn = t as Pawn;
            if (pawn != null)
            {
                if (pawn.RaceProps.Animal || pawn.Faction == this.Faction)
                {
                    return false;
                }
            }
            return true;
        }

        private Lord CreateNewLord()
        {
            return LordMaker.MakeNewLord(base.Faction, new LordJob_DefendPoint(base.Position), base.Map, null);
        }

        public override void TickRare()
        {
            base.TickRare();
            this.innerContainer.ThingOwnerTickRare(true);

            if (TryFindNewTarget())
            {
                
                if (this.ContainedThing == null)
                {
                    return;
                }
                else
                {
                    EjectContents();
                }
            }

            if (this.ContainedThing == null)
            {
                DecayBurrow();
            }
            else 
            {
                if (this.HitPoints > this.MaxHitPoints)
                {
                    this.HitPoints += 50;
                }
                else
                {
                    this.HitPoints = this.MaxHitPoints;
                }
            }
        }   

        public void DecayBurrow()
        {
            this.TakeDamage(new DamageInfo(DamageDefOf.Deterioration, 50));
        }

		public Building_Burrow()
		{
			this.innerContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
		}

		public override void ExposeData()
		{
			base.ExposeData();
		}
		public override bool ClaimableBy(Faction fac)
		{
			if (this.innerContainer.Any)
			{
				for (int i = 0; i < this.innerContainer.Count; i++)
				{
					if (this.innerContainer[i].Faction == fac)
					{
						return true;
					}
				}
				return false;
			}
			return base.ClaimableBy(fac);
		}

		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			if (this.innerContainer.Count > 0 && (mode == DestroyMode.Deconstruct || mode == DestroyMode.KillFinalize))
			{
				if (mode != DestroyMode.Deconstruct)
				{
					List<Pawn> list = new List<Pawn>();
					foreach (Thing current in (IEnumerable<Thing>)this.innerContainer)
					{
						Pawn pawn = current as Pawn;
						if (pawn != null)
						{
							list.Add(pawn);
						}
					}
					foreach (Pawn current2 in list)
					{
						HealthUtility.DamageUntilDowned(current2);
					}
				}
				this.EjectContents();
			}
			this.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
			base.Destroy(mode);
		}

        public override void EjectContents()
        {
            foreach (Thing current in (IEnumerable<Thing>)this.innerContainer)
            {
                Pawn pawn = current as Pawn;
                if (pawn != null)
                {
                    PawnComponentsUtility.AddComponentsForSpawn(pawn);
                }
            }
            if (!base.Destroyed)
            {
                SoundDef.Named("BurrowSFX").PlayOneShot(SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), MaintenanceType.None));
            }

            this.innerContainer.TryDropAll(this.InteractionCell, base.Map, ThingPlaceMode.Near);
            this.contentsKnown = true;
        }

        public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if (base.TryAcceptThing(thing, allowSpecialEffects))
            {
                if (allowSpecialEffects)
                {
                    SoundDef.Named("BurrowSFX").PlayOneShot(new TargetInfo(base.Position, base.Map, false));
                }
                return true;
            }
            return false;
        }
    }
}
