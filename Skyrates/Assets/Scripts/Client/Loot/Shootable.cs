using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Loot;
using Skyrates.Client.Ship;
using UnityEngine;

namespace Skyrates.Client.Loot
{

    [RequireComponent(typeof(Collider))]
    public class Shootable : MonoBehaviour
    {

        // The owner of this shootable (destroyed when shot)
        public GameObject RootParent;

        public Loot LootPrefab;

        public LootTable LootTable;

        private bool _markedForDestruction = false;

        // Requires that all colliders are triggers
        // Also requires incoming colliders to be rigidbodies

        private void Start()
        {
            // TODO: Potentially expensive, and not required
            this.checkTriggers();
        }

        private void checkTriggers()
        {
            foreach (Collider collider in this.GetComponents<Collider>())
            {
                collider.isTrigger = true;
            }
        }

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
                Destroy(this.RootParent);

                // Spawn loot
                this.SpawnLoot(position);
            }

        }

        private void SpawnLoot(Vector3 position)
        {
            ShipComponent[] loots = this.LootTable.Generate();
            foreach (ShipComponent lootItem in loots)
            {
                Vector3 pos = position + Random.insideUnitSphere * 3;
                Loot loot = Instantiate(
                    this.LootPrefab.gameObject, pos, Quaternion.identity).GetComponent<Loot>();
                loot.Item = lootItem;
            }
        }

    }


}

