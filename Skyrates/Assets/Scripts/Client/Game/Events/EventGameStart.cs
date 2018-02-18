
using Skyrates.Common.Network;

namespace Skyrates.Client.Game.Event
{

    /// <summary>
    /// Dispatched when a game starts (regardless of if the network 
    /// </summary>
    public class EventGameStart : GameEvent
    {

        /// <summary>
        /// The type of network game that is being started.
        /// </summary>
        public Session.NetworkMode GameMode;

        public EventGameStart(Session.NetworkMode gameMode) : base(GameEventID.GameStart)
        {
            this.GameMode = gameMode;
        }

    }

}
