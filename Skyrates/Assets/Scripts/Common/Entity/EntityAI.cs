using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using Skyrates.Client.Entity;
using Skyrates.Common.AI;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    public class EntityAI : EntityDynamic, DistanceCollidable
    {

        /// <summary>
        /// The steering data used - info which is specific to this
        /// entity and likely used by multiple steering algorithms.
        /// </summary>
        public BehaviorData BehaviorData;

        /// <summary>
        /// The actual steering object - set via editor.
        /// </summary>
        [SerializeField]
        public Behavior[] Steering;


        protected override void Start()
        {
            base.Start();
            this.BehaviorData.Target = new PhysicsData();

            this.UpdateBehaviorData();

            foreach (Behavior behavior in this.Steering)
            {
                if (behavior == null) continue;

                // Init the object and its persistent data
                behavior.AddPersistentDataTo(ref this.BehaviorData);

                // Enter the state
                behavior.OnEnter(ref this.BehaviorData, this.Physics);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (Behavior behavior in this.Steering)
            {
                if (behavior == null) continue;

                // Exit the state
                behavior.OnExit(ref this.BehaviorData, this.Physics);

                // Remove any persistent data
                this.BehaviorData.Remove(behavior.GetPersistentDataGuid());
            }
        }

        protected virtual void UpdateBehaviorData()
        {
            this.BehaviorData.View = this.GetView();
            this.BehaviorData.Render = this.GetRender().transform;
        }
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            this.UpdateBehaviorData();

            // Update steering on a fixed timestep
            foreach (Behavior behavior in this.Steering)
            {
                if (behavior != null)
                {
                    behavior.GetUpdate(ref this.BehaviorData, ref this.Physics, Time.fixedDeltaTime);
                }
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
            if (this._physics == null)
                return;

            // Integrate the velocities and accellerations
            this.Physics.Integrate(deltaTime);

            // Position
            {
                // Use rigidbody to apply velocity
                // https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
                //this._characterController.Move(this.Physics.LinearVelocity * deltaTime);
                this._physics.velocity = this.Physics.LinearVelocity;

                // Retroactively update position due to rigidbody
                this.Physics.LinearPosition = this.transform.position;
            }

            // Rotation
            this._physics.MoveRotation(this.Physics.RotationPosition);

            // Rotation Aestetic
            // TODO: Delegate to subclass maybe?
            if (this.Physics.HasAesteticRotation)
            {
                this.GetRender().transform.localRotation = this.Physics.RotationAesteticPosition;
            }

        }

        /// <summary>
        /// Called when some other entity detects that this entity is now less
        /// than maxDistance away from them.
        /// <see cref="DistanceCollider"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="maxDistance"></param>
        public virtual void OnEnterEntityRadius(EntityAI other, float maxDistance)
        {
            
        }

        public virtual void OnOverlapWith(GameObject other, float radius)
        {
            
        }

    }

}
