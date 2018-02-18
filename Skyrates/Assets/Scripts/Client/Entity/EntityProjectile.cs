using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Entity
{

    /// <summary>
    /// A moving entity which has no steering, but a constant/trajectory force.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EntityProjectile : EntityDynamic
    {

        /// <summary>
        /// The component which controls physics.
        /// </summary>
        [HideInInspector]
        public Rigidbody PhysicsComponent;

        /// <summary>
        /// The baseline amount of damage this projectile does.
        /// </summary>
        public float Damage = 2;

        protected override void Start()
        {
            base.Start();
            this.PhysicsComponent = this.GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Applies initial velocity/position/rotation information.
        /// Should be called during object instantiation.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="velocity"></param>
        /// <param name="impulseForce"></param>
        public void Launch(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 impulseForce)
        {
            this.transform.SetPositionAndRotation(position, rotation);
            this.GetComponent<Rigidbody>().velocity = velocity;
            this.AddForce(impulseForce);
        }

        /// <summary>
        /// Adds force to the projectile as if it was being launched (applies via <see cref="Rigidbody"/> with <see cref="ForceMode.Impulse"/>).
        /// </summary>
        /// <param name="force"></param>
        public void AddForce(Vector3 force)
        {
            this.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }

        /// <summary>
        /// Unity event - overrides things done in <see cref="EntityDynamic.FixedUpdate"/> in order to just set the physics data for network updating.
        /// </summary>
        protected override void FixedUpdate()
        {
            // TODO: this may inhibit networking if the entity is not owned locally
            this.Physics.LinearPosition = this.transform.position;
            this.Physics.LinearVelocity = this.PhysicsComponent.velocity;
            this.Physics.RotationPosition = this.transform.rotation;
        }

        /// <summary>
        /// Returns the amount of damage the projectile does to things it hits.
        /// </summary>
        /// <returns></returns>
        public float GetDamage()
        {
            return this.Damage;
        }

    }


}
