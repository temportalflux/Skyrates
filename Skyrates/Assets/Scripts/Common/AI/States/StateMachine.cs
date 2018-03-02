using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// 
    /// Derived from 
    /// Artifical Intelligence for Games 2nd Edition
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Control/State Machine")]
    public class StateMachine : BehaviorTimed
    {

        public class PersistentDataTimedSm : PersistentDataTimed
        {

            /// <summary>
            /// The current index of the state being run (-1 if idle)
            /// </summary>
            public int StateIndex;

        }

        /// <summary>
        /// The default behavior when no state is available.
        /// </summary>
        [SerializeField]
        public Behavior IdleBehavior;
        
        /// <summary>
        /// All of the possible states.
        /// </summary>
        [SerializeField]
        public State[] States;
        
        public override void AddPersistentDataTo(ref BehaviorData behavioralData)
        {
            base.AddPersistentDataTo(ref behavioralData);
            if (this.IdleBehavior != null) this.IdleBehavior.AddPersistentDataTo(ref behavioralData);
            foreach (State state in this.States)
            {
                if (state.Behavior != null) state.Behavior.AddPersistentDataTo(ref behavioralData);
            }
        }

        /// <summary>
        /// Returns the current state of the state machine, null if state machine is in its idle behavior.
        /// </summary>
        public State GetCurrentState(PersistentDataTimedSm dataTimed)
        {
            return dataTimed.StateIndex < 0 ? null : this.States[dataTimed.StateIndex];
        }

        /// <summary>
        /// Returns the current running behavior.
        /// </summary>
        public Behavior GetCurrentBehavior(PersistentDataTimedSm dataTimed)
        {
            return dataTimed.StateIndex < 0 ? this.IdleBehavior : this.GetCurrentState(dataTimed).Behavior;
        }

        public override object CreatePersistentData()
        {
            PersistentDataTimed dataTimed = (PersistentDataTimed)base.CreatePersistentData();
            return new PersistentDataTimedSm()
            {
                StateIndex = -1,
                ExecuteTimeElapsed = dataTimed.ExecuteTimeElapsed
            };
        }

        /// <inheritdoc />
        /// https://gamedev.stackexchange.com/questions/121469/unity3d-smooth-rotation-for-seek-steering-behavior
        protected override PersistentDataTimed UpdateTimed(ref BehaviorData data, ref PhysicsData physics, float deltaTime, PersistentDataTimed persist)
        {
            PersistentDataTimedSm customDataTimed = (PersistentDataTimedSm) persist;

            // Try to exit the state
            State currentState = this.GetCurrentState(customDataTimed);
            if (currentState != null && currentState.CanExit(data, physics))
            {
                this.ExitCurrentState(ref data, ref customDataTimed, physics);
            }

            if (this.GetCurrentState(customDataTimed) == null)
            {
                // Try to find a new state
                for (int iState = 0; iState < this.States.Length; iState++)
                {
                    // Check if the state can be entered
                    if (!this.States[iState].CanEnter(data, physics)) continue;

                    // Enter the state
                    this.EnterState(iState, ref data, ref customDataTimed, physics);
                    return customDataTimed;
                }
            }

            return customDataTimed;
        }

        /// <summary>
        /// Exits the current state, therefore returning to IDLE.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        private void ExitCurrentState(ref BehaviorData data, ref PersistentDataTimedSm persist, PhysicsData physics)
        {
            this.GetCurrentState(persist).Exit(ref data, physics);
            persist.StateIndex = -1;
        }

        /// <summary>
        /// Enters a state at the specified index.
        /// </summary>
        /// <param name="iState"></param>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        private void EnterState(int iState, ref BehaviorData data, ref PersistentDataTimedSm persist, PhysicsData physics)
        {
            persist.StateIndex = iState;
            this.GetCurrentState(persist).Enter(ref data, physics);
        }
        
    }

}
