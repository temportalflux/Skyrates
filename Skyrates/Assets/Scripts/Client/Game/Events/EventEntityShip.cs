using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Game.Event
{

    public class EventEntityShip : EventEntity
    {

        public EntityShip Ship
        {
            get
            {
                return this.Entity as EntityShip;
            }
        }

        public EventEntityShip(GameEventID id, EntityShip ship) : base(id, ship)
        {
        }
        

    }

}
