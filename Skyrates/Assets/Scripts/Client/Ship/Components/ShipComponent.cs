using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Ship
{

    /// <summary>
    /// Base class for all components when are generated as a part of a <see cref="Ship"/> via <see cref="ShipBuilder"/>.
    /// </summary>
    public class ShipComponent : MonoBehaviour
    {

        [HideInInspector]
        public EntityShip Ship;
        
    }

}
