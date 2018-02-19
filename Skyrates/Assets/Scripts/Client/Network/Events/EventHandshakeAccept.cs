using System;
using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;

namespace Skyrates.Client.Network.Event
{

    /// <summary>
    /// Event dispatched from client to server, to accept a client connection data from server.
    /// </summary>
    [NetworkEvent(Side.Client, Side.Server)]
    public class EventHandshakeAccept : NetworkEvent
    {

        /// <summary>
        /// The clientID of this client.
        /// </summary>
        [BitSerialize(1)]
        public uint ClientID;

        /// <summary>
        /// 
        /// </summary>
        public EventHandshakeAccept() : base(NetworkEventID.HandshakeAccept)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientID"></param>
        public EventHandshakeAccept(uint clientID) : this()
        {
            this.ClientID = clientID;
        }

    }


}
