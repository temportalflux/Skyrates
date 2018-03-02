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
    public class BehaviorData
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
    /// The base class for all ai behaviors.
    /// </summary>
    public abstract class Behavior : ScriptableObject
    {
        /// <summary>
        /// Called to request updated physics. Recommend running on a fixed time step.
        /// </summary>
        /// <param name="data">data specfic to the owner</param>
        /// <param name="physics">the data that is steering the owner</param>
        /// <param name="deltaTime"></param>
        public abstract PhysicsData GetUpdate(BehaviorData data, PhysicsData physics, float deltaTime);

        /// <summary>
        /// Executed when a state begins execution of this behavior.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        public virtual void OnStateEnter(BehaviorData data, PhysicsData physics)
        {
        }

        /// <summary>
        /// Executed when a state ends execution of this behavior.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        public virtual void OnStateExit(BehaviorData data, PhysicsData physics)
        {
        }

    }

}
