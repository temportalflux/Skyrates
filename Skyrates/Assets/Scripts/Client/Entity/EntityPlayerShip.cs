
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Ship;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Client
{

    public class EntityPlayerShip : EntityShip
    {

        [BitSerialize(5)]
        [HideInInspector]
        public ShipData ShipData;

        public LocalData PlayerData;

        [Tooltip("The transform which points towards where the forward direction is.")]
        public Transform View;

        [Tooltip("The root of the render object (must be a child/decendent of this root)")]
        public Transform Render;

        public Ship.Ship ShipRoot;

        protected override void Awake()
        {
            base.Awake();
            this.ShipRoot.Destroy();
            this.ShipData = this.ShipRoot.Generate();
            this.PlayerData.Init();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.PlayerData.Init();
        }

        protected override Transform GetView()
        {
            return this.View;
        }

        protected override Transform GetRender()
        {
            return this.Render;
        }

        #region Network
        
        public void SetDummy()
        {
            this.View.gameObject.SetActive(false);
            this.Steering = null;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            GameManager.Events.Dispatch(new EventEntityPlayerShip(GameEventID.PlayerMoved, this));
        }

        public override bool ShouldDeserialize()
        {
            return this.OwnerNetworkID != NetworkComponent.GetSession.NetworkID;
        }

        public override void OnDeserializeSuccess()
        {
            base.OnDeserializeSuccess();
            if (this.ShipRoot.ShipData.MustBeRebuilt)
            {
                this.ShipRoot.Destroy();
                this.ShipData = this.ShipRoot.Generate(this.ShipData);
            }
        }

        #endregion

        protected override Shooter[] GetArtilleryShooters()
        {
            // TODO: Optimize this
            Vector3 forwardAim = this.GetView().forward;
            ShipComponent[] components = this.ShipRoot.Hull.GetGeneratedComponent(ShipData.ComponentType.Artillery);
            List<Shooter> evtArtillery = new List<Shooter>();
            for (int iComponent = 0; iComponent < components.Length; iComponent++)
            {
                Vector3 forwardArtillery = components[iComponent].transform.forward;
                float dot = Vector3.Dot(forwardAim, forwardArtillery);
                if (dot > 0.3)
                {
                    evtArtillery.Add(((ShipArtillery) components[iComponent]).Shooter);
                }
            }
            return evtArtillery.ToArray();
        }

        public override void TakeDamage(float damage)
        {
        }

        protected override void SpawnLoot(Vector3 position)
        {
        }

        public void OnLootCollided(Loot.Loot loot)
        {
            // TODO: Add to inventory
            this.PlayerData.LootCount++;

            GameManager.Events.Dispatch(new EventLootCollected(this, loot));
            
            // destroy the loot
            Destroy(loot.gameObject);
        }

    }

}
