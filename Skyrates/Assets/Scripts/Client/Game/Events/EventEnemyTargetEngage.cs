
using Skyrates.Client.Entity;
using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    /// <summary>
    /// Event for enmies engaging/disengaging in combat with a player.
    /// </summary>
    public class EventEnemyTargetEngage : EventEntityShip
    {

        public EntityShip Target { get; private set; }

        public EventEnemyTargetEngage(GameEventID id, EntityShip owner, EntityShip target)
            : base(id, owner)
        {
            this.Target = target;
        }

        /// <summary>
        /// Create an event for a ship engaging in combat with a target.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static EventEnemyTargetEngage Engage(EntityShip owner, EntityShip target)
        {
            return new EventEnemyTargetEngage(GameEventID.EnemyTargetEngage, owner, target);
        }

        /// <summary>
        /// Create an event for a ship disengaging in combat with a target.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static EventEnemyTargetEngage Disengage(EntityShip owner, EntityShip target)
        {
            return new EventEnemyTargetEngage(GameEventID.EnemyTargetDisengage, owner, target);
        }

    }

}