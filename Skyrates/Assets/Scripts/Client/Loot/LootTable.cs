using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Ship;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Skyrates.Client.Loot
{

    [CreateAssetMenu(menuName = "Data/Ship/Loot Table")]
    public class LootTable : ScriptableObject
    {

        [Serializable]
        public class Row
        {
            [SerializeField]
            public int Weight;

            [SerializeField]
            public ShipComponent Item;

            [SerializeField]
            public GameObject Prefab;

            public float Percentage; // generated
        }

        [SerializeField]
        public Row[] Table;

        public int AmountMin;
        public int AmountMax;

        public void CalculatePercentages(float sumWeight)
        {
            for (int iRow = 0; iRow < this.Table.Length; iRow++)
            {
                this.Table[iRow].Percentage = (this.Table[iRow].Weight) / sumWeight;
            }
        }

        public KeyValuePair<ShipComponent, GameObject>[] Generate()
        {
            KeyValuePair<ShipComponent, GameObject>[] loots = new KeyValuePair<ShipComponent, GameObject>[Random.Range(this.AmountMin, this.AmountMax)];
            int iLoot = 0;
            while (iLoot < loots.Length)
            {
                float rand = Random.value;
                float sum = 0;
                foreach (Row row in this.Table)
                {
                    sum += row.Percentage;
                    if (rand < sum)
                    {
                        loots[iLoot] = new KeyValuePair<ShipComponent, GameObject>(row.Item, row.Prefab);
                        break;
                    }
                }
                iLoot++;
            }
            return loots;
        }

    }

}
