using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;

namespace Skyrates.Client.Network.Event
{
    [NetworkEvent(Side.Client, Side.Server)]
    public class EventHandshakeJoin : NetworkEvent
    {
        public EventHandshakeJoin() : base(NetworkEventID.HandshakeJoin) { }
    }
}
