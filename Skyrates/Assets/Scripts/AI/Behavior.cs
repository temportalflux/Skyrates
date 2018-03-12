using System;
using System.Collections.Generic;
using Skyrates.Common.AI.Formation;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// Any generic data used by all steering behaviours and that is
    /// specific to each <see cref="EntityDynamic"/> instance.
    /// MUST be network-safe/serializable.
    /// Basically any information that is not algorithm specifc and is pertinent to the owner.
    /// </summary>
    [Serializable]
    public class BehaviorData
    {
        
        public Waypoint[] InitialTargets;

        public FormationOwner FormationOwner;
        public int FormationSlot;

        [HideInInspector]
        public Transform View;

        [HideInInspector]
        public Transform Render;
        
        public bool HasTarget = false;

        /// <summary>
        /// The location/physics information for the target.
        /// </summary>
        [SerializeField]
        public PhysicsData Target;
        
        /// <summary>
        /// A map of data stored by the behaviors in this object
        /// </summary>
        public Dictionary<Guid, object> PersistentData = new Dictionary<Guid, object>();
        
        public object this[Guid guid]
        {
            get { return this.PersistentData.ContainsKey(guid) ? this.PersistentData[guid] : null; }
            set { this.PersistentData[guid] = value; }
        }
        
        public object Remove(Guid guid)
        {
            object data = this[guid];
            this.PersistentData.Remove(guid);
            return data;
        }

    }

    /// <summary>
    /// The base class for all ai behaviors.
    /// </summary>
    public abstract class Behavior : ScriptableObject
    {

        private Guid _persistentDataGuid;

        public virtual void AddPersistentDataTo(ref BehaviorData behavioralData)
        {
            this._persistentDataGuid = Guid.NewGuid();
            behavioralData[this._persistentDataGuid] = this.CreatePersistentData();
        }

        public Guid GetPersistentDataGuid()
        {
            return this._persistentDataGuid;
        }

        public virtual object CreatePersistentData()
        {
            return new object();
        }
        
        /// <summary>
        /// Called to request updated physics. Recommend running on a fixed time step.
        /// </summary>
        /// <param name="data">data specfic to the owner</param>
        /// <param name="physics">the data that is steering the owner</param>
        /// <param name="deltaTime"></param>
        public void GetUpdate(ref BehaviorData data, ref PhysicsData physics, float deltaTime)
        {
            object persistent = data[this.GetPersistentDataGuid()] ?? this.CreatePersistentData();
            data[this.GetPersistentDataGuid()] = this.GetUpdate(ref data, ref physics, deltaTime, persistent);
        }

        public abstract object GetUpdate(ref BehaviorData data, ref PhysicsData physics, float deltaTime, object persistent);

        public void OnEnter(ref BehaviorData data, PhysicsData physics)
        {
            object persistent = data[this.GetPersistentDataGuid()];
            data[this.GetPersistentDataGuid()] = this.OnEnter(ref data, physics, persistent);
        }

        /// <summary>
        /// Executed when a state begins execution of this behavior.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        public virtual object OnEnter(ref BehaviorData data, PhysicsData physics, object persistentData)
        {
            return persistentData;
        }

        public void OnExit(ref BehaviorData data, PhysicsData physics)
        {
            object persistent = data[this.GetPersistentDataGuid()];
            this.OnExit(ref data, physics, persistent);
        }

        /// <summary>
        /// Executed when a state ends execution of this behavior.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="physics"></param>
        public virtual void OnExit(ref BehaviorData data, PhysicsData physics, object persistentData)
        {
        }

    }

}
