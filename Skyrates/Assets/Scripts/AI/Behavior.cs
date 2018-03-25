using System;
using System.Collections.Generic;
using Skyrates.AI.Formation;
using Skyrates.AI.Target;
using Skyrates.Entity;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI
{

    /// <summary>
    /// The base class for all ai behaviors.
    /// </summary>
    public abstract class Behavior : ScriptableObject
    {

        /// <summary>
        /// Any generic data used by all steering behaviours and that is
        /// specific to each <see cref="EntityDynamic"/> instance.
        /// Basically any information that is not algorithm specifc and is pertinent to the owner.
        /// </summary>
        [Serializable]
        public class DataBehavioral
        {

            public class NearbyTarget
            {
                public PhysicsData Target;
                public float MaxDistanceSq;
            }

            [HideInInspector]
            [NonSerialized]
            public Transform View;

            [HideInInspector]
            [NonSerialized]
            public Transform Render;

            [HideInInspector]
            [NonSerialized]
            public bool HasTarget = false;

            /// <summary>
            /// The location/physics information for the target.
            /// </summary>
            [HideInInspector]
            [SerializeField]
            public PhysicsData Target;

            [NonSerialized]
            public List<NearbyTarget> NearbyTargets;
            [NonSerialized]
            public List<NearbyTarget> NearbyFormationTargets;

            [HideInInspector]
            [NonSerialized]
            public FormationAgent Formation;

            [HideInInspector]
            [NonSerialized]
            public WaypointAgent Waypoints;
            
            public float TurnSpeedMultiplier;
            
            public float ThrustMultiplier;

            public void CleanNearby(PhysicsData physics)
            {
                this.CleanNearby(physics, ref this.NearbyTargets);
                this.CleanNearby(physics, ref this.NearbyFormationTargets);
            }

            private void CleanNearby(PhysicsData physics, ref List<NearbyTarget> nearbyTargets)
            {
                nearbyTargets.RemoveAll(target => (physics.LinearPosition - target.Target.LinearPosition).sqrMagnitude > target.MaxDistanceSq);
            }

        }

        /// <summary>
        /// An object to be held by the agent which has custom data which cannot go in an asset.
        /// i.e. the index of a target in a list.
        /// </summary>
        [Serializable]
        public class DataPersistent
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }
        
        /// <summary>
        /// Returns an object to be held by the agent which has custom data which cannot go in an asset.
        /// i.e. the index of a target in a list.
        /// </summary>
        /// <returns></returns>
        public virtual DataPersistent CreatePersistentData()
        {
            return null;
        }

        /// <summary>
        /// Called to request updated physics. Recommend running on a fixed time step.
        /// </summary>
        /// <param name="behavioral">data specfic to the owner</param>
        /// <param name="physics">the data that is steering the owner</param>
        /// <param name="persistent"></param>
        /// <param name="deltaTime"></param>
        public abstract DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent, float deltaTime);
        
        /// <summary>
        /// Executed when a state begins execution of this behavior.
        /// </summary>
        /// <param name="behavioral"></param>
        /// <param name="persistent"></param>
        /// <param name="physics"></param>
        public virtual DataPersistent OnEnter(PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent)
        {
            return persistent;
        }
        
        /// <summary>
        /// Executed when a state ends execution of this behavior.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        /// <param name="persistent"></param>
        public virtual void OnExit(PhysicsData physics, ref DataBehavioral data, DataPersistent persistent)
        {
        }

        public virtual void OnDetect(EntityAI other, float distance, ref DataPersistent persistent)
        {
        }

        public virtual void OnDetectEntityNearFormation(FormationAgent source, EntityAI other, float distanceFromSource, ref DataPersistent persistent)
        {
        }

        public virtual void DrawGizmos(PhysicsData physics, DataPersistent persistent)
        {
        }

    }
}