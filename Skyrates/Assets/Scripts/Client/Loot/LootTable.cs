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
                this.Table[iRow].Percentage = this.Table[iRow].Weight / sumWeight;
            }
        }

        public ShipComponent[] Generate()
        {
            ShipComponent[] loots = new ShipComponent[Random.Range(this.AmountMin, this.AmountMax)];
            for (int i = 0; i < loots.Length; i++)
            {
                float rand = Random.value;
                int iRow = 0;
                while (rand > 0 && iRow < this.Table.Length)
                {
                    rand -= this.Table[iRow++].Percentage;
                }
                if (iRow < this.Table.Length)
                    loots[i] = this.Table[iRow].Item;
                else
                    Debug.Log("Error, loot table gen found null");
            }
            return loots;
        }

    }

}
