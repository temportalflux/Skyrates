using Skyrates.Client.Entity;
using Skyrates.Common.AI;
using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;

namespace Skyrates.Client.Network.Event
{

    /// <summary>
    /// Dispatched by client to update the location/physics of their <see cref="EntityPlayerShip"/> over the network
    /// </summary>
    [NetworkEvent(Side.Client, Side.Server)]
    public class EventRequestSetPlayerPhysics : NetworkEvent
    {

        /// <summary>
        /// The client's network ID
        /// </summary>
        [BitSerialize(1)]
        public uint ClientID;

        /// <summary>
        /// The physics data associated with the client's <see cref="EntityPlayerShip"/>.
        /// </summary>
        [BitSerialize(2)]
        public PhysicsData Physics;

        /// <summary>
        /// 
        /// </summary>
        public EventRequestSetPlayerPhysics() : base(NetworkEventID.RequestSetPlayerPhysics)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="physicsIn"></param>
        public EventRequestSetPlayerPhysics(PhysicsData physicsIn) : this()
        {
            this.ClientID = NetworkComponent.GetSession.ClientID;
            this.Physics = physicsIn;
        }

    }

}
