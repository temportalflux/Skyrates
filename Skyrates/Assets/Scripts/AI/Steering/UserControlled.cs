
using Skyrates.Client.Data;
using System;
using Skyrates.AI;
using UnityEngine;

namespace Skyrates.Common.AI
{

    [CreateAssetMenu(menuName = "Data/AI/Player")]
    public class UserControlled : Steering
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
                this.ConstantSpeed = Mathf.Min(this.ConstantSpeed, this.ControllerData.StateData.SpeedMax);
                this.ConstantSpeed = Mathf.Max(this.ConstantSpeed, this.ControllerData.StateData.SpeedMin);
            }
        }

        protected override void OnEnable()
        {
            this.Speed = this.ControllerData.StateData.SpeedInitial;
        }

        protected override void OnDisable()
        {
            this.Speed = 0.0f;
        }

        public override object GetUpdate(ref BehaviorData data, ref PhysicsData physics, float deltaTime, object pData)
        {
            physics.HasAesteticRotation = true;
            this.Move(data, ref physics);
            return pData;
        }

        private void Move(BehaviorData data, ref PhysicsData physicsData)
        {
            PlayerData.Input input = this.ControllerData.InputData;

            Vector3 vertical = Vector3.up;
            Vector3 forward = data.Render.forward.Flatten(vertical);

            this.Speed += input.MoveForward.Value;
            
            // For character
            //Vector3 movementForward = cameraForward * this.playerInput.MoveForward;
            
            //float movementForwardSpeed = ((forwardSpeed + (1 - backpedal)) * this.ConstantSpeed);
            this.ControllerData.StateData.MovementSpeed = this.Speed;
            
            // for character
            // Vector3 movementXZ = movementForward + movementStrafe;
            // for ship
            //Vector3 movementXZ = forward * this.ControllerData.StateData.MovementSpeed;

            physicsData.LinearVelocity =
                (forward * this.ControllerData.StateData.MovementSpeed)
                +
                (vertical * input.MoveVertical.Value);

            // for ship movement
            float rotationY = input.TurnY.Value;
            //rotationY *= (1 - input.MoveForward.Input) * 0.5f;
            
            physicsData.RotationVelocity = new Vector3(0.0f, rotationY, 0.0f);
            
        }

    }

}
