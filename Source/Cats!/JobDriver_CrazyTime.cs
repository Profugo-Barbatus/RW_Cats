using System.Collections.Generic;
using Verse;          
using Verse.AI;     

namespace Fluffy
{
    public class JobDriver_CrazyTime : JobDriver
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            int num = Rand.RangeInclusive(3, 8);

            for (int i = 0; i < num; i++)
            {
                yield return CrazyTime();
            }
            
            yield break;
        }

        public Toil CrazyTime()
        {
            Toil toil = new Toil();
            IntVec3 target = CellFinder.RandomClosewalkCellNear(pawn.Position, pawn.Map, 10);

            toil.initAction = delegate
            {
                CurJob.locomotionUrgency = LocomotionUrgency.Sprint;
                pawn.pather.StartPath(target, PathEndMode.OnCell);
            };

            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }
    }
}
