using Skyrates.Client.Ship;
using Skyrates.Common.Entity;
using Skyrates.Client.Loot;
using UnityEngine;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Common.Network;

namespace Skyrates.Client.Entity
{
    public class EntityShipNavy : EntityShip
    {

        private EntityDynamic _aiTarget;

        protected override void Start()
        {
            base.Start();
            this._aiTarget = null;
        }

        protected override void FixedUpdate()
        {
            this.SteeringData.Target = this._aiTarget == null ? this.Physics : this._aiTarget.Physics;
            base.FixedUpdate();
        }

        private void OnEnable()
        {
            GameManager.Events.EntityStart += this.OnEntityStart;
            GameManager.Events.EntityDestroy += this.OnEntityDestroy;
        }

        private void OnDisable()
        {
            GameManager.Events.EntityStart -= this.OnEntityStart;
            GameManager.Events.EntityDestroy -= this.OnEntityDestroy;
        }

        void OnEntityStart(GameEvent evt)
        {
            EventEntity evtEntity = (EventEntity) evt;
            if (evtEntity.Entity.EntityType.EntityType == Type.Player && evtEntity.Entity is EntityPlayerShip)
            {
                if (this._aiTarget == null || Random.value < 0.5f)
                {
                    this._aiTarget = (EntityPlayerShip)evtEntity.Entity;
                }
            }
        }

        void OnEntityDestroy(GameEvent evt)
        {
            EventEntity evtEntity = (EventEntity) evt;
            if (this._aiTarget != null && evtEntity.Entity.Guid == this._aiTarget.Guid)
            {
                this._aiTarget = null;
            }
        }

    }
}
