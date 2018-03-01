using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Ship;
using UnityEngine;

namespace Skyrates.Client.Loot
{

    /// <summary>
    /// Any falling loot in the game
    /// </summary>
    public class Loot : MonoBehaviour
    {

        /// <summary>
        /// The item contained in the "loot-crate"
        /// </summary>
        public ShipData.BrokenComponentType Item;

        /// <summary>
        /// The prefab of this loot without its sail
        /// </summary>
        public GameObject LootPrefabWithoutSail;

    }

}
