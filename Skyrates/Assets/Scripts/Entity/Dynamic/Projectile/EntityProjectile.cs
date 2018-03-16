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

		[Tooltip("The base damage of the projectile")]
		public float Attack;

		protected override void Start()
        {
            base.Start();
            this.PhysicsComponent = this.GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Gets the base damage of the projectile.
        /// </summary>
        /// <returns>The base damage of the projectile</returns>
        public float GetAttack()
        {
            return this.Attack;
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
            this.PhysicsData.LinearPosition = this.transform.position;
            this.PhysicsData.LinearVelocity = this.PhysicsComponent.velocity;
            this.PhysicsData.RotationPosition = this.transform.rotation;
        }

    }


}
