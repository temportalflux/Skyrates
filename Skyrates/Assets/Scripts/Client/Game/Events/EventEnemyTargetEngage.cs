
using Skyrates.Client.Entity;
using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    public class EventEnemyTargetEngage : EventEntityShip
    {

        public EntityShip Target { get; private set; }

        public EventEnemyTargetEngage(GameEventID id, EntityShip owner, EntityShip target)
            : base(id, owner)
        {
            this.Target = target;
        }

        public static EventEnemyTargetEngage Engage(EntityShip owner, EntityShip target)
        {
            return new EventEnemyTargetEngage(GameEventID.EnemyTargetEngage, owner, target);
        }

        public static EventEnemyTargetEngage Disengage(EntityShip owner, EntityShip target)
        {
            return new EventEnemyTargetEngage(GameEventID.EnemyTargetDisengage, owner, target);
        }

    }

}