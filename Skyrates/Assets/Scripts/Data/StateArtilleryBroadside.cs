using System;
using Skyrates.Misc;
using UnityEngine;

namespace Skyrates.Data
{

    [Serializable]
    public class StateArtilleryBroadside
    {

        [SerializeField]
        public StateActiveReload Reload;

        [SerializeField]
        public float ShootDelay = 2.0f;

        public void LoadBy(float deltaTime)
        {
            this.Reload.LoadBy(deltaTime / this.ShootDelay);
        }

    }

}
