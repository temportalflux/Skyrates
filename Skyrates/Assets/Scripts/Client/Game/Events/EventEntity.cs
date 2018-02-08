using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Game.Event
{

    public class EventEntity : GameEvent
    {

        public Entity Entity;

        public EventEntity(GameEventID id, Entity entity) : base(id)
        {
            this.Entity = entity;
        }

    }

}
