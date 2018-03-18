using System;
using Skyrates.Client.Entity;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Game;
using UnityEngine;

namespace Skyrates.Client.Mono
{
    
    /// <summary>
    /// Special component which dispatches the <see cref="EventSpawnEntityProjectile"/> game event.
    /// </summary>
    public class Shooter : MonoBehaviour
    {

        /// <summary>
        /// The prefab of the projectile to launch from this source.
        /// </summary>
        [SerializeField]
        public Common.Entity.Entity projectilePrefab;

        /// <summary>
        /// The spawn location of the projectile.
        /// </summary>
        public Transform spawn;

        /// <summary>
        /// How much force to apply to the projectile.
        /// </summary>
        public float force = 1;

        /// <summary>
        /// Returns the direction the shooter is facing.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetProjectileDirection()
        {
            return this.spawn.forward;
        }

        /// <summary>
        /// Fires off <see cref="EventSpawnEntityProjectile"/> with the spawn location, the projectile to fire, and the initial velocity.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="launchVelocity"></param>
        public void FireProjectile(Vector3 direction, Vector3 launchVelocity)
        {
            // TODO: These are fired off one by one, and are often done in batches. This should just be one packet of all the projectiles to spawn.
            Common.Entity.Entity entity = GameManager.Instance.SpawnEntity(this.projectilePrefab); //TODO: Use an object pool if possible.
            if (entity != null)
            {
                EntityProjectile projectile = (EntityProjectile)entity;
                projectile.Launch(this.spawn.position, this.spawn.rotation, launchVelocity, direction * this.force);
            }
        }
	}
}