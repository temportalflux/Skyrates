
using Skyrates.Client.Data;
using System;
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

        public void OnEnable()
        {
            this.Speed = this.ControllerData.StateData.SpeedInitial;
        }

        public void OnDisable()
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

            Vector3 forward = data.Render.forward;
            //Vector3 vertical = data.Render.up.Flatten(Vector3.forward + Vector3.right).normalized;
            Vector3 vertical = Vector3.up;

            this.Speed += input.MoveForward.Value;
            
            // For character
            //Vector3 movementForward = cameraForward * this.playerInput.MoveForward;
            
            //float movementForwardSpeed = ((forwardSpeed + (1 - backpedal)) * this.ConstantSpeed);
            this.ControllerData.StateData.MovementSpeed = this.ConstantSpeed;

            Vector3 movementVertical = vertical * input.MoveVertical.Value * ((100.0f + input.AdditionalMovePercent) / 100.0f);

            // for character
            // Vector3 movementXZ = movementForward + movementStrafe;
            // for ship
            Vector3 movementXZ = forward * this.ControllerData.StateData.MovementSpeed * ((100.0f + input.AdditionalMovePercent) / 100.0f); ;

            Vector3 movementXYZ = movementXZ + movementVertical;

            physicsData.LinearVelocity = movementXYZ;

            // for ship movement
            float rotationY = input.TurnY.Value * ((100.0f + input.AdditionalTurnPercent) / 100.0f); ;
            //rotationY *= (1 - input.MoveForward.Input) * 0.5f;
            
            physicsData.RotationVelocity = new Vector3(0.0f, rotationY, 0.0f);
            
        }

        private void LerpRotation(float current, float target, float speed, ref float velocity)
        {
            float rotation = target - current;
            rotation = ((int)(Align.MapToRange(rotation) * 100)) * 0.01f;
            float rotationSize = Mathf.Abs(rotation);
            float dir = rotationSize > 0.0f ? rotation / rotationSize : 0.0f;
            velocity = speed * dir;
        }

    }

}
