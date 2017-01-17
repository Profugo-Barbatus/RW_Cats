using System.Collections.Generic;
using System.Linq;
using Verse;  
using Verse.AI;


namespace Fluffy
{
    public class JobGiver_Scratch : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            // Log.Message("Scratching the itch.");

            // try finding scratching poles.
            Thing target = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDef.Named("Fluffy_ScratchingPole")), PathEndMode.Touch, TraverseParms.For(pawn), 20);
            if (target != null)
            {
                // Log.Message("Scratching pole found.");
                return new Job(DefDatabase<JobDef>.GetNamed("Fluffy_Scratch", true), target);
            }

            // potential other targets.
            IEnumerable<Building> targets = from t in pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Building>()
                                            where t.def.useHitPoints
                                                && pawn.CanReach(t, PathEndMode.Touch, Danger.Some)
                                            select t;

            if (!targets.Any())
                return null;

            target = targets.RandomElement();
            return new Job(DefDatabase<JobDef>.GetNamed("Fluffy_Scratch", true), target);
        }
    }
}
