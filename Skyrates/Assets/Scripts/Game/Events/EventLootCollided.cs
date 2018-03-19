using Skyrates.Entity;

namespace Skyrates.Game.Event
{

    /// <summary>
    /// Dispatched when loot is collided with by an <see cref="EntityPlayerShip"/>.
    /// </summary>
    public class EventLootCollided : EventEntityPlayerShip
    {

        /// <summary>
        /// The loot collided with.
        /// </summary>
        public Loot.Loot Loot { get; private set; }

        public EventLootCollided(GameEventID id, EntityPlayerShip playerShip, Loot.Loot loot)
            : base(id, playerShip)
        {
            this.Loot = loot;
        }

        public EventLootCollided(EntityPlayerShip playerShip, Loot.Loot loot)
            : this(GameEventID.LootCollided, playerShip, loot)
        {
        }

    }

}
