using System;
using Skyrates.Common.Network;

namespace Skyrates.Client.Network.Event
{

    [NetworkEvent(Side.Client, Side.Server)]
    public class EventHandshakeAccept : NetworkEvent
    {

        [BitSerialize(1)]
        public uint clientID;
        
        [BitSerialize(2)]
        public Guid playerEntityGuid;

        public EventHandshakeAccept(uint clientID, Guid playerEntityGuid) : base(NetworkEventID.HandshakeAccept)
        {
            this.clientID = clientID;
            this.playerEntityGuid = playerEntityGuid;
        }

    }


}
