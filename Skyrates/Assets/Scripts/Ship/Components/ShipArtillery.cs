using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Mono;
using UnityEngine;

namespace Skyrates.Client.Ship
{

    /// <summary>
    /// Subclass of <see cref="ShipComponent"/> dedicated to
    /// components of type <see cref="ShipData.ComponentType.Artillery"/>.
    /// </summary>
    public class ShipArtillery : ShipComponent
    {
        
        public Shooter Shooter;

    }

}
