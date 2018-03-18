using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Entity;
using Skyrates.Client.Game.Event;
using Skyrates.Game.Event;
using UnityEngine;

namespace Skyrates.Client.Game.Event
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
