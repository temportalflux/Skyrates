using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    
    [HideInInspector]
    public Rigidbody Physics;

    void Start()
    {
        this.Physics = this.GetComponent<Rigidbody>();
    }
    
    public void AddForce(Vector3 force)
    {
        this.GetComponent<Rigidbody>().AddForce(force, ForceMode.Acceleration);
    }

    public float GetDamage()
    {
        // TODO: Implement projectile damage
        return 1;
    }

}
