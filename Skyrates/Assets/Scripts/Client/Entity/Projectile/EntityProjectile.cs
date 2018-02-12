using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EntityProjectile : EntityDynamic
{
    
    [HideInInspector]
    public Rigidbody PhysicsComponent;

    protected override void Start()
    {
        base.Start();
        this.PhysicsComponent = this.GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 impulseForce)
    {
        this.transform.SetPositionAndRotation(position, rotation);
        this.GetComponent<Rigidbody>().velocity = velocity;
        this.AddForce(impulseForce);
    }
    
    public void AddForce(Vector3 force)
    {
        this.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }

    protected override void FixedUpdate()
    {
        this.Physics.LinearPosition = this.transform.position;
        this.Physics.LinearVelocity = this.PhysicsComponent.velocity;
        this.Physics.RotationPosition = this.transform.rotation;
    }

    public float GetDamage()
    {
        // TODO: Implement projectile damage
        return 1;
    }

}
