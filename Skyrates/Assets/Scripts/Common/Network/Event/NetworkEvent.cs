using System;
using System.Linq;
using Skyrates.Common.Network;

namespace Skyrates.Common.Network.Event
{

    /// <summary>
    /// Any packet/message/event which is sent over the network.
    /// </summary>
    public abstract class NetworkEvent
    {

        /// <summary>
        /// The identifier of this event.
        /// Will always line up with something in <see cref="NetworkEventID"/>.
        /// </summary>
        [BitSerialize]
        public byte EventId;

        /// <summary>
        /// The is the IP address of the source, usually IPv4.
        /// </summary>
        public string SourceAddress;

        /// <summary>
        /// How long it took to transmit this packet from dispatcher to receiver in ms.
        /// </summary>
        public float TransmitTime;

        // Dispatch
        protected NetworkEvent(NetworkEventID id)
        {
            this.EventId = (byte)id;
        }

        // Deserialize
        protected NetworkEvent() { }

        public virtual byte[] Serialize()
        {
            return BitSerializeAttribute.Serialize(this);
        }

        public virtual void Deserialize(byte[] data)
        {
            BitSerializeAttribute.Deserialize(this, data);
        }

        public NetworkEventAttribute GetAttribute()
        {
            return this.GetType().GetCustomAttributes(
                typeof(NetworkEventAttribute), true).FirstOrDefault() as NetworkEventAttribute;
        }

        public bool IsSentBy(Side side)
        {
            var attribute = this.GetAttribute();
            return attribute != null && attribute.GetSender() == side;
        }

        public bool IsReceivedBy(Side side)
        {
            var attribute = this.GetAttribute();
            return attribute != null && attribute.GetReceiver() == side;
        }

    }

}
