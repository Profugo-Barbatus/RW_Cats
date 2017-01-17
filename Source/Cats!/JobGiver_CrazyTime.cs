using Verse;
using Verse.AI;  

namespace Fluffy
{
    public class JobGiver_CrazyTime : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob( Pawn pawn )
        {
#if DEBUG
            Log.Message( "Going crazy!" );
#endif
            return new Job( DefDatabase<JobDef>.GetNamed( "Fluffy_CrazyTime", true ) );
        }
    }
}
