using System;
using Skyrates.Client.Entity;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Ship;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;

namespace Skyrates.Client.Network.Event
{

    /// <summary>
    /// Event dispatched to server when an <see cref="EntityShip"/> a client owns should be damaged.
    /// </summary>
    [NetworkEvent(Side.Client, Side.Server)]
    public class EventRequestEntityShipDamaged : NetworkEvent
    {

        /// <summary>
        /// The client's network ID
        /// </summary>
        [BitSerialize(1)]
        public uint ClientID;

        /// <summary>
        /// The <see cref="Entity.Type"/> (as an int) of the entity causing the damage
        /// </summary>
        [BitSerialize(2)]
        public int SourceEntityTypeAsInt;

        /// <summary>
        /// The <see cref="Guid"/> of the entity causing the damage
        /// </summary>
        [BitSerialize(3)]
        public Guid SourceEntityGuid;

        /// <summary>
        /// The <see cref="Entity.Type"/> (as an int) of the entity taking the damage
        /// </summary>
        [BitSerialize(4)]
        public int TargetEntityTypeAsInt;

        /// <summary>
        /// The <see cref="Guid"/> of the entity taking the damage
        /// </summary>
        [BitSerialize(5)]
        public Guid TargetEntityGuid;

        /// <summary>
        /// 0 if a projectile is causing the damage.
        /// 1 if an <see cref="EntityShip"/>'s <see cref="ShipFigurehead"/> is causing the damage.
        /// </summary>
        [BitSerialize(6)]
        public uint ProjectileOrRam; // 0 = proj, 1 = ram

        /// <summary>
        /// The amount of damage to cause.
        /// </summary>
        [BitSerialize(7)]
        public float Damage;

        /// <summary>
        /// The <see cref="Entity.Type"/> of the entity causing the damage
        /// </summary>
        public Common.Entity.Entity.Type SourceEntityType
        {
            get { return (Common.Entity.Entity.Type)this.SourceEntityTypeAsInt; }
            set { this.SourceEntityTypeAsInt = (int)value; }
        }

        /// <summary>
        /// The <see cref="Entity.Type"/> of the entity taking the damage
        /// </summary>
        public Common.Entity.Entity.Type TargetEntityType
        {
            get { return (Common.Entity.Entity.Type)this.TargetEntityTypeAsInt; }
            set { this.TargetEntityTypeAsInt = (int)value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public EventRequestEntityShipDamaged() : base(NetworkEventID.RequestEntityShipDamaged)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="projectileOrRam"></param>
        /// <param name="damage"></param>
        private EventRequestEntityShipDamaged(Common.Entity.Entity source, EntityShip target, uint projectileOrRam, float damage) : this()
        {
            this.ClientID = NetworkComponent.GetSession.ClientID;
            this.SourceEntityType = source.EntityType.EntityType;
            this.SourceEntityGuid = source.Guid;
            this.TargetEntityType = target.EntityType.EntityType;
            this.TargetEntityGuid = target.Guid;
            this.ProjectileOrRam = projectileOrRam;
            this.Damage = damage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        /// <returns></returns>
        public static EventRequestEntityShipDamaged Projectile(Common.Entity.Entity source, EntityShip target, float damage)
        {
            return new EventRequestEntityShipDamaged(source, target, 0, damage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        /// <returns></returns>
        public static EventRequestEntityShipDamaged Ram(Common.Entity.Entity source, EntityShip target, float damage)
        {
            return new EventRequestEntityShipDamaged(source, target, 1, damage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
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
