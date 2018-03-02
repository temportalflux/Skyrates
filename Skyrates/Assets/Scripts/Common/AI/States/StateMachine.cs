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
    public class StateMachine : Behavior
    {
        
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

        /// <summary>
        /// How many seconds between state checks.
        /// </summary>
        public float StateCheckFrequency = 1.0f;

        /// <summary>
        /// Scatters the state checks by offseting the start time by a
        /// random amount of seconds between 0 and <see cref="StateCheckFrequency"/>.
        /// </summary>
        public bool ScatterStateChecks = true;
        
        /// <summary>
        /// The current index of the state being run (-1 if idle)
        /// </summary>
        private int _stateIndex;

        /// <summary>
        /// The amount of time since the states where last checked.
        /// </summary>
        private float _stateCheckTimeElapsed;

        /// <summary>
        /// Returns the current state of the state machine, null if state machine is in its idle behavior.
        /// </summary>
        private State CurrentState
        {
            get { return this._stateIndex < 0 ? null : this.States[this._stateIndex]; }
        }

        /// <summary>
        /// Returns the current running behavior.
        /// </summary>
        private Behavior CurrentBehavior
        {
            get { return this._stateIndex < 0 ? this.IdleBehavior : this.CurrentState.Behavior; }
        }

        private void OnEnable()
        {
            // Set state to default value
            this._stateIndex = -1;
            this._stateCheckTimeElapsed = this.ScatterStateChecks ? Random.Range(0, this.StateCheckFrequency) : 0.0f;
        }

        private void OnDisable()
        {
            // Set state to default value
            this._stateIndex = -1;
            this._stateCheckTimeElapsed = 0.0f;
        }

        /// <inheritdoc />
        /// https://gamedev.stackexchange.com/questions/121469/unity3d-smooth-rotation-for-seek-steering-behavior
        public override PhysicsData GetUpdate(BehaviorData data, PhysicsData physics, float deltaTime)
        {
            this.TryUpdateState(data, physics, deltaTime);
            return this.CurrentBehavior != null
                ? this.CurrentBehavior.GetUpdate(data, physics, deltaTime)
                : physics;
        }

        /// <summary>
        /// If necessary, checks time elapsed since last update, and checks state triggers as necessary.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        /// <param name="deltaTime"></param>
        private void TryUpdateState(BehaviorData data, PhysicsData physics, float deltaTime)
        {
            // Check to see if the timer is used
            if (this.StateCheckFrequency > 0.0f)
            {
                // Add time
                this._stateCheckTimeElapsed += deltaTime;

                // If not enough time has elapsed, return
                if (this._stateCheckTimeElapsed < this.StateCheckFrequency)
                    return;

                // Otherwise, decrement the time elapsed
                this._stateCheckTimeElapsed -= this.StateCheckFrequency;
            }

            // And check for updates
            this.UpdateState(data, physics);
        }

        /// <summary>
        /// Checks all triggers if the current state should exit and another state should enter.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        private void UpdateState(BehaviorData data, PhysicsData physics)
        {
            // Try to exit the state
            if (this.CurrentState != null && this.CurrentState.CanExit(data, physics))
            {
                this.ExitCurrentState(data, physics);
            }

            if (this.CurrentState == null)
            {
                // Try to find a new state
                for (int iState = 0; iState < this.States.Length; iState++)
                {
                    // Check if the state can be entered
                    if (!this.States[iState].CanEnter(data, physics)) continue;
                    
                    // Enter the state
                    this.EnterState(iState, data, physics);
                    return;
                }
            }

        }

        /// <summary>
        /// Exits the current state, therefore returning to IDLE.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        private void ExitCurrentState(BehaviorData data, PhysicsData physics)
        {
            this.CurrentState.Exit(data, physics);
            this._stateIndex = -1;
        }

        /// <summary>
        /// Enters a state at the specified index.
        /// </summary>
        /// <param name="iState"></param>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        private void EnterState(int iState, BehaviorData data, PhysicsData physics)
        {
            this._stateIndex = iState;
            this.CurrentState.Enter(data, physics);
        }

    }

}
