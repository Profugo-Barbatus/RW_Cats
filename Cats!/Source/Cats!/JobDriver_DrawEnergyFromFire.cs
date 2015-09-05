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
    public class JobDriver_DrawEnergyFromFire : JobDriver
    {
        // locomotionUrgency doesnt actually work =/
        //protected LocomotionUrgency locomotionUrgency = LocomotionUrgency.Walk;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return this.GotoThing(this.TargetA.Cell, PathEndMode.Touch).FailOnDespawned(TargetIndex.A);
            yield return this.WanderAround(this.TargetA.Cell).FailOnDespawned(TargetIndex.A);
            yield return this.WanderAround(this.TargetA.Cell).FailOnDespawned(TargetIndex.A);
            yield return this.WanderAround(this.TargetA.Cell).FailOnDespawned(TargetIndex.A);
            yield return this.WanderAround(this.TargetA.Cell).FailOnDespawned(TargetIndex.A);
            yield break;
        }

        public Toil GotoThing(IntVec3 cell, PathEndMode PathEndMode)
        {
            //Log.Message("Going to.");
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

        public Toil WanderAround(IntVec3 cell)
        {
            //Log.Message("Wandering.");
            Toil toil = new Toil();
            TargetInfo target = new TargetInfo(cell);
            TargetInfo target2 = null;
            TargetInfo target3 = null;
            Pawn pawn = this.pawn;

            //if (pawn == null) Log.Message("pawn is null");

            int attempt = 0;

            //Log.Message("Pawn: " + pawn.ToString());
            //Log.Message("Main target: " + target.ToString());

            while (attempt < 20)
            {
                target2 = new TargetInfo(target.Cell.RandomAdjacentCell8Way());
                //Log.Message("Checking potential target: " + target2.ToString() + " attempt " + attempt);
                if (target2 != null && pawn.CanReach(target2, PathEndMode.OnCell, Danger.Deadly))
                {
                    target3 = target2;
                    break;
                }
                attempt += 1;
            }

            //Log.Message("Found target: " + target3.ToString());

            if (target3 == null) return null;

            toil.initAction = delegate
            {
                pawn.pather.StartPath(target3, PathEndMode.OnCell);
            };

            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }
    }
}
