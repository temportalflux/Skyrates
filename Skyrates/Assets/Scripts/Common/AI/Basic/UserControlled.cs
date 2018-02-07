
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
            public float Speed;

            // Calculates
            public float Value
            {
                get { return this.Input * this.Speed; }
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
            // TODO: Optimize. This can be done via matricies/linear algebra
            Vector3 cameraForward = data.View.forward.Flatten(Vector3.up).normalized;
            Vector3 cameraStrafe = data.View.right.Flatten(Vector3.up).normalized;
            Vector3 vertical = data.View.up.Flatten(Vector3.forward + Vector3.right).normalized;

            // For character
            //Vector3 movementForward = cameraForward * this.playerInput.Forward;
            // for ship
            float forwardSpeed = input.Forward.Value; // + this.constantVelocity;
            Vector3 movementForward = data.Render.forward * forwardSpeed;

            Vector3 movementStrafe = cameraStrafe * input.Strafe.Value;
            Vector3 movementVertical = vertical * input.Vertical.Value;

            // for character
            // Vector3 movementXZ = movementForward + movementStrafe;
            // for ship
            Vector3 movementXZ = movementForward;

            Vector3 movementXYZ = movementXZ + movementVertical;

            physicsData.LinearVelocity = movementXYZ;

            // for ship movement
            physicsData.RotationVelocity = Quaternion.Euler(new Vector3(0.0f, input.Strafe.Value, 0.0f));

        }

    }

}
