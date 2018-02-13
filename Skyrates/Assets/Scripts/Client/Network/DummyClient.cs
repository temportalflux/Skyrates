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

        public override void Connect(Session session)
        {}

        public override void Create()
        {}

        public override void Destroy()
        {}

        public override void Shutdown()
        {}

        public override void StartAndConnect(Session session)
        {}

        public override void StartClient(Session session)
        {}

        public override void StartServer(Session session)
        {}

        public override void Dispatch(NetworkEvent evt, string address, int port, bool broadcast = false)
        {
        }

        public override void Update()
        {
        }

        public override void SubscribeEvents()
        {
            this.HasSubscribed = true;
            GameManager.Events.SpawnEntityProjectile += this.OnRequestSpawnEntityProjectile;
            GameManager.Events.EntityShipHitByProjectile += this.OnEntityShipHitBy;
            GameManager.Events.EntityShipHitByRam += this.OnEntityShipHitBy;
            GameManager.Events.LootCollided += this.OnLootCollided;
        }

        public override void UnsubscribeEvents()
        {
            this.HasSubscribed = false;
            GameManager.Events.SpawnEntityProjectile -= this.OnRequestSpawnEntityProjectile;
            GameManager.Events.EntityShipHitByProjectile -= this.OnEntityShipHitBy;
            GameManager.Events.EntityShipHitByRam -= this.OnEntityShipHitBy;
            GameManager.Events.LootCollided -= this.OnLootCollided;
        }

        public void OnRequestSpawnEntityProjectile(GameEvent evt)
        {
            EventSpawnEntityProjectile evtSpawn = (EventSpawnEntityProjectile) evt;
            Common.Entity.Entity entity = GameManager.Instance.SpawnEntity(evtSpawn.TypeData, Common.Entity.Entity.NewGuid());
            if (entity != null)
            {
                EntityProjectile projectile = (EntityProjectile) entity;
                projectile.Launch(evtSpawn.Spawn.position, evtSpawn.Spawn.rotation, evtSpawn.Velocity, evtSpawn.ImpluseForce);
            }
        }

        public override void OnEntityShipHitBy(GameEvent evt)
        {
            EventEntityShipDamaged evtDamaged = (EventEntityShipDamaged) evt;
            evtDamaged.Ship.TakeDamage(evtDamaged.Damage);
        }

        public override void OnLootCollided(GameEvent evt)
        {
            EventLootCollided evtLoot = (EventLootCollided) evt;
            // pass back to player for handling locally
            evtLoot.PlayerShip.OnLootCollided(evtLoot.Loot);
        }

    }

}
