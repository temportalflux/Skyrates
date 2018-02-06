using Skyrates.Common.AI;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    /// <summary>
    /// Any entity in the world which moves around.
    /// By default, this object uses <see cref="Skyrates.Common.AI.Steering"/>, and it is
    /// the assumption that objects which move will use some form of steering.
    /// </summary>
    public class EntityDynamic : Entity
    {

        /// <summary>
        /// The current physics data of this object.
        /// Updated via <see cref="Steering"/>.
        /// </summary>
        [BitSerialize(2)]
        public PhysicsData Physics = new PhysicsData();

        /// <summary>
        /// The steering data used - info which is specific to this
        /// entity and likely used by multiple steering algorithms.
        /// </summary>
        [BitSerialize(3)]
        public SteeringData SteeringData;

        /// <summary>
        /// The actual steering object - set via editor.
        /// </summary>
        public Steering Steering;
        
        void FixedUpdate()
        {
            
            // Update steering on a fixed timestep
            if (this.Steering != null)
            {
                this.Steering.GetSteering(this.SteeringData, ref this.Physics);
            }

            // Integrate physics from steering and any network updates
            this.IntegratePhysics(Time.fixedDeltaTime);

        }

        /// <summary>
        /// Integrates the current set of physics with the <see cref="Transform"/>.
        /// </summary>
        /// <param name="deltaTime"></param>
        private void IntegratePhysics(float deltaTime)
        {

            // Update velocity
            this.Physics.LinearVelocity += this.Physics.LinearAccelleration * deltaTime;

            // Update position
            this.Physics.LinearPosition += this.Physics.LinearVelocity * deltaTime;

            // Set position
            this.transform.position = this.Physics.LinearPosition;

            // Update rotational velocity
            this.Physics.RotationVelocity = Quaternion.Euler(
                this.Physics.RotationVelocity.eulerAngles +
                this.Physics.RotationAccelleration.eulerAngles * deltaTime
            );

            // Update rotation
            this.Physics.RotationPosition = Quaternion.Euler(
                this.Physics.RotationPosition.eulerAngles +
                this.Physics.RotationVelocity.eulerAngles * deltaTime
            );

            // Set rotation
            this.transform.rotation = this.Physics.RotationPosition;

        }

    }

}
