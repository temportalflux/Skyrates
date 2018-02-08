
using System;

namespace Skyrates.Client.Game.Event
{

    public class EventPlayerLeft : GameEvent
    {

        public Guid PlayerGuid;

        public EventPlayerLeft(Guid guid) : base(GameEventID.PlayerLeft)
        {
            this.PlayerGuid = guid;
        }

    }

}