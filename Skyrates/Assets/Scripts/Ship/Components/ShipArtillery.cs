using Skyrates.Mono;

namespace Skyrates.Ship
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
