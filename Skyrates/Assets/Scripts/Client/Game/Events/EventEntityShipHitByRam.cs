using Skyrates.Client.Ship;
using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    public class EventEntityShipHitByRam : EventEntityShipDamaged
    {

        public ShipFigurehead Figurehead;

        public EventEntityShipHitByRam(EntityShip ship, ShipFigurehead ram, float damage)
            : base(GameEventID.EntityShipHitByRam, ship, damage)
        {
            this.Figurehead = ram;
        }


    }

}