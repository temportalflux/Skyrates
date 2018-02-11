using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Loot;
using Skyrates.Client.Ship;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{
    
    public class EntityShip : EntityDynamic
    {

        public ShipStat StatBlock;

        [BitSerialize(0)]
        public float Health;

        protected override void Start()
        {
            base.Start();
            Debug.Assert(this.StatBlock.Health > 0, string.Format(
                "StatBlock {0} has 0 health, they will be killed on first hit, so at least make this a 1 pls.", this.StatBlock.name));
            this.Health = this.StatBlock.Health;
        }

        // Called when some non-trigger collider with a rigidbody enters
        protected virtual void OnTriggerEnter(Collider other)
        {
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)
            {
                GameManager.Events.Dispatch(new EventEntityShipHitByProjectile(this, projectile));
                // collider is a projectile
                // TODO: Owner should destroy based on networking
                Destroy(projectile.gameObject);
            }

            ShipFigurehead ram = other.GetComponent<ShipFigurehead>();
            if (ram != null)
            {
                GameManager.Events.Dispatch(new EventEntityShipHitByRam(this, ram));
            }

        }

        // called by network interface events which originate from OnTriggerEnter
        public void TakeDamage(float damage)
        {
            this.Health -= damage;

            GameManager.Events.Dispatch(new EventEntityShipDamaged(this, damage));

            if (this.Health > 0) return;

            Vector3 position = this.transform.position;

            // Deal damage
            Destroy(this.gameObject);

            // Spawn loot
            // NOTE: Since this function is called by the owning client, then loot is spawned on owning client
            // TODO: Sync loot - make loot entity static
            this.SpawnLoot(position);

        }

        private void SpawnLoot(Vector3 position)
        {
            ShipComponent[] loots = this.StatBlock.Loot.Generate();
            foreach (ShipComponent lootItem in loots)
            {
                if (lootItem == null)
                {
                    Debug.LogWarning("Error, loot item null");
                    continue;
                }

                Vector3 pos = position + Random.insideUnitSphere * 3;
                Loot loot = Instantiate(
                    this.StatBlock.LootPrefab.gameObject, pos, Quaternion.identity).GetComponent<Loot>();
                loot.Item = lootItem;
                // TODO: Loot event, loot should be static entity
            }
        }

    }

}
