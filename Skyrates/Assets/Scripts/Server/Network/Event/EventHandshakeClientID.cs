using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;
using UnityEngine;

namespace Skyrates.Server.Network.Event
{

    [NetworkEvent(Side.Server, Side.Client)]
    public class EventHandshakeClientID : NetworkEvent
    {

        [BitSerialize(1)]
        public uint clientID;

        public EventHandshakeClientID(uint clientID) : base(NetworkEventID.HandshakeClientID)
        {
            this.clientID = clientID;
        }

    }

}
