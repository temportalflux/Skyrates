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
    [CreateAssetMenu(menuName = "Data/AI/Basic/Lock At")]
    public class LockAt : Steering
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
        public override PhysicsData GetUpdate(BehaviorData data, PhysicsData physics)
        {
            Vector3 direction = data.Target.LinearPosition - physics.LinearPosition;

            Vector3 dir2 = direction.Flatten(Vector3.up);
            if (dir2.magnitude > 0.0f)
            {
                physics.RotationPosition = Quaternion.LookRotation(dir2);
            }

           // Quaternion dirQ = Quaternion.LookRotation(direction);
            //Quaternion slerp = Quaternion.Slerp(physics.RotationPosition, dirQ, direction.magnitude * MaxSpeed * Time.deltaTime);
            //RenderBuffer.MoveRotation(slerp);

            return physics;
        }

    }

}
