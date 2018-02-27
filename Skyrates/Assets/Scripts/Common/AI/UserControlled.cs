
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
        
        public override PhysicsData GetUpdate(BehaviorData data, PhysicsData physics)
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

            // For character
            //Vector3 movementForward = cameraForward * this.playerInput.Forward;

            // for ship
            // Value in range [0, input.Forward.Modifier] which changes how fast the ship moves forward
            float forwardSpeed = Mathf.Max(0, input.Forward.Value);
            // value in range [0, 1] of how much constant velocity to counteract
            float backpedal = Mathf.Max(0, -input.Forward.Input);

            float movementForwardSpeed = ((forwardSpeed + (1 - backpedal)) * this.ConstantSpeed);
            Vector3 movementForward = forward * movementForwardSpeed;

            Vector3 movementVertical = vertical * input.Vertical.Value;

            // for character
            // Vector3 movementXZ = movementForward + movementStrafe;
            // for ship
            Vector3 movementXZ = movementForward;

            Vector3 movementXYZ = movementXZ + movementVertical;

            physicsData.LinearVelocity = movementXYZ;

            // for ship movement
            float rotationY = input.Strafe.Value;
            rotationY *= (1 - input.Forward.Input) * 0.5f;
            float rotationZ = Mathf.Sign(-rotationY) * Mathf.Min(Mathf.Abs(rotationY), 10);
            physicsData.RotationVelocity = new Vector3(0.0f, rotationY, 0.0f);
            physicsData.RotationAestetic = new Vector3(0.0f, 0.0f, rotationZ);
        }

    }

}
