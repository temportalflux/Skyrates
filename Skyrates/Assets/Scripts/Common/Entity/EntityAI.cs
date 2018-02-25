using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.AI;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    public class EntityAI : EntityDynamic
    {

        /// <summary>
        /// The actual steering object - set via editor.
        /// </summary>
        [SerializeField]
        public Behavior[] Steering;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            // Update steering on a fixed timestep
            foreach (Behavior behavior in this.Steering)
            {
                if (behavior != null)
                {
                    this.Physics = behavior.GetUpdate(this.BehaviorData, this.Physics);
                }
            }

            // Integrate physics from steering and any network updates
            this.IntegratePhysics(Time.fixedDeltaTime);

        }

        /// <summary>
        /// Integrates the current set of physics with the <see cref="Transform"/>.
        /// </summary>
        /// <param name="deltaTime"></param>
        protected virtual void IntegratePhysics(float deltaTime)
        {
            if (this._physics == null)
                return;

            //this.transform.position = this.Physics.LinearPosition;

            // Update velocity
            this.Integrate(ref this.Physics.LinearVelocity, this.Physics.LinearAccelleration, deltaTime);

            // Update position
            // https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
            //this._characterController.Move(this.Physics.LinearVelocity * deltaTime);
            this._physics.velocity = this.Physics.LinearVelocity;

            // Update physics position
            this.Physics.LinearPosition = this.transform.position;

            // Update rotational velocity
            this.Integrate(ref this.Physics.RotationVelocity, this.Physics.RotationAccelleration, deltaTime);

            // Update rotation
            this.GetRender().transform.Rotate(this.Physics.RotationVelocity.eulerAngles, Space.Self);
            //this.GetRender().AddTorque(this.Physics.RotationVelocity.eulerAngles * deltaTime, ForceMode.VelocityChange);

            // Set rotation
            this.Physics.RotationPosition = this.GetRender().rotation;

        }

        /// <summary>
        /// Integrates a Vector3 by another Vector3 over time.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="amount"></param>
        /// <param name="deltaTime"></param>
        private void Integrate(ref Vector3 start, Vector3 amount, float deltaTime)
        {
            // TODO: Move to an extension method.
            start += amount * deltaTime;
        }

        /// <summary>
        /// Integrates a Quaternion by another Quaternion over time.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="amount"></param>
        /// <param name="deltaTime"></param>
        private void Integrate(ref Quaternion start, Quaternion amount, float deltaTime)
        {
            // TODO: Move to an extension method.
            Vector3 euler = start.eulerAngles;
            this.Integrate(ref euler, amount.eulerAngles, deltaTime);
            start = Quaternion.Euler(euler);
        }

    }

}
