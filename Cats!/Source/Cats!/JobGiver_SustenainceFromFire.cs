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
    public class JobGiver_SustenainceFromFire : ThinkNode_JobGiver
    {
        public override float GetPriority(Pawn pawn)
        {
            Need_Food food = pawn.needs.food;
            if (food == null)
            {
                return 0f;
            }
            if (food.CurLevel < food.ThreshHungry + 0.02f)
            {
                return 9.5f;
            }
            return 0f;
        }

        public static Thing FindClosestFlammableThing(Pawn pawn, float distance)
        {
            //Log.Message("Trying to find target at range: " + distance);
            IEnumerable<Thing> flammables = from t in Find.ListerThings.AllThings
                   where t.Position.InHorDistOf(pawn.Position, distance) && t.FlammableNow && !t.IsBurning()
                   select t;

            return GenClosest.ClosestThing_Global_Reachable(pawn.Position, flammables, PathEndMode.Touch, TraverseParms.For(pawn));
        }

        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            // find something closeish
            Job close = FindOrStartFire(pawn, 20f);
            if (close != null)
            {
                return close;
            }

            // widen the search
            Job far = FindOrStartFire(pawn, 100f);
            if (far != null)
            {
                return far;
            }

            return null;
        }

        private static Job FindOrStartFire(Pawn pawn, float dist)
        {
            // try finding existing fires.
            Thing f = GenClosest.ClosestThingReachable(pawn.Position, ThingRequest.ForDef(ThingDefOf.Fire), PathEndMode.Touch, TraverseParms.For(pawn), dist);

            if (f != null)
            {
                return new Job(DefDatabase<JobDef>.GetNamed("Fluffy_DrawEnergyFromFire", true), f);
            }
           
            // light a new fire
            Thing t = FindClosestFlammableThing(pawn, dist);
            if (t != null)
            {
                return new Job(DefDatabase<JobDef>.GetNamed("Fluffy_IgniteFire", true), t);
            }

            return null;
        }
    }
}
