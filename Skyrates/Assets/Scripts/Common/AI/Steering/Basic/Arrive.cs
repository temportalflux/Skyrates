
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// Accelerates to the target and slows down as it gets closer.
    /// 
    /// Derived from pg 61 of
    /// Artifical Intelligence for Games 2nd Edition
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Basic/Arrive")]
    public class Arrive : Steering
    {

        /// <summary>
        /// Acceleration coefficient in the target direction.
        /// </summary>
        public float MaxAcceleration;

        /// <summary>
        /// The maximum nominal velocity.
        /// </summary>
        public float MaxSpeed;

        /// <summary>
        /// The radius at which to start slowing down.
        /// </summary>
        public float DistanceArriving;

        /// <summary>
        /// The radius at which to stop.
        /// </summary>
        public float DistanceArrived;

        /// <summary>
        /// The amount of time it takes to accelerate.
        /// </summary>
        public float AccelerationTime;

        /// <inheritdoc />
        public override PhysicsData GetUpdate(BehaviorData data, PhysicsData physics, float deltaTime)
        {
            // Direction to the target
            Vector3 direction = data.Target.LinearPosition - physics.LinearPosition;
            // Square distance to the target
            float distanceSqr = direction.sqrMagnitude;

            // Check if we are there, return no steering
            if (distanceSqr < this.DistanceArrived * this.DistanceArrived)
            {
                // Slow down till stopped
                physics.LinearAccelleration = Vector3.zero - physics.LinearVelocity;
                physics.LinearAccelleration /= this.AccelerationTime;
                return physics;
            }

            float targetSpeed;

            // If we are outside the slowRadius, then go max speed
            if (distanceSqr > this.DistanceArriving * this.DistanceArriving)
            {
                targetSpeed = this.MaxSpeed;
            }
            // Otherwise calculate a scaled speed
            else
            {
                targetSpeed = this.MaxSpeed * distanceSqr / (this.DistanceArriving * this.DistanceArriving);
            }

            // The target velocity combines speed and direction
            Vector3 targetVelocity = direction;
            targetVelocity.Normalize();
            targetVelocity *= targetSpeed;

            // Acceleration tries to get to the target velocity
            physics.LinearAccelleration = targetVelocity - physics.LinearVelocity;
            physics.LinearAccelleration /= this.AccelerationTime;

            // Check if the acceleration is too fast
            if (physics.LinearAccelleration.sqrMagnitude > this.MaxAcceleration * this.MaxAcceleration)
            {
                physics.LinearAccelleration.Normalize();
                physics.LinearAccelleration *= this.MaxAcceleration;
            }

            return physics;
        }

    }

}
