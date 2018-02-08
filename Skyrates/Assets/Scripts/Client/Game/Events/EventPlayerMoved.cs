using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Client.Game.Event
{

    public class EventPlayerMoved : EventEntity
    {

        public EntityPlayer Player
        {
            get
            {
                return this.Entity as EntityPlayer;
            }
        }

        public EventPlayerMoved(EntityPlayer player) : base(GameEventID.PlayerMoved, player)
        {
        }

    }

}
