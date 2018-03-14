﻿using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Mono;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Entity
{

    /// <summary>
    /// Special case of entity ship which is completely controlled by AI.
    /// </summary>
    public class EntityShipNPC : EntityShip
    {

        [SerializeField]
        public float LootDropRadius;

        public Shooter[] Shooters;

        protected override void SpawnLoot(Vector3 position)
        {
            if (this.StatBlock == null || this.StatBlock.Loot == null) return;

            // Generate the loot to spawn
            KeyValuePair<ShipData.BrokenComponentType, Loot.Loot>[] loots = this.StatBlock.Loot.Generate();
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
                // TODO: Loot event, loot should be static entity for networking
            }
        }

        protected override Shooter[] GetArtilleryShooters(ShipData.ComponentType artillery)
        {
            return this.Shooters;
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
                // TODO: Move to a behavior
                this.StartShooting(other as EntityPlayerShip, maxDistance);
            }
        }

        protected virtual void StartShooting(EntityPlayerShip target, float maxDistance)
        {
            
        }

    }
}
