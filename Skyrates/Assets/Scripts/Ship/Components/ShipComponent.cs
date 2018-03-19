using Skyrates.Client.Entity;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Ship
{

    /// <summary>
    /// Base class for all components when are generated as a part of a <see cref="Ship"/> via <see cref="ShipRig"/>.
    /// </summary>
    public class ShipComponent : UnityEngine.MonoBehaviour
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
