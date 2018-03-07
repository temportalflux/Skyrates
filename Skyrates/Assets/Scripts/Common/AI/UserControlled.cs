﻿
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
            LocalData.Input input = this.ControllerData.InputData;

            Vector3 forward = data.Render.forward;
            //Vector3 vertical = data.Render.up.Flatten(Vector3.forward + Vector3.right).normalized;
            Vector3 vertical = Vector3.up;

            this.Speed += input.MoveForward.Value;
            
            // For character
            //Vector3 movementForward = cameraForward * this.playerInput.MoveForward;

            // for ship
            // Value in range [0, input.MoveForward.Modifier] which changes how fast the ship moves forward
            float forwardSpeed = Mathf.Max(0, input.MoveForward.Value);
            // value in range [0, 1] of how much constant velocity to counteract
            float backpedal = Mathf.Max(0, -input.MoveForward.Input);

            //float movementForwardSpeed = ((forwardSpeed + (1 - backpedal)) * this.ConstantSpeed);
            this.ControllerData.StateData.MovementSpeed = this.ConstantSpeed;

            Vector3 movementVertical = vertical * input.MoveVertical.Value;

            // for character
            // Vector3 movementXZ = movementForward + movementStrafe;
            // for ship
            Vector3 movementXZ = forward * this.ControllerData.StateData.MovementSpeed;

            Vector3 movementXYZ = movementXZ + movementVertical;

            physicsData.LinearVelocity = movementXYZ;

            // for ship movement
            float rotationY = input.TurnY.Value;
            //rotationY *= (1 - input.MoveForward.Input) * 0.5f;

            // banking
            float rotationZ = Mathf.Sign(-rotationY) * Mathf.Min(Mathf.Abs(rotationY), input.YawAngle);

            // pitching (up/down rotation)
            float rotationX = -1 * (movementVertical.sqrMagnitude > 0 ? Mathf.Sign(movementVertical.y) : 0) * input.PitchAngle;
            
            physicsData.Rotation.Velocity = new Vector3(0.0f, rotationY, 0.0f);

            Vector3 desiredBankingVec = new Vector3(rotationX, 0.0f, rotationZ);
            Vector3 currentRotationVec = physicsData.RotationAestetic.Position.eulerAngles;
            currentRotationVec.y = 0.0f;
            Quaternion desiredBankingQuat = Quaternion.Euler(desiredBankingVec);
            Quaternion currentRotationQuat = Quaternion.Euler(desiredBankingVec);
            float angleBetweenCurrentAndTargetBanking = Quaternion.Angle(currentRotationQuat, desiredBankingQuat);
            if (angleBetweenCurrentAndTargetBanking > 5f)
            {
                Vector3 angleDiff = desiredBankingVec - currentRotationVec;
                angleDiff *= 0.1f;
                physicsData.RotationAestetic.Velocity = angleDiff;
            }
            else
            {
                physicsData.RotationAestetic.Velocity = Vector3.zero;
            }

        }

    }

}
