using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;

namespace Skyrates.Client.Network.Event
{

    /// <summary>
    /// Event dispatched when a client wants to join a server.
    /// </summary>
    [NetworkEvent(Side.Client, Side.Server)]
    public class EventHandshakeJoin : NetworkEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public EventHandshakeJoin() : base(NetworkEventID.HandshakeJoin) { }
    }

}
