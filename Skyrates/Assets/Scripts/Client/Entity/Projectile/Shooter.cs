using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Network.Event;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    public Entity.TypeData projectilePrefab;

    public Transform spawn;

    public float force = 1;
    
    public void FireProjectile(Vector3 direction, Vector3 launchVelocity)
    {
        // TODO: These are fired off one by one, and are often done in batches. This should just be one packet of all the projectiles to spawn.
        NetworkComponent.GetNetwork().Dispatch(new EventRequestSpawnEntityProjectile(this.projectilePrefab, this.spawn, direction * this.force + launchVelocity));
    }

}
