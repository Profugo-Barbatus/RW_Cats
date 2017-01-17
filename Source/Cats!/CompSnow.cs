using System;
using Verse;
using RimWorld;

namespace Fluffy
{
	public class CompSnow : ThingComp
	{
        // we need to replace props as the base object doesn't have fields for our properties.
		public new CompPropertiesSnow props;

	    public float Temperature => GenTemperature.GetTemperatureForCell( parent.Position, parent.MapHeld );

        public override void CompTick()
        {
            base.CompTick();
            if (parent.IsHashIntervalTick(60)) {
                
                if ( Temperature > props.heatSuckMinTemperature)
                {
                    GenTemperature.PushHeat(parent.Position, parent.MapHeld, - props.coldPerSecond);
                }

                SnowUtility.AddSnowRadial(parent.Position, parent.Map, props.snowRadius, props.snowDepth);
            }
        }

        public override void Initialize(CompProperties vprops)
        {
            base.Initialize(vprops);
            props = (vprops as CompPropertiesSnow);
            if (props == null)
            {
                Log.Warning("Props went horribly wrong.");
                props.snowDepth = 1f;
                props.snowRadius = 5f;
                props.heatSuckMinTemperature = -10f;
                props.coldPerSecond = 200f;
            }
        }
    }
}
