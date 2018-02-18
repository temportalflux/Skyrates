using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    public class EventEntityShipDamaged : EventEntityShip
    {

        public Common.Entity.Entity Source;

        public EntityShip Target
        {
            get { return this.Ship; }
        }

        public float Damage;

        public EventEntityShipDamaged(GameEventID id, Common.Entity.Entity source, EntityShip ship, float damage) : base(id, ship)
        {
            this.Source = source;
            this.Damage = damage;
        }

        public EventEntityShipDamaged(Common.Entity.Entity source, EntityShip target, float damage) : this(GameEventID.EntityShipDamaged, source, target, damage)
        {
        }


    }

}