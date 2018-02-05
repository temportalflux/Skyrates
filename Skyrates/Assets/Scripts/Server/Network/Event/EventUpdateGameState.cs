using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;

namespace Skyrates.Server.Network.Event
{

    [NetworkEvent(Side.Server, Side.Client)]
    public class EventUpdateGameState : NetworkEvent
    {

        [BitSerialize(1)]
        public GameStateData ServerState;

        public EventUpdateGameState()
        {
            this.ServerState = new GameStateData();
        }

    }

}