
using Skyrates.Common.Network;

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
