using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Client.Game.Event
{

    public class GameEvent
    {

        public GameEventID EventID;

        public GameEvent(GameEventID id)
        {
            this.EventID = id;
        }

    }

}
