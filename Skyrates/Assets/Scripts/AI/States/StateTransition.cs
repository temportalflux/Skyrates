using System;
using Skyrates.Entity;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.State
{

    /// <summary>
    /// Specifies when a state is able to be transitioned into.
    /// </summary>
    [Serializable]
    public abstract class StateTransition : ScriptableObject
    {

        public virtual Behavior.DataPersistent CreatePersistentData()
        {
            return new Behavior.DataPersistent();
        }
        
        /// <summary>
        /// Returns if a <see cref="StateMachine"/> can transition from the <see cref="StateSource"/> to the <see cref="StateDestination"/>.
        /// </summary>
        /// <param name="behavioralData"></param>
        /// <param name="physics"></param>
        /// <returns></returns>
        public abstract bool CanEnter(Behavior.DataBehavioral behavioralData, PhysicsData physics, ref Behavior.DataPersistent persistent);

        public virtual void OnDetect(EntityAI other, float distance)
        {
        }

#if UNITY_EDITOR
        public virtual void DrawGizmos(PhysicsData physics, Behavior.DataPersistent persistent)
        {

        }
        public virtual void DrawGizmosSelected(PhysicsData physics, Behavior.DataPersistent persistent)
        {

        }
#endif

    }

}
