
using Skyrates.Common.Network;

namespace Skyrates.Client.Game.Event
{

    public class EventGameStart : GameEvent
    {

        public Session.NetworkMode GameMode;

        public EventGameStart(Session.NetworkMode gameMode) : base(GameEventID.GameStart)
        {
            this.GameMode = gameMode;
        }

    }

}
