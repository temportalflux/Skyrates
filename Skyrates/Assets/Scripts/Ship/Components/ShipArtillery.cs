﻿using System;
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
        
        public Shooter Shooter;

        public virtual void Shoot(Func<Shooter, Vector3> getDirection, Vector3 velocity)
        {
            this.Shooter.FireProjectile(getDirection(this.Shooter), velocity);
        }

    }

}
