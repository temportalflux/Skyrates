using Skyrates.Client.Entity;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;
using Skyrates.Server.Network;
using UnityEngine;

namespace Skyrates.Client.Network
{

    public class DummyClient : Client
    {

        /// <inheritdoc />
        public override void Connect(Session session)
        {}

        /// <inheritdoc />
        public override void Create()
        {}

        /// <inheritdoc />
        public override void Destroy()
        {}

        /// <inheritdoc />
        public override void Shutdown()
        {}

        /// <inheritdoc />
        public override void StartAndConnect(Session session)
        {}

        /// <inheritdoc />
        public override void StartClient(Session session)
        {}

        /// <inheritdoc />
        public override void StartServer(Session session)
        {}

        /// <inheritdoc />
        public override void Dispatch(NetworkEvent evt, string address, int port, bool broadcast = false)
        {
        }

        /// <inheritdoc />
        public override void Update()
        {
        }

        /// <inheritdoc />
        public override void SubscribeEvents()
        {
            this.HasSubscribed = true;
            GameManager.Events.SpawnEntityProjectile += this.OnRequestSpawnEntityProjectile;
            GameManager.Events.EntityShipHitByProjectile += this.OnEntityShipHitBy;
            GameManager.Events.EntityShipHitByRam += this.OnEntityShipHitBy;
            GameManager.Events.LootCollided += this.OnLootCollided;
        }

        /// <inheritdoc />
        public override void UnsubscribeEvents()
        {
            this.HasSubscribed = false;
            GameManager.Events.SpawnEntityProjectile -= this.OnRequestSpawnEntityProjectile;
            GameManager.Events.EntityShipHitByProjectile -= this.OnEntityShipHitBy;
            GameManager.Events.EntityShipHitByRam -= this.OnEntityShipHitBy;
            GameManager.Events.LootCollided -= this.OnLootCollided;
        }


        /// <inheritdoc />
        public override void OnRequestSpawnEntityProjectile(GameEvent evt)
        {
            EventSpawnEntityProjectile evtSpawn = (EventSpawnEntityProjectile) evt;
            Common.Entity.Entity entity = GameManager.Instance.SpawnEntity(evtSpawn.TypeData, Common.Entity.Entity.NewGuid());
            if (entity != null)
            {
                EntityProjectile projectile = (EntityProjectile) entity;
                projectile.Launch(evtSpawn.Spawn.position, evtSpawn.Spawn.rotation, evtSpawn.Velocity, evtSpawn.ImpluseForce);
            }
        }

        /// <inheritdoc />
        public override void OnEntityShipHitBy(GameEvent evt)
        {
            EventEntityShipDamaged evtDamaged = (EventEntityShipDamaged) evt;
            evtDamaged.Target.TakeDamage(evtDamaged);
        }

        /// <inheritdoc />
        public override void OnLootCollided(GameEvent evt)
        {
            EventLootCollided evtLoot = (EventLootCollided) evt;
            // pass back to player for handling locally
            evtLoot.PlayerShip.OnLootCollided(evtLoot.Loot);
        }

    }

}
