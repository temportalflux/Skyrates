using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    public class EventEntityShipHitByProjectile : EventEntityShipDamaged
    {

        public Projectile Projectile;

        public EventEntityShipHitByProjectile(EntityShip ship, Projectile projectile, float damage)
            : base(GameEventID.EntityShipHitByProjectile, ship, damage)
        {
            this.Projectile = projectile;
        }
        
    }

}