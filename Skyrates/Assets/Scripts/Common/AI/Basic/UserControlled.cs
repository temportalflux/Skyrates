
using System;
using UnityEngine;

namespace Skyrates.Common.AI
{

    [CreateAssetMenu(menuName = "Data/AI/Basic/Player")]
    public class UserControlled : Steering
    {

        [Serializable]
        public class InputConfig
        {
            // Set via GetInput
            [HideInInspector]
            [SerializeField]
            public float Input;

            // Confgurable
            [SerializeField]
            public float Modifier;

            // Calculates
            public float Value
            {
                get { return this.Input * this.Modifier; }
            }
        }

        [Serializable]
        public class InputData
        {
            // The input which steers the object forward
            [Tooltip("XZ Plan forward movement")]
            [SerializeField]
            public InputConfig Forward;

            [Tooltip("XZ Plan forward movement")]
            [SerializeField]
            public InputConfig Strafe;

            [Tooltip("Y Axis forward movement")]
            [SerializeField]
            public InputConfig Vertical;

        }

        [SerializeField]
        public InputData PlayerInput;

        public float constantSpeed;

        public override void GetSteering(SteeringData data, ref PhysicsData physics)
        {
            this.GetInput(ref this.PlayerInput);
            this.Move(data, this.PlayerInput, ref physics);
        }

        private void GetInput(ref InputData input)
        {

            // ForwardInput is left stick (up/down)
            input.Forward.Input = Input.GetAxis("xbox_stick_l_vertical");

            // Strafe is left stick (left/right)
            input.Strafe.Input = Input.GetAxis("xbox_stick_l_horizontal");

            // Vertical is bumpers
            input.Vertical.Input = Input.GetButton("xbox_bumper_r") ? 1 :
                Input.GetButton("xbox_bumper_l") ? -1 : 0;

        }

        private void Move(SteeringData data, InputData input, ref PhysicsData physicsData)
        {
            Vector3 forward = data.Render.forward;
            Vector3 vertical = data.Render.up.Flatten(Vector3.forward + Vector3.right).normalized;

            // For character
            //Vector3 movementForward = cameraForward * this.playerInput.Forward;
            
            // for ship
            // Value in range [0, input.Forward.Modifier] which changes how fast the ship moves forward
            float forwardSpeed = Mathf.Max(0, input.Forward.Value);
            // value in range [0, 1] of how much constant velocity to counteract
            float backpedal = Mathf.Max(0, -input.Forward.Input);
            Vector3 movementForward = forward * (forwardSpeed + (1 - backpedal) * this.constantSpeed);

            Vector3 movementVertical = vertical * input.Vertical.Value;

            // for character
            // Vector3 movementXZ = movementForward + movementStrafe;
            // for ship
            Vector3 movementXZ = movementForward;

            Vector3 movementXYZ = movementXZ + movementVertical;

            physicsData.LinearVelocity = movementXYZ;

            // for ship movement
            float rotation = input.Strafe.Value;
            rotation *= (1 - input.Forward.Input) * 0.5f;
            physicsData.RotationVelocity = Quaternion.Euler(new Vector3(0.0f, rotation, 0.0f));

        }

    }

}
