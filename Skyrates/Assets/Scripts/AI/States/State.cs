using System;
using System.Linq;
using Skyrates.AI.Formation;
using Skyrates.Entity;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.State
{

    /// <summary>
    /// A state in the overarching machine.
    /// </summary>
    [Serializable]
    public class State
    {

        [Serializable]
        public class Persistent : Behavior.DataPersistent
        {
            public Behavior.DataPersistent DataBehavior;
            public Behavior.DataPersistent[] DataTransition;
        }

        [SerializeField]
        public string StateName;

        /// <summary>
        /// The behavior which runs when this state is active.
        /// </summary>
        [SerializeField]
        public Behavior Behavior;

        /// <summary>
        /// A list of possible transitions.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        public StateTransition[] Transitions = new StateTransition[0];

        [SerializeField]
        public int[] TransitionDestinations = new int[0];

        public Behavior.DataPersistent CreatePersistentData()
        {
            Behavior.DataPersistent[] DataTransitions = new Behavior.DataPersistent[this.Transitions.Length];
            for (int i = 0; i < this.Transitions.Length; i++)
            {
                DataTransitions[i] = this.Transitions[i] != null ? this.Transitions[i].CreatePersistentData() : null;
            }
            return new Persistent
            {
                DataBehavior = this.Behavior != null ? this.Behavior.CreatePersistentData() : null,
                DataTransition = DataTransitions,//this.Transitions.Select(
                //    transition => transition != null ? transition.CreatePersistentData() : null
                //).ToArray()
            };
        }

        public bool CanExit(PhysicsData physics, Behavior.DataBehavioral behavioral, Behavior.DataPersistent persistent, out int destinationIndex)
        {
            destinationIndex = -1;
            Persistent statePersistent = (Persistent) persistent;

            // Try to transition out of the current state (idle or otherwise)
            if (this.Transitions == null) return false;

            for (int iTransition = 0; iTransition < this.Transitions.Length; iTransition++)
            {
                StateTransition transition = this.Transitions[iTransition];
                // Check if we can enter some state via the transition, if not, continue the loop
                if (transition == null || !transition.CanEnter(
                    behavioral, physics, ref statePersistent.DataTransition[iTransition]))
                    continue;
                destinationIndex = this.TransitionDestinations[iTransition];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Executed when the state begins execution.
        /// </summary>
        /// <param name="physics"></param>
        /// <param name="behavioral"></param>
        /// <param name="persistent"></param>
        public void OnEnter(PhysicsData physics, ref Behavior.DataBehavioral behavioral, ref Behavior.DataPersistent persistent)
        {
            Persistent statePersistent = ((Persistent) persistent);
            statePersistent.DataBehavior = this.Behavior.OnEnter(physics, ref behavioral, statePersistent.DataBehavior);
            persistent = statePersistent;
        }

        public Behavior.DataPersistent GetUpdate(ref PhysicsData physics, ref Behavior.DataBehavioral behavioral,
            Behavior.DataPersistent persistent, float deltaTime)
        {
            Persistent statePersistent = (Persistent) persistent;
            if (this.Behavior != null)
            {
                statePersistent.DataBehavior = this.Behavior.GetUpdate(ref physics, ref behavioral, statePersistent.DataBehavior, deltaTime);
            }
            return statePersistent;
        }

        /// <summary>
        /// Executed when the state ends execution.
        /// </summary>
        /// <param name="physics"></param>
        /// <param name="behavioral"></param>
        /// <param name="persistent"></param>
        public void OnExit(PhysicsData physics, ref Behavior.DataBehavioral behavioral, Behavior.DataPersistent persistent)
        {
            this.Behavior.OnExit(physics, ref behavioral, ((Persistent)persistent).DataBehavior);
        }

        public void OnDetect(EntityAI other, float distance, ref Behavior.DataPersistent persistent)
        {
            this.Behavior.OnDetect(other, distance, ref ((Persistent)persistent).DataBehavior);
        }

#if UNITY_EDITOR
        public void DrawGizmos(PhysicsData physics, Behavior.DataPersistent persistent)
        {
            Persistent statePersistent = (Persistent)persistent;
            if (this.Behavior != null)
            {
                this.Behavior.DrawGizmos(physics,
                    statePersistent != null ? statePersistent.DataBehavior : null);
            }
            for (int iTransition = 0; iTransition < this.Transitions.Length; iTransition++)
            {
                if (this.Transitions[iTransition] == null) continue;
                this.Transitions[iTransition].DrawGizmos(physics,
                    statePersistent != null ? statePersistent.DataTransition[iTransition] : null);
            }
        }
        public void DrawGizmosSelected(PhysicsData physics, Behavior.DataPersistent persistent)
        {
            Persistent statePersistent = (Persistent)persistent;
            if (this.Behavior != null)
            {
                this.Behavior.DrawGizmosSelected(physics,
                    statePersistent != null ? statePersistent.DataBehavior : null);
            }
            for (int iTransition = 0; iTransition < this.Transitions.Length; iTransition++)
            {
                if (this.Transitions[iTransition] == null) continue;
                this.Transitions[iTransition].DrawGizmosSelected(physics,
                    statePersistent != null ? statePersistent.DataTransition[iTransition] : null);
            }
        }
#endif

    }

}