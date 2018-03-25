using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Target
{

    /// <summary>
    /// Selects the next target in a set of waypoints at a timed interval.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Decorator/Waypoints")]
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

            if (behavior.Waypoints == null) return data;
            if (behavior.Waypoints.Targets.Length <= 0) return data;

            Waypoint currentWaypoint = behavior.Waypoints.Targets[data.CurrentInitialTarget];
            // Check distance to current waypoint
            if ((currentWaypoint.transform.position - physics.LinearPosition).sqrMagnitude <
                currentWaypoint.Radius * currentWaypoint.Radius)
            {
                // Can transition to next waypoint
                data.CurrentInitialTarget++;
                data.CurrentInitialTarget %= behavior.Waypoints.Targets.Length;
            }
            
            behavior.Target = PhysicsData.From(behavior.Waypoints.Targets[data.CurrentInitialTarget].transform);

            return data;
        }

        //public override void DrawGizmos(DataPersistent persistent)
        //{
        //    base.DrawGizmos(persistent);
        // TODO: Add lines between waypoints
        // TODO: Add hotkey to enable/disable gameobject
        //    Gizmos.DrawLine();

        //}

    }

}
