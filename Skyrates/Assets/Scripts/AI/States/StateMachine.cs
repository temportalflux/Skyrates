using System;
using System.Collections.Generic;
using System.Linq;
using Skyrates.AI.Formation;
using Skyrates.Entity;
using Skyrates.Physics;
using UnityEditor;
using UnityEngine;

namespace Skyrates.AI.State
{

    /// <summary>
    /// 
    /// Derived from 
    /// Artifical Intelligence for Games 2nd Edition
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Composite/State/State Machine")]
    public class StateMachine : BehaviorTimed
    {

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class PersistentDataTimedSm : PersistentDataTimed
        {

            /// <summary>
            /// List of persistent data for all state behaviors (idle and otherwise).
            /// </summary>
            public DataPersistent[] PeristentData;

            /// <summary>
            /// The current index of the state being run (-1 if idle).
            /// </summary>
            [SerializeField]
            public int StateIndex;

            /// <summary>
            /// Gets/Sets persistent data from <see cref="PeristentData"/> based on <see cref="StateIndex"/> (where -1 is the first entry for the idle state).
            /// </summary>
            public DataPersistent CurrentPersistent
            {
                get { return PeristentData[StateIndex + 1]; }
                set { PeristentData[StateIndex + 1] = value; }
            }
            
        }

        [SerializeField]
        public State IdleState;
        
        /// <summary>
        /// All of the possible states.
        /// </summary>
        [SerializeField]
        public State[] States;

#if UNITY_EDITOR
        public string[] StateNames;
#endif

        public List<State> AllStates
        {
            get
            {
                List<State> ret = new List<State>();
                ret.Add(this.IdleState);
                ret.AddRange(this.States);
                return ret;
            }
        }

        /// <inheritdoc />
        public override DataPersistent CreatePersistentData()
        {
            PersistentDataTimed dataTimed = (PersistentDataTimed)base.CreatePersistentData();
            
            return new PersistentDataTimedSm()
            {
                ExecuteTimeElapsed = dataTimed.ExecuteTimeElapsed,
                StateIndex = -1,
                PeristentData = this.AllStates.Select(state => state != null ? state.CreatePersistentData() : null).ToArray(),
            };
        }

        /// <summary>
        /// Returns the current state of the state machine, null if state machine is in its idle behavior.
        /// </summary>
        public State GetCurrentState(PersistentDataTimedSm dataTimed)
        {
            return dataTimed.StateIndex < 0 ? this.IdleState : this.GetState(dataTimed.StateIndex);
        }

        /// <summary>
        /// Returns the current running behavior.
        /// </summary>
        public Behavior GetCurrentBehavior(PersistentDataTimedSm dataTimed)
        {
            return this.GetCurrentState(dataTimed).Behavior;
        }

        /// <summary>
        /// Returns the current state at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public State GetState(int index)
        {
            return index < 0 ? this.IdleState : this.States[index];
        }

        /// <inheritdoc />
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent, float deltaTime)
        {
            // Execute checks for changing state via a timed interval interface (calls UpdateTimed)
            persistent = base.GetUpdate(ref physics, ref behavioral, persistent, deltaTime);

            PersistentDataTimedSm customDataTimed = (PersistentDataTimedSm)persistent;

            // Execute update for the current behavior
            customDataTimed.CurrentPersistent = this.GetCurrentState(customDataTimed).GetUpdate(ref physics, ref behavioral, customDataTimed.CurrentPersistent, deltaTime);

            return customDataTimed;
        }

        /// <inheritdoc />
        /// https://gamedev.stackexchange.com/questions/121469/unity3d-smooth-rotation-for-seek-steering-behavior
        protected override PersistentDataTimed UpdateTimed(ref PhysicsData physics, ref DataBehavioral data, PersistentDataTimed persist, float deltaTime)
        {
            PersistentDataTimedSm customDataTimed = (PersistentDataTimedSm) persist;

            State currentState = this.GetCurrentState(customDataTimed);

            StateTransition transition;
            if (currentState.CanEnter(physics, data, customDataTimed.CurrentPersistent, out transition))
            {
                // Can enter the state defined by the transition, so exit the current state
                this.ExitCurrentState(physics, ref data, ref customDataTimed);
                // And enter the destination state set in the transition
                this.EnterState(transition.StateDestination, physics, ref data, ref customDataTimed);
            }

            return customDataTimed;
        }

        /// <summary>
        /// Exits the current state, therefore returning to IDLE.
        /// </summary>
        /// <param name="physics"></param>
        /// <param name="behavioral"></param>
        /// <param name="persistent"></param>
        private void ExitCurrentState(PhysicsData physics, ref DataBehavioral behavioral, ref PersistentDataTimedSm persistent)
        {
            this.GetCurrentState(persistent).Exit(physics, ref behavioral, persistent.CurrentPersistent);
            persistent.StateIndex = -1;
        }

        /// <summary>
        /// Enters a state at the specified index.
        /// </summary>
        /// <param name="iState"></param>
        /// <param name="physics"></param>
        /// <param name="behavioral"></param>
        /// <param name="persistent"></param>
        private void EnterState(int iState, PhysicsData physics, ref DataBehavioral behavioral, ref PersistentDataTimedSm persistent)
        {
            persistent.StateIndex = iState;
            DataPersistent statePersistent = persistent.CurrentPersistent;
            this.GetCurrentState(persistent).Enter(physics, ref behavioral, ref statePersistent);
            persistent.CurrentPersistent = statePersistent;
        }

        public override void OnDetect(EntityAI other, float distance, ref DataPersistent persistent)
        {
            PersistentDataTimedSm smPersistent = (PersistentDataTimedSm)persistent;
            DataPersistent currentPersistent = smPersistent.CurrentPersistent;
            this.GetCurrentState(smPersistent).OnDetect(other, distance, ref currentPersistent);
            smPersistent.CurrentPersistent = currentPersistent;
            persistent = smPersistent;
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        public override void DrawGizmos(PhysicsData physics, DataPersistent persistent)
        {
            PersistentDataTimedSm smPersistent = (PersistentDataTimedSm)persistent;

            Handles.Label(physics.LinearPosition,
                smPersistent != null ? this.GetCurrentState(smPersistent).StateName : "Idle");

            for (int iState = -1; iState < this.States.Length; iState++)
            {
                this.GetState(iState).DrawGizmos(physics,
                    smPersistent != null ? smPersistent.PeristentData[iState + 1] : null);
            }
        }
#endif

    }

}
