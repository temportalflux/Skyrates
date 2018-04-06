using System;
using Skyrates.Misc;
using Skyrates.Ship;
using UnityEngine;

namespace Skyrates.Data
{

    [Serializable]
    public class StateArtillery
    {

        public ShipComponentList ComponentList;

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

		public void Update(float deltaTime, ShipData shipData)
        {
            this.Gimbal.Update(deltaTime, this.GetRateOfFire(shipData, ShipData.ComponentType.ArtilleryForward));
            this.Starboard.Update(deltaTime, this.GetRateOfFire(shipData, ShipData.ComponentType.ArtilleryForward));
            this.Port.Update(deltaTime, this.GetRateOfFire(shipData, ShipData.ComponentType.ArtilleryForward));
            this.Bombs.Update(deltaTime, this.GetRateOfFire(shipData, ShipData.ComponentType.ArtilleryForward));
        }

        private float GetRateOfFire(ShipData shipData, ShipData.ComponentType type)
        {
            int tier = shipData.ComponentTiers[(int) type];
            ShipArtillery artillery = (ShipArtillery) this.ComponentList.Categories[(int)type].Prefabs[tier];
            return artillery.RateOfFireModifier;
        }

    }

}
