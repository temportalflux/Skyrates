using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game.Event;
using UnityEngine;

namespace Skyrates.Client.Game.Event
{

    public class EventLootCollected : EventLootCollided
    {

        public EventLootCollected(EntityPlayerShip playerShip, Loot.Loot loot)
            : base(GameEventID.LootCollected, playerShip,
            loot)
        {
            
        }

    }

}
