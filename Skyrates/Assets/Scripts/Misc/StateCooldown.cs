using System;
using UnityEngine;

namespace Skyrates.Misc
{

    [Serializable]
    public class StateCooldown
    {

        public float MaxDelaySeconds = 2.0f;

        /// <summary>
        /// How much reloaded the cannon is.
        /// </summary>
        [Range(0, 1)]
        protected float PercentLoaded = 1.0f;

        public virtual void Awake()
        {
            this.PercentLoaded = 0.0f;
        }

        /// <summary>
        /// If the cannon is fully loaded.
        /// </summary>
        public virtual bool IsLoaded()
        {
            return this.PercentLoaded >= 1.0f;
        }

        public float GetPercentLoaded()
        {
            return this.PercentLoaded;
        }

        public virtual void Unload()
        {
            this.PercentLoaded = 0.0f;
        }

        public virtual void Load()
        {
            this.PercentLoaded = 1.0f;
            this.OnLoaded();
        }

        public virtual void Update(float deltaTime)
        {
            float amount = deltaTime / this.MaxDelaySeconds;
            this.PercentLoaded = Mathf.Min(1.0f, this.PercentLoaded + amount);
            if (this.IsLoaded())
            {
                this.OnLoaded();
            }
        }

        public virtual void OnLoaded()
        {
        }

    }

}