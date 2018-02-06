using System.Diagnostics;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;

namespace Skyrates.Server.Network.Event
{

    [NetworkEvent(Side.Server, Side.Client)]
    public class EventUpdateGameState : NetworkEvent, ISerializing
    {

        public EventUpdateGameState() : base(NetworkEventID.UpdateGamestate)
        {}

        /// <inheritdoc />
        public int GetSize()
        {
            EntityTracker tracker = NetworkComponent.GetNetwork().GetEntityTracker();
            tracker.GenerateData();
            return 0
                   // event id
                + sizeof(byte)
                + tracker.GetSize()
                ;
        }

        /// <inheritdoc />
        public void Serialize(ref byte[] data, ref int lastIndex)
        {
            EntityTracker tracker = NetworkComponent.GetNetwork().GetEntityTracker();
            tracker.Serialize(ref data, ref lastIndex);

        }

        /// <inheritdoc />
        public void Deserialize(byte[] data, ref int lastIndex)
        {
            EntityTracker tracker = NetworkComponent.GetNetwork().GetEntityTracker();
            tracker.Deserialize(data, ref lastIndex);

        }

    }

}