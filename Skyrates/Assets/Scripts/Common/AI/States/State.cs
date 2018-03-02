using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// A state in the overarching machine.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Control/State")]
    public class State : ScriptableObject
    {

        /// <summary>
        /// A list of possible triggers.
        /// Any trigger can cause the state to enter,
        /// but all triggers must allow the trigger to exit.
        /// </summary>
        [SerializeField]
        public StateTrigger[] Trigger;

        /// <summary>
        /// The behavior which runs when this state is active.
        /// </summary>
        [SerializeField]
        public Behavior Behavior;

        /// <summary>
        /// Returns if the state can enter.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        /// <returns></returns>
        public bool CanEnter(BehaviorData data, PhysicsData physics)
        {
            foreach (StateTrigger trigger in this.Trigger)
            {
                // One trigger must be non-null and allow enter
                if (trigger != null && trigger.CanEnter(data, physics))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns if the state can exit
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        /// <returns></returns>
        public bool CanExit(BehaviorData data, PhysicsData physics)
        {
            foreach (StateTrigger trigger in this.Trigger)
            {
                // All triggers must either be null or allow exit
                if (trigger != null && !trigger.CanExit(data, physics))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Executed when the state begins execution.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        public void Enter(BehaviorData data, PhysicsData physics)
        {
            this.Behavior.OnStateEnter(data, physics);
        }

        /// <summary>
        /// Executed when the state ends execution.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        public void Exit(BehaviorData data, PhysicsData physics)
        {
            this.Behavior.OnStateExit(data, physics);
        }

    }

}