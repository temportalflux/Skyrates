using System;
using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;

namespace Skyrates.Client.Network.Event
{

    [NetworkEvent(Side.Client, Side.Server)]
    public class EventHandshakeAccept : NetworkEvent
    {

        [BitSerialize(1)]
        public uint clientID;

        public EventHandshakeAccept() : base(NetworkEventID.HandshakeAccept)
        {
        }

        public EventHandshakeAccept(uint clientID) : this()
        {
            this.clientID = clientID;
        }

    }


}
