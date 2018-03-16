using Skyrates.Common.AI;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.AI.Steering
{

    /// <summary>
    /// The base class for all steering algorithms.
    /// This is object which <see cref="EntityDynamic"/> uses to control its movement.
    /// It is inspector worthy (easy for designers to drag and drop).
    /// </summary>
    public abstract class Steering : Behavior
    {

        /// <summary>
        /// Returns the orientation to look at if velocity is present.
        /// 
        /// Derived from https://gamedev.stackexchange.com/questions/121469/unity3d-smooth-rotation-for-seek-steering-behavior.
        /// Derived from pg 49 of
        /// Artifical Intelligence for Games 2nd Edition
        /// Ian Millington & John Funge
        /// </summary>
        /// <param name="physics"></param>
        /// <returns></returns>
        public Quaternion GetLookOrientation(PhysicsData physics)
        {
            Vector3 velocity = physics.LinearVelocity;
            // Check if the velocity is non-zero (checking magnitude for 0 is also valid for checking sqrMagnitude, which does not have a sqrt)
            if (velocity.sqrMagnitude > 0)
            {
                //float rotationGlobalY = Mathf.Atan2(-velocity.x, velocity.z);
                return Quaternion.LookRotation(velocity);
            }
            return physics.RotationPosition;
        }

    }

}
