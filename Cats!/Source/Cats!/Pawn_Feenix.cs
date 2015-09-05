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
    public class Feenix : Pawn
    {
        public override void Tick()
        {
            base.Tick();

            // do stuff every second
            if (this.IsHashIntervalTick(60))
            {
                // gain food from fires
                IEnumerable<Thing> fires = from f in Find.ListerThings.ThingsOfDef(ThingDefOf.Fire)
                                           where f.Position.InHorDistOf(this.Position, 4)
                                           select f;

                foreach (Fire f in fires)
                {
                    // TODO: gain health from fires
                    this.needs.food.CurLevel += f.fireSize / 25000f * 60f;
                }

                if (this.needs.food.CurLevel > 1) this.needs.food.CurLevel = 1;

                // push heat to the room (now handled with comp in Race.xml
                // float energy = 50 * this.health.summaryHealth.SummaryHealthPercent * this.HealthScale;
                // GenTemperature.PushHeat(this.Position, energy);

                // TODO: get hurt by rain
            }
        }
    }
}
