using UnityEngine;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// Accellerates towards a rotation, and slows as it gets closer.
    /// 
    /// Derived from pg 64 of
    /// Artifical Intelligence for Games 2nd Edition
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Basic/Align")]
    public class Align : Steering
    {

        /// <summary>
        /// The maximum nominal acceleration.
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
        public override void GetSteering(SteeringData data, ref PhysicsData physics)
        {
            float currentRotation = physics.RotationPosition.eulerAngles.y;

            // Get the naive direction to the target
            float rotation = data.Target.RotationPosition.eulerAngles.y - currentRotation;

            // Map the result to the (-pi, pi) interval
            rotation = MapToRange(rotation);
            float rotationSize = Mathf.Abs(rotation);

            float accelleration;
            // Check if we are there, return no ste4ering
            if (rotationSize < this.DistanceArrived)
            {
                // Slow down till stopped
                accelleration = 0 - physics.RotationVelocity.eulerAngles.y;
                accelleration /= this.AccelerationTime;
                physics.RotationAccelleration = Quaternion.Euler(0, accelleration, 0);
                return;
            }

            // velocity of rotation
            float targetRotation;

            // If we are outside the slowRadius, then use maximum rotation
            if (rotationSize > this.DistanceArriving)
            {
                targetRotation = this.MaxSpeed;
            }
            // Otherwise calculate a scaled rotation
            else
            {
                targetRotation = this.MaxSpeed * rotationSize / this.DistanceArriving;
            }

            // The final target rotation combines speed and direction
            targetRotation *= rotation / rotationSize;

            // Acceleration tries to get to the target rotation
            accelleration = targetRotation - physics.RotationVelocity.eulerAngles.y;
            accelleration /= this.AccelerationTime;

            // Check if the accelleration is too great
            float absAccel = Mathf.Abs(accelleration);
            if (absAccel > this.MaxAcceleration)
            {
                accelleration /= absAccel;
                accelleration *= this.MaxAcceleration;
            }

            physics.RotationAccelleration = Quaternion.Euler(0, accelleration, 0);
        }

        private float MapToRange(float rotation)
        {
            rotation %= 360.0f;
            if (Mathf.Abs(rotation) > 180.0f)
            {
                if (rotation < 0.0f)
                    rotation += 360.0f;
                else
                    rotation -= 360.0f;
            }
            return rotation;
        }

    }

}
