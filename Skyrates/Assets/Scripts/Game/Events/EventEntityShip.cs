using Skyrates.Entity;

namespace Skyrates.Game.Event
{

    /// <summary>
    /// An event in which the entity is an <see cref="EntityShip"/>.
    /// </summary>
    public class EventEntityShip : EventEntity
    {

        /// <summary>
        /// The entity ship.
        /// </summary>
        public EntityShip Ship
        {
            get { return this.Entity as EntityShip; }
        }

        public EventEntityShip(GameEventID id, EntityShip ship) : base(id, ship)
        {
        }
        

    }

}
