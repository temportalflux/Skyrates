using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Client.Game.Event
{

    public class EventLootCollided : EventEntityPlayerShip
    {

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
