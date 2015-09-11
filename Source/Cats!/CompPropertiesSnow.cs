using System;
using Verse;

namespace Fluffy
{
    public class CompPropertiesSnow : CompProperties
    {
        public float heatSuckMinTemperature = -10f;

        public float coldPerSecond = 50f;

        public float snowRadius = 5f;

        public float snowDepth = 1f;

        public CompPropertiesSnow()
        {
        }
        
        public CompPropertiesSnow(Type compClass) : base(compClass)
        {
        }
    }
}
