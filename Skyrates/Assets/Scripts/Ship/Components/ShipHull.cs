using System;
using System.Collections.Generic;
using Skyrates.Entity;
using UnityEngine;

namespace Skyrates.Ship
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

        [Tooltip("The amount of damage the ship can take")]
        public int MaxHealth;

        public float HealthRegenAmount;
        public float HealthRegenDelay;

        [Serializable]
        public class HealthFeedback
        {

            // TODO: https://docs.unity3d.com/ScriptReference/EditorGUILayout.CurveField.html

            // amount of damage taken at which the smoke starts at
            [SerializeField]
            public Vector2Int SmokeDamage;

            [SerializeField]
            public Vector2Int SmokeEmissionAmount;

            // amount of damage taken at which the fire starts at
            [SerializeField]
            public Vector2Int FireDamage;

            [SerializeField]
            public Vector2Int FireEmissionAmount;

        }

        [Header("Effects")]
        [SerializeField]
        public HealthFeedback HealthFeedbackData;

        [Header("Components")]
        [SerializeField]
        public ComponentList[] Components = new ComponentList[ShipData.ComponentTypes.Length];

        [Header("Loot on Ship")]
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
            float value = 0.0f;
            foreach (ShipComponent comp in this.GetComponent(ShipData.ComponentType.HullArmor))
            {
                ShipHullArmor hullArmor = comp as ShipHullArmor;
                if (hullArmor != null)
                {
                    value += hullArmor.GetDefense();
                }
            }
            return value;
        }

        /// <summary>
        /// Gets the percentage of damage subtracted from damage taken.
        /// </summary>
        /// <returns>The percentage of damage subtracted from damage taken</returns>
        public float GetProtection()
        {
            float value = 0.0f;
            foreach (ShipComponent comp in this.GetComponent(ShipData.ComponentType.HullArmor))
            {
                ShipHullArmor hullArmor = comp as ShipHullArmor;
                if (hullArmor != null)
                {
                    value *= hullArmor.GetProtection();
                }
            }
            return value;
        }

        public void AddLoot(GameObject lootObjectPrefab, ShipData.BrokenComponentType item, bool forced = false)
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
            return (int)type;
        }

        /// <summary>
        /// Returns the generated component list for a specific type.
        /// </summary>
        /// <param name="compType"></param>
        /// <returns></returns>
        public ShipComponent[] GetComponent(ShipData.ComponentType compType)
        {
            return this.Components[this.GetComponentIndex(compType)].Value;
        }

    }

}
