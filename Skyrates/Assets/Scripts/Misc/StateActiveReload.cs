using System;
using UnityEngine;

namespace Skyrates.Misc
{

    [Serializable]
    public class StateActiveReload : StateCooldown
    {

        [Range(0.0f, 1.0f)]
        public float PercentStart = 0.2f;

        [Range(0.0f, 1.0f)]
        public float PercentEnd = 0.3f;

        /// <summary>
        /// If the cannon can be actively reloaded.
        /// Can be triggered after starting reload to auto complete if within a specific range.
        /// </summary>
        private bool _canActiveReload = false;

        /// <summary>
        /// If the cannon is in the process of reloading.
        /// </summary>
        private bool _isLoading = false;
        
        public override void Update(float deltaTime)
        {
            if (!this._isLoading)
                return;
            base.Update(deltaTime);
        }

        public override void Unload()
        {
            base.Unload();
            this._canActiveReload = true;
            this._isLoading = false;
        }
        
        public override void OnLoaded()
        {
            base.OnLoaded();
            this._canActiveReload = false;
            this._isLoading = false;
        }

        public void TryReload()
        {
            if (!this._isLoading)
            {
                this._isLoading = true;
            }
            else if (this._canActiveReload)
            {
                if (this.PercentLoaded >= this.PercentStart &&
                    this.PercentLoaded <= this.PercentEnd)
                {
                    this.Load();
                }
                else
                {
                    this._canActiveReload = false;
                }
            }
        }
    }

}
