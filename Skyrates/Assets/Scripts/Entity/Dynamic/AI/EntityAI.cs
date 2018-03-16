using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.AI;
using Skyrates.AI.Composite;
using Skyrates.Client.Entity;
using Skyrates.Common.AI;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    /// <summary>
    /// An entity class which is steered/controled by some for of behavioral AI.
    /// </summary>
    public class EntityAI : EntityDynamic, IDistanceCollidable
    {

        /// <summary>
        /// The actual steering object - set via editor.
        /// </summary>
        [Header("AI Settings")]
        [SerializeField]
        public BehaviorPipeline Behavior;

        /// <summary>
        /// The steering data used - info which is specific to this
        /// entity and likely used by multiple steering algorithms.
        /// </summary>
        [SerializeField]
        public Behavior.DataBehavioral DataBehavior;

        [HideInInspector]
        [SerializeField]
        public Behavior.DataPersistent DataPersistent;

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            this.DataBehavior.Target = new PhysicsData();

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
            this.DataBehavior.View = this.GetView();
            this.DataBehavior.Render = this.GetRender().transform;
        }
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            this.UpdateBehaviorData();

            // Update steering on a fixed timestep
            if (this.Behavior != null)
            {
                this.DataPersistent = this.Behavior.GetUpdate(ref this.PhysicsData, ref this.DataBehavior, this.DataPersistent, Time.fixedDeltaTime);
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

        /// <inheritdoc />
        public virtual void OnEnterEntityRadius(EntityAI other, float maxDistance)
        {
            if (this.Behavior == null) return;
            this.Behavior.OnDetect(other, maxDistance);
        }

        /// <inheritdoc />
        public virtual void OnOverlapWith(GameObject other, float radius)
        {
            if (this.Behavior == null) return;
            EntityAI otherAi = other.GetComponent<EntityAI>();
            if (otherAi == null) return;
            this.Behavior.OnDetect(otherAi, radius);
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (this.Behavior == null) return;
            if (this.DataPersistent == null) return;
            try
            {
                this.Behavior.DrawGizmos(this.DataPersistent);
            }
            catch (Exception)
            {
                // ignored
            }
        }
#endif

    }

}
