using System.Collections;
using System.Collections.Generic;
using Skyrates.AI;
using UnityEngine;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// A state in the overarching machine.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Control/State")]
    public class State : ScriptableObject
    {

        public string StateName;

        /// <summary>
        /// A list of possible transitions.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        public StateTransition[] Transitions;

        /// <summary>
        /// The behavior which runs when this state is active.
        /// </summary>
        [SerializeField]
        public Behavior Behavior;

        /// <summary>
        /// Executed when the state begins execution.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        public void Enter(ref BehaviorData data, PhysicsData physics)
        {
            this.Behavior.OnEnter(ref data, physics);
        }

        /// <summary>
        /// Executed when the state ends execution.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        public void Exit(ref BehaviorData data, PhysicsData physics)
        {
            this.Behavior.OnExit(ref data, physics);
        }

    }

}