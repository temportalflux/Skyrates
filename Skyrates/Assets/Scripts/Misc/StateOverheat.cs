using System;
using UnityEngine;

namespace Skyrates.Misc
{

    [Serializable]
    public class StateOverheat : StateCooldown
    {
        
        public float HeatPerShot = 0.1f;
        public float RechargePerSecond = 0.01f;
        public float OverheatMin = 0.8f;

        public StateCooldown PerShot;
        
        private bool _isDisabled = false;

        public float PercentComplete
        {
            get { return 1.0f - this.PercentLoaded; }
            set { this.PercentLoaded = 1.0f - value; }
        }

        public override void Awake()
        {
            base.Awake();
            this._isDisabled = false;
            this.PercentComplete = 0.0f;
        }

        public override bool IsLoaded()
        {
            return !this._isDisabled && this.PerShot.IsLoaded();
        }

        public override void Unload()
        {
            this.PerShot.Unload();
            this.PercentLoaded -= this.HeatPerShot;
        }

        public override void Update(float deltaTime, float rateOfFireModifier)
        {
            this.PerShot.Update(deltaTime, rateOfFireModifier);
            
            float rechargePerSecond = !this._isDisabled ?
                this.RechargePerSecond :
                this.MaxDelaySeconds;
            float amount = deltaTime / rechargePerSecond;
            this.PercentLoaded = Mathf.Min(1.0f, this.PercentLoaded + amount);

            if (!this._isDisabled && this.PercentComplete >= this.OverheatMin)
            {
                this._isDisabled = true;
            }
            else if (this._isDisabled && this.PercentComplete <= 0.0f)
            {
                this._isDisabled = false;
            }
        }

    }

}
