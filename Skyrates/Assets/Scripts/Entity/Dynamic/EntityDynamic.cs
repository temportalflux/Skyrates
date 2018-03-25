using Skyrates.AI.Steering;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.Entity
{

    /// <summary>
    /// Any entity in the world which moves around.
    /// By default, this object uses <see cref="Steering"/>, and it is
    /// the assumption that objects which move will use some form of steering.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EntityDynamic : Entity
    {

        /// <summary>
        /// The current physics data of this object.
        /// Updated via <see cref="Steering"/>.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        public /*readonly*/ PhysicsData PhysicsData = new PhysicsData();

        /// <summary>
        /// The component which controls the physics of the entity.
        /// </summary>
        protected Rigidbody Physics;

        protected virtual void Awake()
        {
            this.Physics = this.GetComponent<Rigidbody>();
            Debug.Assert(this.Physics != null, string.Format("{0} has null rigidbody - this is required to move with collisions.", this.name));
            this.PhysicsData.LinearPosition = this.transform.position;
            this.PhysicsData.RotationPosition = this.transform.rotation;
        }

        protected virtual void FixedUpdate()
        {
        }

        /// <summary>
        /// Returns the direction/rotation that the entity is viewing at (where it is looking from/to).
        /// </summary>
        /// <returns></returns>
        protected virtual Transform GetView()
        {
            return this.transform;
        }

        /// <summary>
        /// Returns the direction/rotation that the entity is facing.
        /// </summary>
        /// <returns></returns>
        public virtual Transform GetRender()
        {
            return this.GetView();
        }

    }

}
