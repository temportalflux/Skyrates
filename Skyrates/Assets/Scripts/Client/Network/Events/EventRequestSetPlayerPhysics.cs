using Skyrates.Common.AI;
using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;

namespace Skyrates.Client.Network.Event
{

    [NetworkEvent(Side.Client, Side.Server)]
    public class EventRequestSetPlayerPhysics : NetworkEvent
    {

        [BitSerialize(1)] public uint clientID;

        [BitSerialize(2)] public PhysicsData data;

        public EventRequestSetPlayerPhysics() : base(NetworkEventID.RequestSetPlayerPhysics)
        {
        }

        public EventRequestSetPlayerPhysics(PhysicsData dataIn) : this()
        {
            this.clientID = NetworkComponent.GetSession.ClientID;
            this.data = dataIn;
        }

    }

}
