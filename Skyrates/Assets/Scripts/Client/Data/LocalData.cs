using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Client.Data
{

    /// <summary>
    /// Encapsulates data for the player which does not need to be transferred over the network.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/Player/Local")]
    public class LocalData : ScriptableObject
    {

        /// <summary>
        /// The amount of loot currently collected.
        /// Value is 0 during instantiation and destruction.
        /// </summary>
        public uint LootCount;

        // TODO: Temporary
        /// <summary>
        /// The amount of loot to collect for the "winstate" to occur.
        /// Set to a random number between 10 and 60 on instantiation and destruction (the latter isn't necessary).
        /// </summary>
        public uint LootGoal;

        public void Init()
        {
            this.LootCount = 0;
            this.LootGoal = (uint) Random.Range(10, 60);
        }

    }

}
