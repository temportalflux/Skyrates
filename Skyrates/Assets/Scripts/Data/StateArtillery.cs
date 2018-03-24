using System;
using Skyrates.Misc;
using UnityEngine;

namespace Skyrates.Data
{

    [Serializable]
    public class StateArtillery
    {

        [SerializeField]
        public StateActiveReload Gimbal;

        [SerializeField]
        public StateOverheat Starboard;

        [SerializeField]
        public StateOverheat Port;

        [SerializeField]
        public StateCooldown Bombs;

        public void Awake()
        {
            this.Gimbal.Awake();
            this.Starboard.Awake();
            this.Port.Awake();
            this.Bombs.Awake();
        }

		public void Update(float deltaTime)
        {
            this.Gimbal.Update(deltaTime);
            this.Starboard.Update(deltaTime);
            this.Port.Update(deltaTime);
            this.Bombs.Update(deltaTime);
        }

    }

}
