using System;
using Skyrates.Physics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Skyrates.AI
{
    
    /// <summary>
    /// A base class for behaviors which should only be executed at a time interval, instead of every tick.
    /// </summary>
    public abstract class BehaviorTimed : Behavior
    {

        /// <inheritdoc />
        [Serializable]
        public class PersistentDataTimed : DataPersistent
        {

            /// <summary>
            /// The amount of time since the states where last checked.
            /// </summary>
            [SerializeField]
            public float ExecuteTimeElapsed;

        }

        /// <summary>
        /// How many seconds between state checks.
        /// </summary>
        public float ExecuteFrequency = 1.0f;

        /// <summary>
        /// Scatters the state checks by offseting the start time by a
        /// random amount of seconds between 0 and <see cref="ExecuteFrequency"/>.
        /// </summary>
        public bool ScatterExecute = true;

        /// <inheritdoc />
        public override DataPersistent CreatePersistentData()
        {
            return new PersistentDataTimed()
            {
                ExecuteTimeElapsed = this.ScatterExecute ? Random.Range(0, this.ExecuteFrequency) : 0.0f
            };
        }

        /// <inheritdoc />
        /// https://gamedev.stackexchange.com/questions/121469/unity3d-smooth-rotation-for-seek-steering-behavior
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent, float deltaTime)
        {
            PersistentDataTimed customDataTimed = (PersistentDataTimed)persistent;

            // Check to see if the timer is used
            if (this.ExecuteFrequency > 0.0f)
            {
                // Add time
                customDataTimed.ExecuteTimeElapsed += deltaTime;

                // If not enough time has elapsed, return
                if (customDataTimed.ExecuteTimeElapsed < this.ExecuteFrequency)
                    return customDataTimed;

                // Otherwise, decrement the time elapsed
                customDataTimed.ExecuteTimeElapsed -= this.ExecuteFrequency;
            }

            // And check for updates
            return this.UpdateTimed(ref physics, ref behavioral, customDataTimed, deltaTime);
        }


        /// <summary>
        /// Checks all triggers if the current state should exit and another state should enter.
        /// </summary>
        /// <param name="physics"></param>
        /// <param name="behavioral"></param>
        /// <param name="persistent"></param>
        /// <param name="deltaTime"></param>
        protected abstract PersistentDataTimed UpdateTimed(ref PhysicsData physics, ref DataBehavioral behavioral, PersistentDataTimed persistent, float deltaTime);

    }

}

