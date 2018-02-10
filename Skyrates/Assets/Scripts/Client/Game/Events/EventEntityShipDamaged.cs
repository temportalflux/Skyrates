using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    public class EventEntityShipDamaged : EventEntityShip
    {

        public float Damage;

        public EventEntityShipDamaged(GameEventID id, EntityShip ship, float damage) : base(id, ship)
        {
            this.Damage = damage;
        }

        public EventEntityShipDamaged(EntityShip ship, float damage) : this(GameEventID.EntityShipDamaged, ship, damage)
        {
        }


    }

}