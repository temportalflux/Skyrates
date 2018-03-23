using Skyrates.Entity;
using Skyrates.Mono;
using Skyrates.Ship;
using UnityEngine;

namespace Skyrates.Loot
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
