using System.Collections.Generic;
using Verse; 
using Verse.AI;

namespace Fluffy
{
    public class JobDriver_Scratch : JobDriver
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            yield return Scratch(TargetThingA).FailOnDespawnedOrNull(TargetIndex.A);
            yield break;
        }
        
        private Toil Scratch(Thing target)
        {
            Toil toil = new Toil();
            
            toil.initAction = delegate
            {
                if (target.HitPoints > 10)
                    if (target.def.defName != "Fluffy_ScratchingPole")
                        target.HitPoints -= 2;
            };

            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = 300;
            return toil;
        }
    }
}
