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
    public class JobDriver_Scratch : JobDriver
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return this.GotoThing(this.TargetA.Cell, PathEndMode.Touch).FailOnDespawned(TargetIndex.A);
            yield return this.Scratch(this.TargetThingA).FailOnDespawnedOrForbidden(TargetIndex.A);
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

        private Toil Scratch(Thing target)
        {
            Toil toil = new Toil();
            
            //Log.Message("Pawn: " + pawn.ToString() + ", target: " + target.ToString());

            toil.initAction = delegate
            {
                Pawn pawn = this.pawn;

                if (target.HitPoints > 10)
                {
                    if (!(target.def.defName == "Fluffy_ScratchingPole"))
                    {
                        target.HitPoints -= 2;
                    }
                }
            };

            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = 300;
            return toil;
        }
    }
}
