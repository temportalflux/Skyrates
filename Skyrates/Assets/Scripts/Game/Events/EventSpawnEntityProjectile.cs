using Skyrates.Game.Event;
using UnityEngine;

namespace Skyrates.Client.Game.Event
{

    /// <summary>
    /// Dispatched when a projectile should be spawned.
    /// </summary>
    public class EventSpawnEntityProjectile : GameEvent
    {
        
        /// <summary>
        /// The data of the projectile.
        /// </summary>
        public GameObject Prefab;

        /// <summary>
        /// The location/rotation/scale of the projectile.
        /// </summary>
        public Transform Spawn;

        /// <summary>
        /// The current velocity of the projectile.
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// The force to apply as initial velocity to the projectile.
        /// </summary>
        public Vector3 ImpluseForce;

        public EventSpawnEntityProjectile(GameObject prefab, Transform spawn, Vector3 velocity, Vector3 impluseForce) : base(GameEventID.SpawnEntityProjectile)
        {
            this.Prefab = prefab;
            this.Spawn = spawn;
            this.Velocity = velocity;
            this.ImpluseForce = impluseForce;
        }

    }

}
