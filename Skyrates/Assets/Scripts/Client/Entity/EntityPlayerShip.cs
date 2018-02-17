
using System.Collections;
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

        protected override void Start()
        {
            base.Start();
            StartCoroutine(this.AutoHeal());
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

        public override Transform GetRender()
        {
            return this.Render;
        }

        IEnumerator AutoHeal()
        {
            while (true)
            {
                yield return new WaitUntil((() => this.Health < this.StatBlock.Health));
                while (this.Health < this.StatBlock.Health)
                {
                    this.Health++;
                    yield return new WaitForSeconds(5.0f);
                }
            }
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

        protected override Shooter[] GetArtilleryShooters(ShipData.ComponentType artillery)
        {
            // TODO: Optimize this
            ShipComponent[] components = this.ShipRoot.Hull.GetGeneratedComponent(artillery);
            List<Shooter> evtArtillery = new List<Shooter>();
            foreach (ShipComponent component in components)
            {
                evtArtillery.Add(((ShipArtillery)component).Shooter);
            }
            return evtArtillery.ToArray();
        }

        protected override bool OnPreDestroy()
        {
            // TODO: Do base, and return to menu (always wait for x seconds, so level loads and the animation can play)
            return false;
        }

        protected override void SpawnLoot(Vector3 position)
        {
        }

        public void OnLootCollided(Loot.Loot loot)
        {
            // TODO: Add to inventory
            this.PlayerData.LootCount++;

            // TODO: Change this
            this.ShipRoot.Hull.GenerateLoot(loot.LootPrefabWithoutSail);

            GameManager.Events.Dispatch(new EventLootCollected(this, loot));
            
            // destroy the loot
            Destroy(loot.gameObject);
        }

    }

}
