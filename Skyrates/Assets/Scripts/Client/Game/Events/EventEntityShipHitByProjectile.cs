using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    public class EventEntityShipHitByProjectile : EventEntityShipDamaged
    {

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