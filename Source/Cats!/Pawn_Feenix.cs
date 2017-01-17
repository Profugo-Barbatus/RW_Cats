using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;              

namespace Fluffy
{
    public class Feenix : Pawn
    {
        public override void Tick()
        {
            base.Tick();

            // do stuff every second
            if (this.IsHashIntervalTick(60))
            {
                // gain food from fires
                IEnumerable<Thing> fires = from f in Map.listerThings.ThingsOfDef(ThingDefOf.Fire)
                                           where f.Position.InHorDistOf(Position, 4)
                                           select f;

                foreach (Fire f in fires)
                {
                    // TODO: gain health from fires
                    needs.food.CurLevel += f.fireSize / 25000f * 60f;
                }

                if (needs.food.CurLevel > 1) needs.food.CurLevel = 1;

            }
        }
        
        public override void TickRare()
        {
            base.TickRare();
            if ( Map.weatherManager.RainRate > .8 && !Position.Roofed( Map ) )
                health.AddHediff( HediffDefOf.Burn );

            if ( Map.glowGrid.GameGlowAt( Position ) > .8 && !health.hediffSet.hediffs.NullOrEmpty() )
                health.RemoveHediff( health.hediffSet.hediffs.RandomElement() );
        }
    }
}
