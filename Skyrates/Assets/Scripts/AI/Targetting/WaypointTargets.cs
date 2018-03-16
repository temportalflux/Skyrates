using System.Collections;
using System.Collections.Generic;
using Skyrates.AI;
using Skyrates.Common.AI;
using UnityEngine;

namespace Skyrates.AI.Target
{

    /// <summary>
    /// Selects the next target in a set of waypoints at a timed interval.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Target/Waypoints")]
    public class WaypointTargets : BehaviorTimed
    {

        /// <inheritdoc />
        public class Persistent : PersistentDataTimed
        {

            /// <summary>
            /// The current target out of the set of waypoints
            /// </summary>
            public int CurrentInitialTarget;

        }

        /// <inheritdoc />
        public override DataPersistent OnEnter(PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent)
        {
            Persistent perData = (Persistent)persistent;
            perData.CurrentInitialTarget = 0;
            return base.OnEnter(physics, ref behavioral, perData);
        }

        /// <inheritdoc />
        public override DataPersistent CreatePersistentData()
        {
            PersistentDataTimed timed = (PersistentDataTimed)base.CreatePersistentData();
            return new Persistent()
            {
                CurrentInitialTarget = 0,
                ExecuteTimeElapsed = timed.ExecuteTimeElapsed
            };
        }

        /// <inheritdoc />
        protected override PersistentDataTimed UpdateTimed(ref PhysicsData physics, ref DataBehavioral behavior, PersistentDataTimed persist, float deltaTime)
        {
            Persistent data = (Persistent)persist;

            if (behavior.InitialTargets.Length <= 0) return data;

            Waypoint currentWaypoint = behavior.InitialTargets[data.CurrentInitialTarget];
            // Check distance to current waypoint
            if ((currentWaypoint.transform.position - physics.LinearPosition).sqrMagnitude <
                currentWaypoint.Radius * currentWaypoint.Radius)
            {
                // Can transition to next waypoint
                data.CurrentInitialTarget++;
                data.CurrentInitialTarget %= behavior.InitialTargets.Length;
            }

            behavior.Target = new PhysicsData()
            {
                LinearPosition = behavior.InitialTargets[data.CurrentInitialTarget].transform.position
            };

            return data;
        }

    }

}
