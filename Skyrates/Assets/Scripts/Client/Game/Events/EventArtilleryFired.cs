using Skyrates.Client.Ship;

namespace Skyrates.Client.Game.Event
{

    public class EventArtilleryFired : EventEntityPlayerShip
    {

        public ShipArtillery[] Artillery;

        public EventArtilleryFired(EntityPlayerShip playerShip, ShipArtillery[] artillery)
            : base(GameEventID.ArtilleryFired, playerShip)
        {
            this.Artillery = artillery;
        }

    }

}
