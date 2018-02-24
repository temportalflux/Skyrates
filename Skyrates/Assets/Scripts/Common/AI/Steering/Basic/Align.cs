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
            // Get the naive direction to the target
            Vector3 direction = data.Target.RotationPosition.eulerAngles - physics.RotationPosition.eulerAngles;
            
            // Map the result to the (-pi, pi) interval
            
            // TODO: Complete align steering

        }

    }

}
