using System;
using Verse;

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
                    GenTemperature.PushHeat(this.parent.Position, this.props.coldPerSecond);
                }

                SnowUtility.AddSnowRadial(this.parent.Position, this.props.snowRadius, this.props.snowDepth);
            }
        }
    }
}
