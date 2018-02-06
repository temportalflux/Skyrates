using Skyrates.Common.Network;

namespace Skyrates.Common.Network.Event
{

    /// <summary>
    /// The message sent when a peer has disconnected from the network.
    /// </summary>
    [NetworkEvent(Side.Client, Side.Server)]
    [NetworkEvent(Side.Server, Side.Client)]
    public class EventDisconnect : NetworkEvent
    {

        /// <summary>
        /// When sent from the client, this is the identifier in the
        /// client's <see cref="Session"/>.
        /// When sent by the server, this is 0.
        /// </summary>
        [BitSerialize(1)]
        public uint clientID;

        // deserialize
        public EventDisconnect() : base(NetworkEventID.Disconnect) { }

        // dispatch
        public EventDisconnect(uint clientID) : this()
        {
            this.clientID = clientID;
        }

    }

}
