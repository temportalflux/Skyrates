using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Common.AI
{

    [CreateAssetMenu(menuName = "Data/AI/Target/Waypoints")]
    public class WaypointTargets : BehaviorTimed
    {

        public class PersistentDataWaypoints : PersistentDataTimed
        {

            public int CurrentInitialTarget;

        }
        
        public override object OnEnter(ref BehaviorData data, PhysicsData physics, object pData)
        {
            base.OnEnter(ref data, physics, pData);

            PersistentDataWaypoints perData = (PersistentDataWaypoints) pData;
            perData.CurrentInitialTarget = 0;
            return perData;
        }

        public override object CreatePersistentData()
        {
            PersistentDataTimed timed = (PersistentDataTimed)base.CreatePersistentData();
            return new PersistentDataWaypoints()
            {
                CurrentInitialTarget = 0,
                ExecuteTimeElapsed = timed.ExecuteTimeElapsed
            };
        }

        protected override PersistentDataTimed UpdateTimed(ref BehaviorData behavior, ref PhysicsData physics, float deltaTime, PersistentDataTimed persist)
        {
            PersistentDataWaypoints data = (PersistentDataWaypoints)persist;

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
