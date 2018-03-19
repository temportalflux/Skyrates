
namespace Skyrates.Game.Event
{

    /// <summary>
    /// Generic event which has an entity.
    /// </summary>
    public class EventEntity : GameEvent
    {

        public Entity.Entity Entity;

        public EventEntity(GameEventID id, Entity.Entity entity) : base(id)
        {
            this.Entity = entity;
        }

    }

}
