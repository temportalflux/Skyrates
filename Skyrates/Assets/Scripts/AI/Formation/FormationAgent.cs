using System.Collections.Generic;
using Skyrates.Entity;
using Skyrates.Physics;
using UnityEngine;

using NearbyTarget = Skyrates.AI.Behavior.DataBehavioral.NearbyTarget;

namespace Skyrates.AI.Formation
{

    [RequireComponent(typeof(EntityAI))]
    public class FormationAgent : MonoBehaviour
    {

        [SerializeField]
        public FormationOwner FormationOwner;

        // TODO: Make editor dropdown which refers to formation owner
        [SerializeField]
        public int FormationSlot;

        private EntityAI _owner;

        public bool HasFormation
        {
            get { return this.FormationOwner != null; }
        }

        void Awake()
        {
            this._owner = this.GetComponent<EntityAI>();
        }

        void OnEnable()
        {
            if (this.FormationOwner != null)
            {
                this.FormationOwner.Subscribe(this);
            }
        }

        void OnDisable()
        {
            if (this.FormationOwner != null)
            {
                this.FormationOwner.Unsubscribe(this);
            }
        }

        public PhysicsData GetTarget()
        {
            return this.FormationOwner != null ? this.FormationOwner.GetTarget(this.FormationSlot) : null;
        }

        public virtual void OnDetect(EntityAI other, float distance)
        {
            // Can be called by the EntityAI owner, or by the FormationOwner
            if (this.FormationOwner != null)
                this.FormationOwner.OnDetect(this, other, distance);
        }

        public List<NearbyTarget> GetNearbyTargets()
        {
            return this.FormationOwner != null ? this.FormationOwner.GetNearbyTargets() : new List<NearbyTarget>();
        }
        
    }

}
