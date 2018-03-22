using System;
using UnityEngine;

namespace Skyrates.Misc
{

    [Serializable]
    public class StateActiveReload
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

        /// <summary>
        /// How much reloaded the cannon is.
        /// </summary>
        [Range(0, 1)]
        private float _percentLoaded = 1.0f;

        /// <summary>
        /// If the cannon is fully loaded.
        /// </summary>
        public bool IsLoaded
        {
            get { return this._percentLoaded >= 1.0f; }
        }

        public float GetPercentLoaded()
        {
            return this._percentLoaded;
        }

        public void Empty()
        {
            this._percentLoaded = 0.0f;
            this._canActiveReload = true;
            this._isLoading = false;
        }

        public void LoadBy(float amount)
        {
            if (!this._isLoading)
                return;
            
            this._percentLoaded = Mathf.Min(1.0f, this._percentLoaded + amount);

            if (this._percentLoaded >= 1.0f)
            {
                this.Load();
            }
        }

        private void Load()
        {
            this._canActiveReload = false;
            this._isLoading = false;
            this._percentLoaded = 1.0f;
        }

        public void TryReload()
        {
            if (!this._isLoading)
            {
                this._isLoading = true;
                this._percentLoaded = 0.0f;
            }
            else if (this._canActiveReload)
            {
                if (this._percentLoaded >= this.PercentStart && this._percentLoaded <= this.PercentEnd)
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
