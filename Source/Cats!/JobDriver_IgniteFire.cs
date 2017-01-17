using System.Collections.Generic;
using Verse;               
using Verse.AI;           
using RimWorld;          

namespace Fluffy
{
    public class JobDriver_IgniteFire : JobDriver
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            yield return Ignite(TargetThingA);
            yield break;
        }
        

        private Toil Ignite(Thing target)
        {
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                Pawn feenix = pawn;
                if (target.FlammableNow && !target.IsBurning())
                {
                    if (target.CanEverAttachFire())
                    {
                        target.TryAttachFire(1f);
                    }
                    else
                    {
                        FireUtility.TryStartFireIn(target.Position, target.Map, 1f);
                    }
                    PawnUtility.ForceWait(feenix, 250, target);
                }
                else
                {
                    return;
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = 250;
            return toil;
        }
    }
}
