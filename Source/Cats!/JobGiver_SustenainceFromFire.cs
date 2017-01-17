using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using RimWorld; 


namespace Fluffy
{
    public class JobGiver_SustenainceFromFire : ThinkNode_JobGiver
    {
        public override float GetPriority(Pawn pawn)
        {
            if ( pawn?.needs?.food?.CurLevel < pawn?.needs?.food?.PercentageThreshHungry + 0.02f)
                return 9.5f;
            return 0f;
        }

        public static Thing FindClosestFlammableThing(Pawn pawn, float distance)
        {
            //Log.Message("Trying to find target at range: " + distance);
            IEnumerable<Thing> flammables = from t in pawn.Map.listerThings.AllThings
                   where t.Position.InHorDistOf(pawn.Position, distance) && t.FlammableNow && !t.IsBurning()
                   select t;
            
            return GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, flammables, PathEndMode.Touch, TraverseParms.For(pawn));
        }

        protected override Job TryGiveJob(Pawn pawn)
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
            Thing f = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.Fire), PathEndMode.Touch, TraverseParms.For(pawn), dist);

            if (f != null)
                return new Job(DefDatabase<JobDef>.GetNamed("Fluffy_DrawEnergyFromFire" ), f);
           
            // light a new fire
            Thing t = FindClosestFlammableThing(pawn, dist);
            if (t != null)
                return new Job(DefDatabase<JobDef>.GetNamed("Fluffy_IgniteFire"), t);

            return null;
        }
    }
}
