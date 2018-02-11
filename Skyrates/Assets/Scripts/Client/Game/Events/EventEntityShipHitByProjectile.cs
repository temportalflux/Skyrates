using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    public class EventEntityShipHitByProjectile : EventEntityShipDamaged
    {

        public Projectile Projectile;

        public EventEntityShipHitByProjectile(EntityShip ship, Projectile projectile)
            : base(GameEventID.EntityShipHitByProjectile, ship, projectile.GetDamage())
        {
            this.Projectile = projectile;
        }
        
    }

}