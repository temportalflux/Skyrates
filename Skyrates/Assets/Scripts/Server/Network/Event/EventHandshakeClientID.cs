using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;
using UnityEngine;

namespace Skyrates.Server.Network.Event
{

    [NetworkEvent(Side.Server, Side.Client)]
    public class EventHandshakeClientID : NetworkEvent
    {

        [BitSerialize(1)]
        public ClientData client;

        // dispatch
        public EventHandshakeClientID(ClientData client) : this()
        {
            this.client = client;
        }

        // deserialize
        public EventHandshakeClientID() : base(NetworkEventID.HandshakeClientID)
        {
        }

    }

}
