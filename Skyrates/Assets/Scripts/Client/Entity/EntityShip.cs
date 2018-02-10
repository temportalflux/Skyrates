using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Loot;
using Skyrates.Client.Ship;
using UnityEngine;

namespace Skyrates.Common.Entity
{
    
    public class EntityShip : EntityDynamic
    {

        public ShipStat StatBlock;

        private bool _markedForDestruction = false;

        // Called when some non-trigger collider with a rigidbody enters
        protected virtual void OnTriggerEnter(Collider other)
        {
            int damage = 0;

            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null && !this._markedForDestruction)
            {
                // TODO: Implement projectile damage
                damage += 1;//projectile.GetDamage();

                GameManager.Events.Dispatch(new EventEntityShipHitByProjectile(this, projectile, damage));

                // collider is a projectile
                Destroy(projectile.gameObject);
            }

            ShipFigurehead ram = other.GetComponent<ShipFigurehead>();
            if (ram != null && !this._markedForDestruction)
            {
                // TODO: Implement ram damage
                damage += 1;//ram.GetDamage();
                
                GameManager.Events.Dispatch(new EventEntityShipHitByRam(this, ram, damage));
            }

            if (damage <= 0) return;

            // TODO: Use a health system
            this._markedForDestruction = true;

            Vector3 position = this.transform.position;
                
            GameManager.Events.Dispatch(new EventEntityShipDamaged(this, damage));

            // Deal damage
            Destroy(this.gameObject);

            // Spawn loot
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
