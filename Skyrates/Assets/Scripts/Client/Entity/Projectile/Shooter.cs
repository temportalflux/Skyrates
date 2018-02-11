using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Network.Event;
using Skyrates.Common.Network;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    public EntityProjectile projectilePrefab;

    public Transform spawn;

    public float force = 1;

    private void Start()
    {
        Debug.Assert(this.projectilePrefab.GetComponent<EntityProjectile>() != null,
            "The projectilePrefab of a Shooter MUST have the Projectile MonoBehaviour.");
    }

    public void FireProjectile(Vector3 direction, Vector3 launchVelocity)
    {
        // TODO: These are fired off one by one, and are often done in batches. This should just be one packet of all the projectiles to spawn.
        NetworkComponent.GetNetwork().Dispatch(new EventRequestSpawnEntityProjectile(this.projectilePrefab.EntityType, this.spawn, direction * this.force + launchVelocity));
    }

}
