using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{

    // used to identify the owner (id is unique) (so this object doesn't collide with it)
    private int ownerInstanceID;

    public void init(Shooter owner)
    {
        this.ownerInstanceID = owner.GetInstanceID();
    }

    public void addForce(Vector3 force)
    {
        this.GetComponent<Rigidbody>().AddForce(force, ForceMode.Acceleration);
    }

}
