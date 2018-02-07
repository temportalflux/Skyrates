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

        /// <summary>
        /// The location/physics information for the target.
        /// </summary>
        [BitSerialize(0)]
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

    }

}
