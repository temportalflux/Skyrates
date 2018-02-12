using Skyrates.Common.AI;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    /// <summary>
    /// Any entity in the world which moves around.
    /// By default, this object uses <see cref="Skyrates.Common.AI.Steering"/>, and it is
    /// the assumption that objects which move will use some form of steering.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EntityDynamic : Entity
    {

        /// <summary>
        /// The current physics data of this object.
        /// Updated via <see cref="Steering"/>.
        /// </summary>
        [BitSerialize(4)]
        public PhysicsData Physics = new PhysicsData();

        /// <summary>
        /// The steering data used - info which is specific to this
        /// entity and likely used by multiple steering algorithms.
        /// </summary>
        // TODO: Player AI doesn't use a target...?
        //[BitSerialize(3)]
        [HideInInspector]
        public SteeringData SteeringData = new SteeringData();

        /// <summary>
        /// The actual steering object - set via editor.
        /// </summary>
        public Steering Steering;

        private Rigidbody _rigidbody;

        protected override void Start()
        {
            base.Start();
            this._rigidbody = this.GetComponent<Rigidbody>();
            Debug.Assert(this._rigidbody != null, string.Format("{0} has null rigidbody - this is required to move with collisions.", this.name));
        }

        protected virtual void FixedUpdate()
        {

            this.SteeringData.View = this.GetView();
            this.SteeringData.Render = this.GetRender();

            // Update steering on a fixed timestep
            if (this.Steering != null)
            {
                this.Steering.GetSteering(this.SteeringData, ref this.Physics);
            }

            // Integrate physics from steering and any network updates
            this.IntegratePhysics(Time.fixedDeltaTime);

        }

        protected virtual Transform GetView()
        {
            return this.transform;
        }

        protected virtual Transform GetRender()
        {
            return this.GetView();
        }

        /// <summary>
        /// Integrates the current set of physics with the <see cref="Transform"/>.
        /// </summary>
        /// <param name="deltaTime"></param>
        private void IntegratePhysics(float deltaTime)
        {

            // Update velocity
            this.Integrate(ref this.Physics.LinearVelocity, this.Physics.LinearAccelleration, deltaTime);
            
            // Update position
            // https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
            //this._characterController.Move(this.Physics.LinearVelocity * deltaTime);
            this._rigidbody.velocity = this.Physics.LinearVelocity;

            // Update physics position
            this.Physics.LinearPosition = this.transform.position;

            // Update rotational velocity
            this.Integrate(ref this.Physics.RotationVelocity, this.Physics.RotationAccelleration, deltaTime);

            // Update rotation
            this.GetRender().Rotate(this.Physics.RotationVelocity.eulerAngles, Space.World);

            // Set rotation
            this.Physics.RotationPosition = this.GetRender().rotation;

        }

        private void Integrate(ref Vector3 start, Vector3 amount, float deltaTime)
        {
            start += amount * deltaTime;
        }

        private void Integrate(ref Quaternion start, Quaternion amount, float deltaTime)
        {
            Vector3 euler = start.eulerAngles;
            this.Integrate(ref euler, amount.eulerAngles, deltaTime);
            start = Quaternion.Euler(euler);
        }

        public override void OnDeserializeSuccess()
        {
            base.OnDeserializeSuccess();
            this.transform.position = this.Physics.LinearPosition;
            this.GetRender().rotation = this.Physics.RotationPosition;
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
            
        }

    }

}
