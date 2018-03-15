using Skyrates.AI;
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

        public bool AlignX = false;
        public bool AlignY = true;
        public bool AlignZ = false;

        /// <inheritdoc />
        public override object GetUpdate(ref BehaviorData data, ref PhysicsData physics, float deltaTime, object persistentData)
        {
            if (AlignX)
            {
                this.DoAlign(
                    physics.RotationPosition.eulerAngles.x,
                    physics.RotationVelocity.x,
                    data.Target.RotationPosition.eulerAngles.x,
                    out physics.RotationVelocity.x,
                    out physics.RotationAccelleration.x
                );
            }

            if (AlignY)
            {
                this.DoAlign(
                    physics.RotationPosition.eulerAngles.y,
                    physics.RotationVelocity.y,
                    data.Target.RotationPosition.eulerAngles.y,
                    out physics.RotationVelocity.y,
                    out physics.RotationAccelleration.y
                );
            }

            if (AlignZ)
            {
                this.DoAlign(
                    physics.RotationPosition.eulerAngles.z,
                    physics.RotationVelocity.z,
                    data.Target.RotationPosition.eulerAngles.z,
                    out physics.RotationVelocity.z,
                    out physics.RotationAccelleration.z
                );
            }

            return persistentData;
        }

        private void DoAlign(float currentRotation,
            float currentVelocity, float targetRotation,
            out float velocity, out float accelleration)
        {
            // Get the naive direction to the target
            float rotation = targetRotation - currentRotation;

            // Map the result to the (-pi, pi) interval
            rotation = MapToRange(rotation);
            float rotationSize = Mathf.Abs(rotation);

            // Check if we are there, return no ste4ering
            if (rotationSize < this.DistanceArrived)
            {
                // Slow down till stopped
                accelleration = 0 - currentVelocity;
                accelleration /= this.AccelerationTime;
                velocity = currentVelocity;
                return;
            }

            // If we are outside the slowRadius, then use maximum rotation
            if (rotationSize > this.DistanceArriving)
            {
                velocity = this.MaxSpeed;
            }
            // Otherwise calculate a scaled rotation
            else
            {
                velocity = this.MaxSpeed * rotationSize / this.DistanceArriving;
            }

            // The final target rotation combines speed and direction
            velocity *= rotation / rotationSize;

            // Acceleration tries to get to the target rotation
            accelleration = velocity - currentVelocity;
            accelleration /= this.AccelerationTime;

            // Check if the accelleration is too great
            float absAccel = Mathf.Abs(accelleration);
            if (absAccel > this.MaxAcceleration)
            {
                accelleration /= absAccel;
                accelleration *= this.MaxAcceleration;
            }
            
        }

        public static float MapToRange(float rotation)
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
