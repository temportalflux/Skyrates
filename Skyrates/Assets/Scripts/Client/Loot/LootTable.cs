using System;
using System.Collections.Generic;
using Skyrates.Client.Ship;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Skyrates.Client.Loot
{

    /// <summary>
    /// Table for generating randomized loot
    /// </summary>
    [CreateAssetMenu(menuName = "Data/Ship/Loot Table")]
    public class LootTable : ScriptableObject
    {

        /// <summary>
        /// A weighted entry in the loot table.
        /// </summary>
        [Serializable]
        public class Row
        {
            /// <summary>
            /// The weight of the entry (bigger = more common).
            /// </summary>
            [SerializeField]
            [Tooltip("Larger = more common")]
            public int Weight;

            /// <summary>
            /// The item component type to be stored in the loot.
            /// </summary>
            [SerializeField]
            public ShipData.BrokenComponentType Item;

            /// <summary>
            /// The prefabed <see cref="Loot"/> to be spawned.
            /// </summary>
            [SerializeField]
            public Loot Prefab;

            /// <summary>
            /// The unweighted percentage calculated when weights are changed.
            /// </summary>
            public float Percentage; // generated
        }

        /// <summary>
        /// The list of weighted entry.
        /// </summary>
        [SerializeField]
        public Row[] Table;

        /// <summary>
        /// The minimum amount of loot to generate at a time.
        /// </summary>
        public int AmountMin;

        /// <summary>
        /// The maximum amount of loot to generate at a time.
        /// </summary>
        public int AmountMax;

        /// <summary>
        /// Calculates the percentage chance of each item in the weighted table.
        /// </summary>
        /// <param name="sumWeight"></param>
        public void CalculatePercentages(float sumWeight)
        {
            foreach (Row row in this.Table)
            {
                row.Percentage = (row.Weight) / sumWeight;
            }
        }

        /// <summary>
        /// Generates a list of loot items and the prefab with a <see cref="Loot"/> component.
        /// The number of loot generated is randomized between <see cref="AmountMin"/> and <see cref="AmountMax"/>.
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<ShipData.BrokenComponentType, Loot>[] Generate()
        {
            // Create the array
            KeyValuePair<ShipData.BrokenComponentType, Loot>[] loots = new KeyValuePair<ShipData.BrokenComponentType, Loot>[Random.Range(this.AmountMin, this.AmountMax)];
            for (int iLoot = 0; iLoot < loots.Length; iLoot++)
            {
                // Randomize the threshold for this loot entry
                float rand = Random.value;
                float sum = 0;
                // Determine which loot is being generated
                foreach (Row row in this.Table)
                {
                    // continue adding the loot's percentage until the sum is greater than the random value
                    sum += row.Percentage;
                    if (rand < sum)
                    {
                        // Set the loot entry and continue with generation
                        loots[iLoot] = new KeyValuePair<ShipData.BrokenComponentType, Loot>(row.Item, row.Prefab);
                        break;
                    }
                }
            }
            return loots;
        }

    }

}
