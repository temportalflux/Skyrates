
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
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

        [Tooltip("The transform which points towards where the forward direction is.")]
        public Transform View;

        [Tooltip("The root of the render object (must be a child/decendent of this root)")]
        public Transform Render;

        public Ship.Ship ShipRoot;

        void Awake()
        {
            this.ShipRoot.Destroy();
            this.ShipData = this.ShipRoot.Generate();
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

        public override void TakeDamage(float damage)
        {
        }

        protected override void SpawnLoot(Vector3 position)
        {
        }

    }

}
