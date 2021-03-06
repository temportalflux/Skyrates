﻿using Skyrates.Entity;

namespace Skyrates.Game.Event
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

        public EventEntityShipHitByProjectile(EntityShip target, EntityProjectile entityProjectile, float damage)
            : base(GameEventID.EntityShipHitByProjectile, entityProjectile, target, damage)
        {
        }
        
    }

}