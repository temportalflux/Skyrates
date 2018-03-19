using Skyrates.Entity;
using UnityEngine;

namespace Skyrates.Ship
{

    /// <summary>
    /// Base class for all components when are generated as a part of a <see cref="Ship"/> via <see cref="ShipRig"/>.
    /// </summary>
    public class ShipComponent : MonoBehaviour
    {

        [HideInInspector]
        public EntityShip Ship;

		/// <summary>
		/// Index of the tier, should be tier number minus one.
		/// </summary>
		[Tooltip("Index of the tier, should be tier number minus one.")]
		public uint TierIndex = 0;
        
    }

}
