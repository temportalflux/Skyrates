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
        public StateTransition[] Transitions;

        public Behavior.DataPersistent CreatePersistentData()
        {
            return new Persistent
            {
                DataBehavior = this.Behavior != null ? this.Behavior.CreatePersistentData() : null,
                DataTransition = this.Transitions.Select(
                    transition => transition != null ? transition.CreatePersistentData() : null
                ).ToArray()
            };
        }

        public bool CanEnter(PhysicsData physics, Behavior.DataBehavioral behavioral, Behavior.DataPersistent persistent, out StateTransition transitionOut)
        {
            transitionOut = null;
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
                transitionOut = transition;
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
        public void Enter(PhysicsData physics, ref Behavior.DataBehavioral behavioral, ref Behavior.DataPersistent persistent)
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
        public void Exit(PhysicsData physics, ref Behavior.DataBehavioral behavioral, Behavior.DataPersistent persistent)
        {
            this.Behavior.OnExit(physics, ref behavioral, ((Persistent)persistent).DataBehavior);
        }

        public void OnDetect(EntityAI other, float distance, ref Behavior.DataPersistent persistent)
        {
            this.Behavior.OnDetect(other, distance, ref ((Persistent)persistent).DataBehavior);
        }

        public void OnDetectEntityNearFormation(FormationAgent source, EntityAI other, float distanceFromSource, ref Behavior.DataPersistent persistent)
        {
            this.Behavior.OnDetectEntityNearFormation(source, other, distanceFromSource, ref ((Persistent)persistent).DataBehavior);
        }

#if UNITY_EDITOR
        public void DrawGizmos(Behavior.DataPersistent persistent)
        {
            Persistent statePersistent = (Persistent)persistent;
            if (this.Behavior != null)
            {
                this.Behavior.DrawGizmos(statePersistent.DataBehavior);
            }
            for (int iTransition = 0; iTransition < this.Transitions.Length; iTransition++)
            {
                if (this.Transitions[iTransition] == null) continue;
                this.Transitions[iTransition].DrawGizmos(statePersistent.DataTransition[iTransition]);
            }
        }
#endif

    }

}