using Skyrates.Common.AI;
using Skyrates.Data;
using Skyrates.Physics;
using Skyrates.Util.Extension;
using UnityEngine;

namespace Skyrates.AI.Custom
{

    [CreateAssetMenu(menuName = "Data/AI/Player")]
    public class UserControlled : Steering.Steering
    {

        [SerializeField]
        public PlayerData ControllerData;
        
        public float ConstantSpeed;

        private float Speed
        {
            get { return this.ConstantSpeed; }
            set
            {
                this.ConstantSpeed = value;
                this.ConstantSpeed = Mathf.Min(this.ConstantSpeed, this.ControllerData.Movement.SpeedMax);
                this.ConstantSpeed = Mathf.Max(this.ConstantSpeed, this.ControllerData.Movement.SpeedMin);
            }
        }

        protected override void OnEnable()
        {
            this.Speed = this.ControllerData.Movement.SpeedInitial;
        }

        protected override void OnDisable()
        {
            this.Speed = 0.0f;
        }

        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent, float deltaTime)
        {
            physics.HasAesteticRotation = true;
            this.Move(behavioral, ref physics);
            return persistent;
        }

        private void Move(DataBehavioral data, ref PhysicsData physicsData)
        {
            PlayerData.Input input = this.ControllerData.InputData;

            Vector3 vertical = Vector3.up;
            Vector3 forward = data.Render.forward.Flatten(vertical);

            this.Speed += input.MoveForward.Value;
            
            // For character
            //Vector3 movementForward = cameraForward * this.playerInput.MoveForward;
            
            //float movementForwardSpeed = ((forwardSpeed + (1 - backpedal)) * this.ConstantSpeed);
            this.ControllerData.Movement.CurrentSpeed = this.Speed;
            
            // for character
            // Vector3 movementXZ = movementForward + movementStrafe;
            // for ship
            //Vector3 movementXZ = forward * this.ControllerData.StateData.CurrentSpeed;

            physicsData.LinearVelocity =
                (forward * this.Speed)
                +
                (vertical * input.MoveVertical.Value);

            // for ship movement
            float rotationY = input.TurnY.Value;
            //rotationY *= (1 - input.MoveForward.Input) * 0.5f;
            
            physicsData.RotationVelocity = new Vector3(0.0f, rotationY, 0.0f);
            
        }

    }

}
