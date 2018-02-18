using Skyrates.Client.Entity;
using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    /// <summary>
    /// A special case of causing damage to a ship via projectile.
    /// </summary>
    public class EventEntityShipHitByProjectile : EventEntityShipDamaged
    {

        /// <summary>
        /// The projectile which caused the damage.
        /// </summary>
        public EntityProjectile Projectile
        {
            get { return (EntityProjectile) this.Source; }
        }

        public EventEntityShipHitByProjectile(EntityShip target, EntityProjectile entityProjectile)
            : base(GameEventID.EntityShipHitByProjectile, entityProjectile, target, entityProjectile.GetDamage())
        {
        }
        
    }

}