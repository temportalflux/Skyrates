using Skyrates.Common.Network;

namespace Skyrates.Client.Network.Event
{
    [NetworkEvent(Side.Client, Side.Server)]
    public class EventHandshakeJoin : NetworkEvent
    {
        public EventHandshakeJoin() : base(NetworkEventID.HandshakeJoin) { }
    }
}
