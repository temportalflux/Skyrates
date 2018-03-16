using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Entity;
using Skyrates.Client.Ship;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Loot
{

    /// <summary>
    /// Any falling loot in the game
    /// </summary>
    public class Loot : MonoBehaviour, IDistanceCollidable
    {

        /// <summary>
        /// The item contained in the "loot-crate"
        /// </summary>
        public ShipData.BrokenComponentType Item;

        /// <summary>
        /// The prefab of this loot without its sail
        /// </summary>
        public GameObject LootPrefabWithoutSail;


        public void OnEnterEntityRadius(EntityAI source, float radius)
        {
        }

        public void OnOverlapWith(GameObject other, float radius)
        {
        }

    }

}
