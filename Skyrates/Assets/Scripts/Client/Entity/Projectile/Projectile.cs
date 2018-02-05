using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{

    // used to identify the owner (id is unique) (so this object doesn't collide with it)
    private int ownerInstanceID;

    // TODO: Move outside strictly physics
    public Vector3 constantForce;
    private Rigidbody physics;

    void Start()
    {
        this.physics = this.GetComponent<Rigidbody>();
    }

    public void init(Shooter owner)
    {
        this.ownerInstanceID = owner.GetInstanceID();
    }

    void FixedUpdate()
    {
        // TODO: cache
        this.physics.position += this.constantForce * Time.deltaTime;
    }

    public void addForce(Vector3 force)
    {
        this.GetComponent<Rigidbody>().AddForce(force, ForceMode.Acceleration);
    }

}
