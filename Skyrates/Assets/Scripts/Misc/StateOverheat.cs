using System;
using UnityEngine;

namespace Skyrates.Misc
{

    [Serializable]
    public class StateOverheat : StateCooldown
    {
        
        public float HeatPerShot = 0.05f;

        public override bool IsLoaded()
        {
            return this.PercentLoaded > 0.0f;
        }

        public override void Unload()
        {
            this.PercentLoaded = Mathf.Max(0.0f, this.PercentLoaded - this.HeatPerShot);
        }

    }

}
