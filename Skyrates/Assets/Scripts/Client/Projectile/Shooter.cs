﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    public GameObject projectilePrefab;

    public Transform spawn;

    private void Start()
    {
        Debug.Assert(this.projectilePrefab.GetComponent<Projectile>() != null,
            "The projectilePrefab of a Shooter MUST have the Projectile MonoBehaviour.");
    }

    public Projectile fireProjectile()
    {
        GameObject gameObject = Instantiate(this.projectilePrefab, this.spawn.position, this.spawn.rotation);

        Projectile projectile = gameObject.GetComponent<Projectile>();
        projectile.init(this);
        projectile.addForce(this.transform.forward * 1000);

        return projectile;
    }

}
