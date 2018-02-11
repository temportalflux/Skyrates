using System;
using Skyrates.Common.AI;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;

namespace Skyrates.Client.Network.Event
{

    [NetworkEvent(Side.Client, Side.Server)]
    public class EventRequestEntityShipDamaged : NetworkEvent
    {

        [BitSerialize(1)]
        public uint clientID;

        [BitSerialize(2)]
        public int EntityTypeAsInt;

        [BitSerialize(3)]
        public Guid EntityGuid;

        [BitSerialize(4)]
        public float Damage;

        public Entity.Type EntityType
        {
            get { return (Entity.Type) this.EntityTypeAsInt; }
            set { this.EntityTypeAsInt = (int) value; }
        }

        public EventRequestEntityShipDamaged() : base(NetworkEventID.RequestEntityShipDamaged)
        {
        }

        public EventRequestEntityShipDamaged(EntityShip entity, float damage) : this()
        {
            this.clientID = NetworkComponent.GetSession.ClientID;
            this.EntityType = entity.EntityType.EntityType;
            this.EntityGuid = entity.Guid;
            this.Damage = damage;
        }

    }

}
