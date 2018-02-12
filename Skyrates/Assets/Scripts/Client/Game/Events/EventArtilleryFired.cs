using Skyrates.Client.Ship;
using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    public class EventArtilleryFired : EventEntity
    {

        public Shooter[] Shooters;

        public EventArtilleryFired(EntityShip ship, Shooter[] shooters)
            : base(GameEventID.ArtilleryFired, ship)
        {
            this.Shooters = shooters;
        }

    }

}
