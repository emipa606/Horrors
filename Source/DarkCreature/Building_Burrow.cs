using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace Horrors
{
    public class Building_Burrow : Building_CryptosleepCasket
    {
        public Building_Burrow()
        {
            innerContainer = new ThingOwner<Thing>(this, false);
        }

        public override bool ClaimableBy(Faction fac)
        {
            if (!innerContainer.Any)
            {
                return base.ClaimableBy(fac);
            }

            foreach (var thing in innerContainer)
            {
                if (thing.Faction == fac)
                {
                    return true;
                }
            }

            return false;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (innerContainer.Count > 0 && (mode == DestroyMode.Deconstruct || mode == DestroyMode.KillFinalize))
            {
                if (mode != DestroyMode.Deconstruct)
                {
                    var list = new List<Pawn>();
                    foreach (var current in innerContainer)
                    {
                        if (current is Pawn pawn)
                        {
                            list.Add(pawn);
                        }
                    }

                    foreach (var current2 in list)
                    {
                        HealthUtility.DamageUntilDowned(current2);
                    }
                }

                EjectContents();
            }

            innerContainer.ClearAndDestroyContents();
            base.Destroy(mode);
        }

        public override void EjectContents()
        {
            foreach (var current in innerContainer)
            {
                if (current is Pawn pawn)
                {
                    PawnComponentsUtility.AddComponentsForSpawn(pawn);
                }
            }

            if (!Destroyed)
            {
                SoundDef.Named("BurrowSFX").PlayOneShot(SoundInfo.InMap(new TargetInfo(Position, Map)));
            }

            innerContainer.TryDropAll(InteractionCell, Map, ThingPlaceMode.Near);
            contentsKnown = true;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            if (Faction == null)
            {
                SetFactionDirect(FactionUtility.DefaultFactionFrom(FactionDef.Named("Horrors")));
            }

            if (Faction != null && Faction.IsPlayer)
            {
                contentsKnown = true;
            }
        }

        public override void TickRare()
        {
            base.TickRare();
            innerContainer.ThingOwnerTickRare();

            if (TryFindNewTarget())
            {
                if (ContainedThing == null)
                {
                    return;
                }

                EjectContents();
            }

            if (ContainedThing == null)
            {
                DecayBurrow();
            }
            else
            {
                if (HitPoints > MaxHitPoints)
                {
                    HitPoints += 50;
                }
                else
                {
                    HitPoints = MaxHitPoints;
                }
            }
        }

        public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if (!base.TryAcceptThing(thing, allowSpecialEffects))
            {
                return false;
            }

            if (allowSpecialEffects)
            {
                SoundDef.Named("BurrowSFX").PlayOneShot(new TargetInfo(Position, Map));
            }

            return true;
        }

        private Lord CreateNewLord()
        {
            return LordMaker.MakeNewLord(Faction, new LordJob_DefendPoint(Position), Map);
        }

        private void DecayBurrow()
        {
            TakeDamage(new DamageInfo(DamageDefOf.Deterioration, 50));
        }

        private bool IsValidTarget(Thing t)
        {
            // ReSharper disable once InvertIf
            if (t is Pawn pawn)
            {
                if (pawn.RaceProps.Animal || pawn.Faction == Faction)
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryFindNewTarget()
        {
            var target = (Pawn) GenClosest.ClosestThingReachable(Position, Map,
                ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell,
                TraverseParms.For(TraverseMode.NoPassClosedDoors), 10, IsValidTarget);
            if (target != null)
            {
                return true;
            }

            return false;
        }
    }
}