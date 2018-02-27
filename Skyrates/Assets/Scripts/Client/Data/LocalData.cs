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
		/// Manages inventory and items.
		/// </summary>
		public Inventory Inventory;

		public void Init()
        {
            //TODO: Implement if necessary.
        }

    }

}
