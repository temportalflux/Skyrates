using Skyrates.Client.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ComponentType = ShipData.ComponentType;

namespace Skyrates.Client.Ship
{

    /// <summary>
    /// Subclass of <see cref="ShipComponent"/> dedicated to
    /// components of type <see cref="ShipData.ComponentType.Hull"/>.
    /// </summary>
    public class ShipHull : ShipComponent
    {

        [Serializable]
        public class ComponentList
        {
            public ShipComponent[] Value;
        }

        [Tooltip("The base amount of damage subtracted from damage taken")]
        public float Defense;

        [Tooltip("The percentage of damage subtracted from damage taken")]
        public float Protection;
        
        [SerializeField]
        public ComponentList[] Components = new ComponentList[ShipData.NonHullComponents.Length];

        [SerializeField]
        public Transform[] LootMounts;

        /// <summary>
        /// All the loot on the deck.
        /// TODO: Generate loot onto ships which are carrying (loot is random gen on spawn not death).
        /// </summary>
        private readonly List<GameObject> GeneratedLoot = new List<GameObject>();

        /// <summary>
        /// Gets the amount of damage subtracted from damage taken.
        /// </summary>
        /// <returns>The amount of damage subtracted from damage taken</returns>
        public float GetDefense()
        {
            return this.Defense;
        }

        /// <summary>
        /// Gets the percentage of damage subtracted from damage taken.
        /// </summary>
        /// <returns>The percentage of damage subtracted from damage taken</returns>
        public float GetProtection()
        {
            return this.Protection;
        }

        public void GenerateLoot(GameObject lootObjectPrefab, ShipData.BrokenComponentType item, bool forced = false)
        {
            int nextIndex = this.GeneratedLoot.Count;
            if (nextIndex < this.LootMounts.Length)
            {
                Transform mount = this.LootMounts[nextIndex];
                GameObject generated = Instantiate(lootObjectPrefab, this.transform);
                generated.transform.SetPositionAndRotation(mount.position, mount.rotation);
                generated.transform.localScale = mount.localScale;
                this.GeneratedLoot.Add(generated);
                EntityPlayerShip playerShip = this.Ship as EntityPlayerShip;
                if (playerShip)
                {
                    playerShip.PlayerData.Inventory.MapGeneratedLoot(item, generated, forced);
                }
            }
        }

        protected int GetComponentIndex(ShipData.ComponentType type)
        {
            return ShipData.HulllessComponentIndex[(int)type];
        }

        /// <summary>
        /// Returns the generated component list for a specific type.
        /// </summary>
        /// <param name="compType"></param>
        /// <returns></returns>
        public ShipComponent[] GetComponent(ShipData.ComponentType compType)
        {
            Debug.Assert((int)ShipData.ComponentType.Hull != 0);
            Debug.Assert(ShipData.ComponentType.Hull != compType, "Cannot get hull from hull");
            // compType - 1 to account for Hull
            return this.Components[this.GetComponentIndex(compType)].Value;
        }

    }

}
