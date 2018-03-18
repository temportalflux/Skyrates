
using Skyrates.Common.Network;
using Skyrates.Game.Event;

namespace Skyrates.Client.Game.Event
{

    /// <summary>
    /// Dispatched when a game starts (regardless of if the network 
    /// </summary>
    public class EventGameStart : GameEvent
    {

        public EventGameStart() : base(GameEventID.GameStart)
        {
        }

    }

}
