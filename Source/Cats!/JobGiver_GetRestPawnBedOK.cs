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
    public class JobGiver_GetRestPawnBedOK : JobGiver_GetRest
    {
        public override float GetPriority(Pawn pawn)
        {
            Need_Rest rest = pawn.needs.rest;
            if (rest == null)
            {
                return 0f;
            }
            TimeAssignmentDef timeAssignmentDef;
            if (pawn.RaceProps.Humanlike)
            {
                timeAssignmentDef = ((pawn.timetable != null) ? pawn.timetable.CurrentAssignment : TimeAssignmentDefOf.Anything);
            }
            else
            {
                int hourOfDay = GenDate.HourOfDay;
                int napStart = Rand.RangeInclusive(10, 14);
                int napEnd = Rand.RangeInclusive(15, 18);
                if (hourOfDay < 6 || hourOfDay > 22 || hourOfDay > napStart && napEnd < 18)
                {
                    timeAssignmentDef = TimeAssignmentDefOf.Sleep;
                }
                else
                {
                    timeAssignmentDef = TimeAssignmentDefOf.Anything;
                }
            }
            float curLevel = rest.CurLevel;
            if (timeAssignmentDef == TimeAssignmentDefOf.Anything)
            {
                if (curLevel < 0.3f)
                {
                    return 8f;
                }
                return 0f;
            }
            else
            {
                if (timeAssignmentDef == TimeAssignmentDefOf.Work)
                {
                    return 0f;
                }
                if (timeAssignmentDef == TimeAssignmentDefOf.Joy)
                {
                    if (curLevel < 0.3f)
                    {
                        return 8f;
                    }
                    return 0f;
                }
                else
                {
                    if (timeAssignmentDef != TimeAssignmentDefOf.Sleep)
                    {
                        throw new NotImplementedException();
                    }
                    if (curLevel < 0.9f)
                    {
                        return 8f;
                    }
                    return 0f;
                }
            }
        }

        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            if (Find.TickManager.TicksGame - pawn.mindState.lastDisturbanceTick < 400)
            {
                return null;
            }
            // find your own bed
            Building_Bed catbed = RestUtility.FindBedFor(pawn);

            // sleep in your own bed 3/4 of the time
            if (catbed != null && Rand.Range(0f, 1f) > .75f)
            {
                return new Job(JobDefOf.LayDown, catbed);
            }

            // find all non-prisoner, non-medical beds
            IEnumerable<Building_Bed> pawnbeds = from b in Find.ListerBuildings.AllBuildingsColonistOfClass<Building_Bed>()
                                          where !b.ForPrisoners && !b.Medical && pawn.CanReach(b.Position, PathEndMode.OnCell, Danger.Some)
                                          select b;

            // find all buildings labeled as having a surface, specifically exclude core stove
            IEnumerable<Building> surfaces = from t in Find.ListerBuildings.allBuildingsColonist as List<Building>
                                               where t.def.surfaceType != SurfaceType.None
                                                 && pawn.CanReach(t.Position, PathEndMode.OnCell, Danger.Some)
                                                 && t.def.defName != "Stove"
                                               select t;


            // try to sleep on beds that occupy more than one space, and don't already have a cat on them.
            // if two cats decide to sleep on the same spot, they will sleep on top of eachother, but I'm going to call that a feature.
            // they also probably don't get restefficiency bonuses from better beds, but I'm not sure cats ever do.
            if (pawnbeds != null && pawnbeds.Any())
            {
                // all cells in all non-prisoner, non-medical beds and all surfaces
                IEnumerable<IntVec3> bedcells = pawnbeds.SelectMany(b => b.OccupiedRect().Cells);
                IEnumerable<IntVec3> surfacecells = surfaces.SelectMany(t => t.OccupiedRect().Cells);
                IEnumerable<IntVec3> cells = bedcells.Union(surfacecells);

                // all cells that will be occupied by colonists (beds only)
                IEnumerable<IntVec3> pawnspots = pawnbeds.Select(b => b.Position);

                // all bed cells that will not be occupied by colonists
                IEnumerable<IntVec3> catspots = cells.Except(pawnspots);

                // reachable, and not already occupied
                IEnumerable<IntVec3> viable = from c in catspots
                                              where pawn.CanReach(c, PathEndMode.OnCell, Danger.Some)
                                                && c.GetFirstThing(ThingDef.Named("Fluffy_DomesticCat")) == null
                                              select c;
                
                if (viable != null && viable.Any())
                {
                    return new Job(JobDefOf.LayDown, viable.RandomElement());
                }
            }

            // sleep on floor as last resort
            IntVec3 vec = CellFinder.RandomClosewalkCellNear(pawn.Position, 4);
            return new Job(JobDefOf.LayDown, vec);
        }
    }
}