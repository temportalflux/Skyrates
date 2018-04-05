using System;
using Skyrates.Mono;
using UnityEngine;

namespace Skyrates.Ship
{

    /// <summary>
    /// Subclass of <see cref="ShipComponent"/> dedicated to
    /// components of type <see cref="ShipData.ComponentType.Artillery"/>.
    /// </summary>
    public class ShipArtillery : ShipComponent
    {

        [Range(0, 1)]
        public float RateOfFireModifier = 1;
        [Range(0, 1)]
        public float AttackModifier = 1;
        [Range(0, 1)]
        public float DistanceModifier = 1;
        
        public Shooter Shooter;

        public virtual void Shoot(Func<ShipArtillery, Vector3> getDirection, Vector3 velocity)
        {
            this.Shooter.FireProjectile(getDirection(this), velocity, this.AttackModifier, this.DistanceModifier);
        }

    }

}
