using System;
using Skyrates.Common.AI;
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

        /// <summary>
        /// The index of the source <see cref="State"/> in the <see cref="StateMachine"/>.
        /// </summary>
        [HideInInspector]
        public int StateSource;

        /// <summary>
        /// The index of the destination <see cref="State"/> in the <see cref="StateMachine"/>.
        /// </summary>
        [HideInInspector]
        public int StateDestination;

        public Behavior.DataPersistent CreatePersistentData()
        {
            return null;
        }

        /// <summary>
        /// Returns the source <see cref="State"/> of this transition in the specified <see cref="StateMachine"/>.
        /// </summary>
        /// <param name="machine"></param>
        /// <returns></returns>
        public State GetStateSource(StateMachine machine)
        {
            return machine.GetState(this.StateSource);
        }

        /// <summary>
        /// Returns the destination <see cref="State"/> of this transition in the specified <see cref="StateMachine"/>.
        /// </summary>
        /// <param name="machine"></param>
        /// <returns></returns>
        public State GetStateDestination(StateMachine machine)
        {
            return machine.GetState(this.StateDestination);
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
        public virtual void DrawGizmos(Behavior.DataPersistent persistent)
        {
            
        }
#endif

    }

}
