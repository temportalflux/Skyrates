using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Network.Event;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    [SerializeField]
    public Entity.TypeData projectilePrefab;

    public Transform spawn;

    public float force = 1;

    public Vector3 ProjectileDirection()
    {
        return this.spawn.forward;
    }

    public void FireProjectile(Vector3 direction, Vector3 launchVelocity)
    {
        // TODO: These are fired off one by one, and are often done in batches. This should just be one packet of all the projectiles to spawn.
        GameManager.Events.Dispatch(new EventSpawnEntityProjectile(this.projectilePrefab, this.spawn, launchVelocity, direction * this.force));
    }

}
