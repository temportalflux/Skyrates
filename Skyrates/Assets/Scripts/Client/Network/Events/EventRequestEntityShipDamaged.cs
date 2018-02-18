using System;
using Skyrates.Client.Entity;
using Skyrates.Client.Game.Event;
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
        public int SourceEntityTypeAsInt;

        [BitSerialize(3)]
        public Guid SourceEntityGuid;

        [BitSerialize(4)]
        public int TargetEntityTypeAsInt;

        [BitSerialize(5)]
        public Guid TargetEntityGuid;

        [BitSerialize(6)]
        public uint ProjectileOrRam; // 0 = proj, 1 = ram

        [BitSerialize(7)]
        public float Damage;

        public Common.Entity.Entity.Type SourceEntityType
        {
            get { return (Common.Entity.Entity.Type)this.SourceEntityTypeAsInt; }
            set { this.SourceEntityTypeAsInt = (int)value; }
        }

        public Common.Entity.Entity.Type TargetEntityType
        {
            get { return (Common.Entity.Entity.Type)this.TargetEntityTypeAsInt; }
            set { this.TargetEntityTypeAsInt = (int)value; }
        }

        public EventRequestEntityShipDamaged() : base(NetworkEventID.RequestEntityShipDamaged)
        {
        }

        private EventRequestEntityShipDamaged(Common.Entity.Entity source, EntityShip target, uint projectileOrRam, float damage) : this()
        {
            this.clientID = NetworkComponent.GetSession.ClientID;
            this.SourceEntityType = source.EntityType.EntityType;
            this.SourceEntityGuid = source.Guid;
            this.TargetEntityType = target.EntityType.EntityType;
            this.TargetEntityGuid = target.Guid;
            this.ProjectileOrRam = projectileOrRam;
            this.Damage = damage;
        }

        public static EventRequestEntityShipDamaged Projectile(Common.Entity.Entity source, EntityShip target, float damage)
        {
            return new EventRequestEntityShipDamaged(source, target, 0, damage);
        }

        public static EventRequestEntityShipDamaged Ram(Common.Entity.Entity source, EntityShip target, float damage)
        {
            return new EventRequestEntityShipDamaged(source, target, 1, damage);
        }

        public EventEntityShipDamaged GetAsGameEvent(Common.Entity.Entity source, EntityShip target)
        {
            switch (this.ProjectileOrRam)
            {
                case 0:
                    return new EventEntityShipHitByProjectile(target, (EntityProjectile)source);
                case 1:
                    return new EventEntityShipHitByRam(target, ((EntityShip)source).GetFigurehead());
                default:
                    return null;
            }
        }

    }

}
