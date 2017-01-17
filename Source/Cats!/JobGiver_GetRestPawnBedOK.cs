using System;
using System.Collections.Generic;
using System.Linq;

using Verse;
using Verse.AI;
using RimWorld; 

namespace Fluffy
{
    public class JobGiver_GetRestPawnBedOK : JobGiver_GetRest
    {
        public override float GetPriority(Pawn pawn)
        {
            Need_Rest restNeed = pawn.needs.rest;
            if (restNeed == null)
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
                int hourOfDay = GenLocalDate.HourOfDay( pawn );
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
            float restLevel = restNeed.CurLevel;
            if (timeAssignmentDef == TimeAssignmentDefOf.Anything)
            {
                if (restLevel < 0.3f)
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
                    if (restLevel < 0.3f)
                    {
                        return 8f;
                    }
                    return 0f;
                }
                else
                {
                    // should be only work, sleep and joy
                    if (timeAssignmentDef != TimeAssignmentDefOf.Sleep)
                    {
                        throw new NotImplementedException();
                    }

                    // neither work nor joy stuck, so we're in sleep (nap) mode.
                    if (restLevel < 0.9f)
                    {
                        return 8f;
                    }
                    return 0f;
                }
            }
        }

        protected override Job TryGiveJob(Pawn pawn)
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

            // sleep in owners bed 1/3 of the remainder
            if (Rand.Range(0f, 1f) > .33f && pawn.playerSettings.master != null)
            {
                Building_Bed masterbed = pawn.playerSettings.master.ownership.OwnedBed;
                if (masterbed != null)
                {
                    List<IntVec3> bedcells = masterbed.OccupiedRect().Cells.ToList();
                    bedcells.Remove(masterbed.Position);
                    return new Job(JobDefOf.LayDown, bedcells.RandomElement());
                }
            }

            // find all non-prisoner, non-medical beds
            IEnumerable<Building_Bed> pawnbeds = from b in pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Building_Bed>()
                                          where !b.ForPrisoners && !b.Medical && pawn.CanReach(b.Position, PathEndMode.OnCell, Danger.Some)
                                          select b;

            // find all buildings labeled as having a surface, specifically exclude core stove
            IEnumerable<Building> surfaces = from t in pawn.Map.listerBuildings.allBuildingsColonist 
                                               where t.def.surfaceType != SurfaceType.None
                                                 && pawn.CanReach(t.Position, PathEndMode.OnCell, Danger.Some)
                                                 && t.def.defName != "CookStove"
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
                                                && c.GetFirstThing( pawn.Map, ThingDef.Named("Fluffy_DomesticCat")) == null
                                              select c;
                
                if (viable != null && viable.Any())
                {
                    return new Job(JobDefOf.LayDown, viable.RandomElement());
                }
            }

            // sleep on floor as last resort
            IntVec3 vec = CellFinder.RandomClosewalkCellNear(pawn.Position, pawn.Map, 4);
            return new Job(JobDefOf.LayDown, vec);
        }
    }
}