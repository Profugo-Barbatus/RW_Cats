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
    public class IncidentWorker_FeenixPasses : IncidentWorker
    {
        public override bool TryExecute(IncidentParms parms)
        {
            IntVec3 intVec;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out intVec))
            {
                return false;
            }
            PawnKindDef feenix = PawnKindDef.Named("Fluffy_Feenix");
            IntVec3 invalid = IntVec3.Invalid;
            if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(intVec, 10f, out invalid))
            {
                invalid = IntVec3.Invalid;
            }
            Pawn pawn = null;
            IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, 10);
            pawn = PawnGenerator.GeneratePawn(feenix, null, false, 0);
            GenSpawn.Spawn(pawn, loc, Rot4.Random);
            pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + Rand.RangeInclusive(90000, 150000);
            if (invalid.IsValid)
            {
                pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(invalid, 10);
            }

            Find.LetterStack.ReceiveLetter("LetterLabelFeenixPasses".Translate(new object[]
            {
                feenix.label
            }).CapitalizeFirst(), "LetterFeenixPasses".Translate(new object[]
            {
                feenix.label
            }), LetterType.Good, pawn, null);
            return true;
        }
    }

    public class IncidentWorker_IcicatPasses : IncidentWorker_MakeMapCondition
    {
        public override bool TryExecute(IncidentParms parms)
        {



            IntVec3 intVec;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out intVec))
            {
                return false;
            }
            PawnKindDef feenix = PawnKindDef.Named("Fluffy_Icicat");
            IntVec3 invalid = IntVec3.Invalid;
            if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(intVec, 10f, out invalid))
            {
                invalid = IntVec3.Invalid;
            }
            Pawn pawn = null;
            IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, 10);
            pawn = PawnGenerator.GeneratePawn(feenix, null, false, 0);
            GenSpawn.Spawn(pawn, loc, Rot4.Random);

            int duration = Rand.RangeInclusive(90000, 150000);

            Find.MapConditionManager.RegisterCondition(MapConditionMaker.MakeCondition(this.def.mapCondition, duration));
            pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame +duration;
            if (invalid.IsValid)
            {
                pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(invalid, 10);
            }

            Find.LetterStack.ReceiveLetter("LetterLabelIcicatPasses".Translate(new object[]
            {
                feenix.label
            }).CapitalizeFirst(), "LetterIcicatPasses".Translate(new object[]
            {
                feenix.label
            }), LetterType.Good, pawn, null);
            return true;
        }
    }
}
