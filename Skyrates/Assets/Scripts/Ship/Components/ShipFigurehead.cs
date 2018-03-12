using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Client.Ship
{

    /// <summary>
    /// Subclass of <see cref="ShipComponent"/> dedicated to
    /// components of type <see cref="ShipData.ComponentType.Figurehead"/>.
    /// </summary>
    public class ShipFigurehead : ShipComponent
    {
		[Tooltip("The base damage of the figurehead's ram")]
		public float Attack;

		/// <summary>
		/// Gets the base damage of the figurehead's ram.
		/// </summary>
		/// <returns>The base damage of the figurehead's ram</returns>
		public float GetAttack()
        {
            return this.Attack;
        }
	}

}
