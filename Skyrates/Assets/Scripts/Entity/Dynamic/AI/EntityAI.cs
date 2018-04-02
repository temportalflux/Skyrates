using System;
using System.Collections.Generic;
using Skyrates.AI;
using Skyrates.AI.Composite;
using Skyrates.AI.Formation;
using Skyrates.AI.Target;
using Skyrates.Mono;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.Entity
{

    /// <summary>
    /// An entity class which is steered/controled by some for of behavioral AI.
    /// </summary>
    public class EntityAI : EntityDynamic, IDistanceCollidable
    {

        /// <summary>
        /// The actual steering object - set via editor.
        /// </summary>
        [Header("AI: Settings")]
        [SerializeField]
        public BehaviorPipeline Behavior;

        /// <summary>
        /// The steering data used - info which is specific to this
        /// entity and likely used by multiple steering algorithms.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        public /*readonly*/ Behavior.DataBehavioral DataBehavior = new Behavior.DataBehavioral();

        [HideInInspector]
        [SerializeField]
        public Behavior.DataPersistent DataPersistent;

        protected FormationOwner FormationOwner;
        private FormationAgent _formationAgent;

        private WaypointAgent _waypointAgent;

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();

            this.FormationOwner = this.GetComponent<FormationOwner>();
            this._formationAgent = this.GetComponent<FormationAgent>();
            this._waypointAgent = this.GetComponent<WaypointAgent>();

            this.DataBehavior.Target = new PhysicsData();
            this.DataBehavior.NearbyTargets = new List<Behavior.DataBehavioral.NearbyTarget>();

            this.UpdateBehaviorData();

            if (this.Behavior != null)
            {
                this.DataPersistent = this.Behavior.CreatePersistentData();
                this.DataPersistent = this.Behavior.OnEnter(this.PhysicsData, ref this.DataBehavior, this.DataPersistent);
            }

        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Exit the state
            if (this.Behavior != null)
            {
                this.Behavior.OnExit(this.PhysicsData, ref this.DataBehavior, this.DataPersistent);
            }

        }

        /// <summary>
        /// Updates any pertinent data from the entity to its behavioral data
        /// </summary>
        protected virtual void UpdateBehaviorData()
        {
            this.PhysicsData.UpdateDirections(this.transform);

            this.DataBehavior.View = this.GetView();
            this.DataBehavior.Render = this.GetRender().transform;

            this.DataBehavior.Target.LinearPosition = this.PhysicsData.LinearPosition;
            this.DataBehavior.Target.RotationPosition = this.PhysicsData.RotationPosition;

            this.DataBehavior.Formation = this._formationAgent;
            this.DataBehavior.Waypoints = this._waypointAgent;

            this.DataBehavior.ThrustMultiplier = 1.0f;
            this.DataBehavior.TurnSpeedMultiplier = 1.0f;

            this.DataBehavior.CleanNearby(this.PhysicsData);

        }
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            this.UpdateBehaviorData();

            // Update steering on a fixed timestep
            if (this.Behavior != null)
            {
                // Cache the old data, for memory checks later
                PhysicsData oldPhysics = this.PhysicsData;
                Behavior.DataBehavioral oldData = this.DataBehavior;
                // Tell the behavior to update AI stuff (where to move, what target, etc)
                this.DataPersistent = this.Behavior.GetUpdate(ref this.PhysicsData, ref this.DataBehavior, this.DataPersistent, Time.fixedDeltaTime);
                // Double check to make sure AI didn't overwrite the actual reference of physics and behavioral data
                // these are expected not to change after start, but cant be readonly
                Debug.Assert(ReferenceEquals(oldPhysics, this.PhysicsData), "Physics data memory has been overwritten");
                Debug.Assert(ReferenceEquals(oldData, this.DataBehavior), "Behavioral data memory has been overwritten");
            }

            // Integrate physics from steering and any network updates
            this.IntegratePhysics(Time.fixedDeltaTime);

        }

        /// <summary>
        /// Integrates the current set of physics with the <see cref="Transform"/>.
        /// </summary>
        /// <param name="deltaTime"></param>
        protected virtual void IntegratePhysics(float deltaTime)
        {
            if (this.Physics == null)
                return;

            // Integrate the velocities and accellerations
            this.PhysicsData.Integrate(deltaTime);

            // Position
            {
                // Use rigidbody to apply velocity
                // https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
                //this._characterController.Move(this.Physics.LinearVelocity * deltaTime);
                this.Physics.velocity = this.PhysicsData.LinearVelocity;

                // Retroactively update position due to rigidbody
                this.PhysicsData.LinearPosition = this.transform.position;
            }

            this.ApplyRotations(this.PhysicsData, deltaTime);
            this.UpdateAnimations(this.PhysicsData);

        }

        /// <summary>
        /// Applies rotation to the entity
        /// </summary>
        /// <param name="physics"></param>
        /// <param name="deltaTime"></param>
        protected virtual void ApplyRotations(PhysicsData physics, float deltaTime)
        {
            this.Physics.MoveRotation(physics.HasAesteticRotation ?
                physics.RotationPositionComposite : physics.RotationPosition);
        }

        /// <summary>
        /// Update any animations based on movement
        /// </summary>
        /// <param name="physics"></param>
        protected virtual void UpdateAnimations(PhysicsData physics)
        {
        }

        /// <inheritdoc />
        public virtual void OnEnterEntityRadius(EntityAI other, float maxDistance)
        {
            this.OnDetect(other, maxDistance);
        }

        /// <inheritdoc />
        public virtual void OnOverlapWith(GameObject other, float maxDistance)
        {
            EntityAI otherAi = other.GetComponent<EntityAI>();
            if (otherAi == null) return;
            this.OnDetect(otherAi, maxDistance);
        }

        public virtual void OnDetect(EntityAI other, float maxDistance)
        {

            if (!this.DataBehavior.NearbyTargets.Exists(target => ReferenceEquals(target.Target, other.PhysicsData)))
            {
                Behavior.DataBehavioral.NearbyTarget target = new Behavior.DataBehavioral.NearbyTarget();
                target.Target = other.PhysicsData;
                target.MaxDistanceSq = maxDistance * maxDistance;
                this.DataBehavior.NearbyTargets.Add(target);
            }

            if (this.Behavior != null)
                this.Behavior.OnDetect(other, maxDistance, ref this.DataPersistent);
            if (this._formationAgent != null)
                this._formationAgent.OnDetect(other, maxDistance);

        }

