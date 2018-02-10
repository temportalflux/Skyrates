using System.Collections;
using System.Collections.Generic;
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
        private void OnTriggerEnter(Collider other)
        {

            bool destroy = false;

            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null && !this._markedForDestruction)
            {
                destroy = true;
                // collider is a projectile
                Destroy(projectile.gameObject);
            }

            if (other.CompareTag("Ram"))
            {
                //ShipFigurehead ram = other.GetComponent<ShipFigurehead>();
                destroy = true;
            }

            if (destroy)
            {
                this._markedForDestruction = true;

                Vector3 position = this.transform.position;

                // Deal damage
                Destroy(this.transform);

                // Spawn loot
                this.SpawnLoot(position);
            }

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
            }
        }

    }

}
