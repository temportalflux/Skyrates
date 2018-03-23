using System.Collections.Generic;
using Skyrates.Loot;
using Skyrates.Ship;
using UnityEngine;

namespace Skyrates.Entity
{

    /// <summary>
    /// Special case of entity ship which is completely controlled by AI.
    /// </summary>
    public class EntityShipNPC : EntityShip
    {

        [Header("NPC: Loot")]

        [Tooltip("The amount of loot dropped by the ship")]
        public LootTable Loot;

        [SerializeField]
        public float LootDropRadius;

        /// <inheritdoc />
        protected override void SpawnLoot(Vector3 position)
        {
            if (this.Loot == null) return;

            // Generate the loot to spawn
            KeyValuePair<ShipData.BrokenComponentType, Loot.Loot>[] loots = this.Loot.Generate();
            // Spawn each loot in turn
            foreach (KeyValuePair<ShipData.BrokenComponentType, Loot.Loot> lootItem in loots)
            {
                // If the loot is invalid in some way, discard
                if (lootItem.Key == ShipData.BrokenComponentType.Invalid || lootItem.Value == null)
                {
                    continue;
                }

                // Get a random position to spawn it
                Vector3 pos = position + Vector3.Scale(Random.insideUnitSphere, Vector3.one * this.LootDropRadius);

                // Create the prefab instance for the loot
                Loot.Loot loot = Instantiate(lootItem.Value.gameObject, pos, Quaternion.identity).GetComponent<Loot.Loot>();
                // Set the item the loot contains
                loot.Item = lootItem.Key;
            }
        }

        /// <summary>
        /// When we enter the radius of some other entity
        /// </summary>
        /// <param name="other"></param>
        /// <param name="maxDistance"></param>
        public override void OnEnterEntityRadius(EntityAI other, float maxDistance)
        {
            if (other is EntityPlayerShip)
            {
                // TODO: Move shooting to an ai behavior
                this.StartShooting(other as EntityPlayerShip, maxDistance);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="maxDistance"></param>
        protected virtual void StartShooting(EntityPlayerShip target, float maxDistance)
        {
            
        }

    }
}
