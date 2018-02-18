using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Entity;
using UnityEngine;

namespace Skyrates.Client.Game.Event
{

    public class EventEntityPlayerShip : EventEntity
    {

        public EntityPlayerShip PlayerShip
        {
            get
            {
                return this.Entity as EntityPlayerShip;
            }
        }

        public EventEntityPlayerShip(GameEventID id, EntityPlayerShip playerShip) : base(id, playerShip)
        {
        }

    }

}
