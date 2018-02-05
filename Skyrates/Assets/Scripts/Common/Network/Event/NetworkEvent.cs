using System;
using System.Linq;
using Skyrates.Common.Network;

namespace Skyrates.Common.Network.Event
{

    public abstract class NetworkEvent
    {

        [BitSerialize]
        public byte eventID;

        public string sourceAddress;

        public float transmitTime;

        // Dispatch
        protected NetworkEvent(NetworkEventID id)
        {
            this.eventID = (byte)id;
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
