using System.Collections.Generic;
using RimWorld;
using Verse;  
using Verse.AI;  

namespace Fluffy
{
    public class JobDriver_DrawEnergyFromFire : JobDriver
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.Goto( TargetIndex.A, PathEndMode.Touch );
            yield return Toils_Goto.GotoCell( TargetA.Cell.RandomAdjacentCell8Way(), PathEndMode.Touch );
            yield return Toils_Goto.GotoCell( TargetA.Cell.RandomAdjacentCell8Way(), PathEndMode.Touch );
            yield return Toils_Goto.GotoCell( TargetA.Cell.RandomAdjacentCell8Way(), PathEndMode.Touch );
            yield return Toils_Goto.GotoCell( TargetA.Cell.RandomAdjacentCell8Way(), PathEndMode.Touch );
            yield break;
        }
    }
}
