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
        public bool IsLoading { get; private set; }

        public override void Awake()
        {
            base.Awake();
            this.IsLoading = false;
            this._canActiveReload = false;
        }

        public override void Update(float deltaTime)
        {
            if (!this.IsLoading)
                return;
            base.Update(deltaTime);
        }

        public override void Unload()
        {
            base.Unload();
            this._canActiveReload = true;
            this.IsLoading = false;
        }
        
        public override void OnLoaded()
        {
            base.OnLoaded();
            this._canActiveReload = false;
            this.IsLoading = false;
        }

        public void TryReload()
        {
            if (!this.IsLoading)
            {
                this.IsLoading = true;
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
