using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;         // Always needed
//using VerseBase;         // Material/Graphics handling functions are found here
using Verse;               // RimWorld universal objects are here (like 'Building')
using Verse.AI;            // Needed when you do something with the AI
//using Verse.Sound;       // Needed when you do something with Sound
//using Verse.Noise;       // Needed when you do something with Noises
using RimWorld;            // RimWorld specific functions are found here (like 'Building_Battery')
//using RimWorld.Planet;   // RimWorld specific functions for world creation
//using RimWorld.SquadAI;  // RimWorld specific functions for squad brains 

namespace Fluffy
{
    public class JobDriver_IgniteFire : JobDriver
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return this.GotoThing(this.TargetA.Cell, PathEndMode.Touch).FailOnDespawned(TargetIndex.A);
            yield return this.Ignite(this.TargetThingA);
            yield break;
        }

        public Toil GotoThing(IntVec3 cell, PathEndMode PathEndMode)
        {
            Toil toil = new Toil();
            TargetInfo target = new TargetInfo(cell);

            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                actor.pather.StartPath(target, PathEndMode);
            };

            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }

        private Toil Ignite(Thing target)
        {
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                Pawn feenix = toil.actor;
                if (target.FlammableNow && !target.IsBurning())
                {
                    if (target.CanEverAttachFire())
                    {
                        target.TryAttachFire(1f);
                    }
                    else
                    {
                        FireUtility.TryStartFireIn(target.Position, 1f);
                    }
                }
                else
                {
                    return;
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            return toil;
        }
    }
}
