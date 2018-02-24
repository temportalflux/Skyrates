using System;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// Any generic data used by all steering behaviours and that is
    /// specific to each <see cref="EntityDynamic"/> instance.
    /// MUST be network-safe/serializable.
    /// Basically any information that is not algorithm specifc and is pertinent to the owner.
    /// </summary>
    [Serializable]
    public class SteeringData
    {

        public Transform View;
        public Transform Render;

        public bool HasTarget = false;

        /// <summary>
        /// The location/physics information for the target.
        /// </summary>
        [SerializeField]
        public PhysicsData Target = new PhysicsData();

    }

    /// <summary>
    /// The base class for all steering algorithms.
    /// This is object which <see cref="EntityDynamic"/> uses to control its movement.
    /// It is inspector worthy (easy for designers to drag and drop).
    /// </summary>
    public abstract class Steering : ScriptableObject
    {

        /// <summary>
        /// Called to request updated physics. Recommend running on a fixed time step.
        /// </summary>
        /// <param name="data">data specfic to the owner</param>
        /// <param name="physics">the data that is steering the owner</param>
        public abstract void GetSteering(SteeringData data, ref PhysicsData physics);

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
