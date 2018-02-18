using Skyrates.Client.Entity;
using Skyrates.Client.Ship;
using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    public class EventEntityShipHitByRam : EventEntityShipDamaged
    {

        public ShipFigurehead Figurehead;

        public EventEntityShipHitByRam(EntityShip ship, ShipFigurehead ram)
            : base(GameEventID.EntityShipHitByRam, ram.Ship, ship, ram.GetDamage())
        {
            this.Figurehead = ram;
        }


    }

}