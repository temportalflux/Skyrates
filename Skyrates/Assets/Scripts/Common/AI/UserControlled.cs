
using Skyrates.Client.Data;
using System;
using UnityEngine;

namespace Skyrates.Common.AI
{

    [CreateAssetMenu(menuName = "Data/AI/Player")]
    public class UserControlled : Steering
    {

        [SerializeField]
        public LocalData ControllerData;
        
        public float ConstantSpeed;

        private float Speed
        {
            get { return this.ConstantSpeed; }
            set
            {
                this.ConstantSpeed = value;
                this.ConstantSpeed = Mathf.Min(this.ConstantSpeed, this.ControllerData.SpeedMax);
                this.ConstantSpeed = Mathf.Max(this.ConstantSpeed, this.ControllerData.SpeedMin);
            }
        }

        public void Awake()
        {
            this.Speed = this.ControllerData.SpeedInitial;
        }

        public void OnDestroy()
        {
            this.Speed = 0.0f;
        }

        public override PhysicsData GetUpdate(BehaviorData data, PhysicsData physics, float deltaTime)
        {
            this.Move(data, ref physics);
            return physics;
        }

        private void Move(BehaviorData data, ref PhysicsData physicsData)
        {
            LocalData.InputData input = this.ControllerData.input;

            Vector3 forward = data.Render.forward;
            //Vector3 vertical = data.Render.up.Flatten(Vector3.forward + Vector3.right).normalized;
            Vector3 vertical = Vector3.up;

            this.Speed += input.Forward.Value;
            
            // For character
            //Vector3 movementForward = cameraForward * this.playerInput.Forward;

            // for ship
            // Value in range [0, input.Forward.Modifier] which changes how fast the ship moves forward
            float forwardSpeed = Mathf.Max(0, input.Forward.Value);
            // value in range [0, 1] of how much constant velocity to counteract
            float backpedal = Mathf.Max(0, -input.Forward.Input);

            //float movementForwardSpeed = ((forwardSpeed + (1 - backpedal)) * this.ConstantSpeed);
            this.ControllerData.MovementSpeed = this.ConstantSpeed;

            Vector3 movementVertical = vertical * input.Vertical.Value;

            // for character
            // Vector3 movementXZ = movementForward + movementStrafe;
            // for ship
            Vector3 movementXZ = forward * this.ControllerData.MovementSpeed;

            Vector3 movementXYZ = movementXZ + movementVertical;

            physicsData.LinearVelocity = movementXYZ;

            // for ship movement
            float rotationY = input.Strafe.Value;
            rotationY *= (1 - input.Forward.Input) * 0.5f;

            // banking
            float rotationZ = Mathf.Sign(-rotationY) * Mathf.Min(Mathf.Abs(rotationY), input.YawAngle);

            // pitching (up/down rotation)
            float rotationX = -1 * (movementVertical.sqrMagnitude > 0 ? Mathf.Sign(movementVertical.y) : 0) * input.PitchAngle;

            physicsData.RotationVelocity = new Vector3(0.0f, rotationY, 0.0f);
            physicsData.RotationAestetic = new Vector3(rotationX, 0.0f, rotationZ);
        }

    }

}