#if UNITY_EDITOR
        public virtual void OnDrawGizmos()
        {
            if (this.Behavior != null)
            {
                PhysicsData data = this.PhysicsData.Copy();
                data.UpdatePositions(this.transform);
                data.UpdateDirections(this.transform);
                try
                {
                    this.Behavior.DrawGizmos(data, Application.isPlaying ? this.DataPersistent : null);
                }
                catch (Exception e)
                {
                    // ignored
                    Debug.LogException(e);
                }
            }

            if (this.DataBehavior.NearbyTargets != null)
            {
                foreach (Behavior.DataBehavioral.NearbyTarget target in this.DataBehavior.NearbyTargets)
                {
                    target.Target.DrawGizmos(1.0f, 0.5f, Color.gray);
                }
            }
        }

        public virtual void OnDrawGizmosSelected()
        {
            if (this.Behavior != null)
            {
                PhysicsData data = this.PhysicsData.Copy();
                data.UpdatePositions(this.transform);
                data.UpdateDirections(this.transform);
                try
                {
                    this.Behavior.DrawGizmosSelected(data, Application.isPlaying ? this.DataPersistent : null);
                }
                catch (Exception e)
                {
                    // ignored
                    Debug.LogException(e);
                }
            }

            if (this.DataBehavior.NearbyTargets != null)
            {
                foreach (Behavior.DataBehavioral.NearbyTarget target in this.DataBehavior.NearbyTargets)
                {
                    target.Target.DrawGizmosSelected(1.0f, 0.5f, Color.gray);
                }
            }
        }
#endif

    }

}
