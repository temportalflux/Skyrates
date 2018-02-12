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

        public float DamageMultiplier;

        public float GetDamage()
        {
            return this.DamageMultiplier;
        }

    }

}
