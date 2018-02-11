using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    public GameObject projectilePrefab;

    public Transform spawn;

    public float force = 1;

    private void Start()
    {
        Debug.Assert(this.projectilePrefab.GetComponent<Projectile>() != null,
            "The projectilePrefab of a Shooter MUST have the Projectile MonoBehaviour.");
    }

    public Projectile fireProjectile(Vector3 direction, Vector3 launchVelocity)
    {
        GameObject gameObject = Instantiate(this.projectilePrefab, this.spawn.position, this.spawn.rotation);

        Projectile projectile = gameObject.GetComponent<Projectile>();
        projectile.Physics.velocity = launchVelocity;
        projectile.AddForce(direction * this.force);

        return projectile;
    }

}
