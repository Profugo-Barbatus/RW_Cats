using System;
using Verse;
using RimWorld;

namespace Fluffy
{
	public class CompSnow : ThingComp
	{
		public new CompPropertiesSnow props;

        public override void CompTick()
        {
            base.CompTick();
            if (this.parent.IsHashIntervalTick(60)) {

                if (this.parent.Position.GetTemperature() > this.props.heatSuckMinTemperature)
                {
                    GenTemperature.PushHeat(this.parent.Position, - this.props.coldPerSecond);
                }

                SnowUtility.AddSnowRadial(this.parent.Position, this.props.snowRadius, this.props.snowDepth);
            }
        }

        public override void Initialize(CompProperties vprops)
        {
            base.Initialize(vprops);
            this.props = (vprops as CompPropertiesSnow);
            if (this.props == null)
            {
                Log.Warning("Props went horribly wrong.");
                this.props.snowDepth = 1f;
                this.props.snowRadius = 5f;
                this.props.heatSuckMinTemperature = -10f;
                this.props.coldPerSecond = 200f;
            }
        }
    }
}
