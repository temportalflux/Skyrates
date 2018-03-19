using Skyrates.Entity;

namespace Skyrates.Game.Event
{

    /// <summary>
    /// Dispatched when loot is collected by a local player.
    /// </summary>
    public class EventLootCollected : EventLootCollided
    {

        public EventLootCollected(EntityPlayerShip playerShip, Loot.Loot loot)
            : base(GameEventID.LootCollected, playerShip,
            loot)
        {
            
        }

    }

}
