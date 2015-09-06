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
    public class JobGiver_Scratch : ThinkNode_JobGiver
    {
        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            // Log.Message("Scratching the itch.");

            // try finding scratching poles.
            Thing target = GenClosest.ClosestThingReachable(pawn.Position, ThingRequest.ForDef(ThingDef.Named("Fluffy_ScratchingPole")), PathEndMode.Touch, TraverseParms.For(pawn), 20);
            if (target != null)
            {
                // Log.Message("Scratching pole found.");
                return new Job(DefDatabase<JobDef>.GetNamed("Fluffy_Scratch", true), target);
            }

            // potential other targets.
            IEnumerable<Building> targets = from t in Find.ListerBuildings.AllBuildingsColonistOfClass<Building>()
                                            where t.def.useHitPoints
                                                && pawn.CanReach(new TargetInfo(t), PathEndMode.Touch, Danger.Some)
                                            select t;

            if (!targets.Any())
            {
                return null;
            }

            target = targets.RandomElement();
            // Log.Message("Furniture found.");
            return new Job(DefDatabase<JobDef>.GetNamed("Fluffy_Scratch", true), target);
        }
    }
}
