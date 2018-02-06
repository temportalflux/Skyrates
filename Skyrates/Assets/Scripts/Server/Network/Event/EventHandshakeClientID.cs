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

        public EventHandshakeClientID(ClientData client) : base(NetworkEventID.HandshakeClientID)
        {
            this.client = client;
        }

    }

}
