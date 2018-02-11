using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    public class EventEntityShipHitByProjectile : EventEntityShipDamaged
    {

        public EntityProjectile Projectile;

        public EventEntityShipHitByProjectile(EntityShip ship, EntityProjectile entityProjectile)
            : base(GameEventID.EntityShipHitByProjectile, ship, entityProjectile.GetDamage())
        {
            this.Projectile = entityProjectile;
        }
        
    }

}