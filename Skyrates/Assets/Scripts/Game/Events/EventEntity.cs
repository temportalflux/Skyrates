using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using Skyrates.Game.Event;
using UnityEngine;

namespace Skyrates.Client.Game.Event
{

    /// <summary>
    /// Generic event which has an entity.
    /// </summary>
    public class EventEntity : GameEvent
    {

        public Common.Entity.Entity Entity;

        public EventEntity(GameEventID id, Common.Entity.Entity entity) : base(id)
        {
            this.Entity = entity;
        }

    }

}
