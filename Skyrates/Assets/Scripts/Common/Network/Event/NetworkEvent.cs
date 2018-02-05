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

    }

}
