using Skyrates.Client.Entity;
using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    /// <summary>
    /// An event in which the entity ship is damaged by some other entity.
    /// </summary>
    public class EventEntityShipDamaged : EventEntityShip
    {

        /// <summary>
        /// The entity which is causing the damage.
        /// </summary>
        public Common.Entity.Entity Source;

        /// <summary>
        /// The target of the damage.
        /// </summary>
        public EntityShip Target
        {
            get { return this.Ship; }
        }

        /// <summary>
        /// The damage to be caused.
        /// </summary>
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