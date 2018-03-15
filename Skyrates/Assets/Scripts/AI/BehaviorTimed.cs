using System;
using Skyrates.AI;
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
    public abstract class BehaviorTimed : Behavior
    {

        public class PersistentDataTimed
        {

            /// <summary>
            /// The amount of time since the states where last checked.
            /// </summary>
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
        
        public override object CreatePersistentData()
        {
            return new PersistentDataTimed()
            {
                ExecuteTimeElapsed = this.ScatterExecute ? Random.Range(0, this.ExecuteFrequency) : 0.0f
            };
        }

        /// <inheritdoc />
        /// https://gamedev.stackexchange.com/questions/121469/unity3d-smooth-rotation-for-seek-steering-behavior
        public override object GetUpdate(ref BehaviorData data, ref PhysicsData physics, float deltaTime, object persist)
        {
            PersistentDataTimed customDataTimed = (PersistentDataTimed)persist;

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
            return this.UpdateTimed(ref data, ref physics, deltaTime, customDataTimed);
        }


        /// <summary>
        /// Checks all triggers if the current state should exit and another state should enter.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        protected abstract PersistentDataTimed UpdateTimed(ref BehaviorData data, ref PhysicsData physics, float deltaTime, PersistentDataTimed persist);

    }

}

