using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace Horrors;

public class Building_Burrow : Building_CryptosleepCasket
{
    public Building_Burrow()
    {
        innerContainer = new ThingOwner<Thing>(this, false);
    }

    public override bool ClaimableBy(Faction fac, StringBuilder reason = null)
    {
        if (!innerContainer.Any)
        {
            return base.ClaimableBy(fac, reason);
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
        if (innerContainer.Count > 0 && mode is DestroyMode.Deconstruct or DestroyMode.KillFinalize)
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

        if (Faction is { IsPlayer: true })
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
        if (t is not Pawn pawn)
        {
            return true;
        }

        return !pawn.RaceProps.Animal && pawn.Faction != Faction;
    }

    private bool TryFindNewTarget()
    {
        var target = (Pawn)GenClosest.ClosestThingReachable(Position, Map,
            ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell,
            TraverseParms.For(TraverseMode.NoPassClosedDoors), 10, IsValidTarget);
        return target != null;
    }
}