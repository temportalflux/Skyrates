using System;
using Skyrates.Client.Input;
using UnityEngine;

namespace Skyrates.Data
{

    /// <summary>
    /// Encapsulates data for the player which does not need to be transferred over the network.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/Player/Local")]
    [Serializable]
    public class PlayerData : ScriptableObject
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
        
        /// <summary>
        /// Structure to contain input values from controllers.
        /// </summary>
        [Serializable]
        public struct Input
        {

            [Header("Movement")]
            // The input which steers the object forward
            [Tooltip("XZ Plan forward movement")]
            [SerializeField]
            public InputConfig MoveForward;

            [Header("Banking")]
            [Tooltip("XZ Plan forward movement")]
            [SerializeField]
            public InputConfig TurnY;

            public float YawAngle;

            public float YawAngleSpeed;
            public float YawAngleDeadZone;

            [Header("Pitch")]
            [Tooltip("Y Axis forward movement")]
            [SerializeField]
            public InputConfig MoveVertical;

            public float PitchAngle;

            public float PitchAngleSpeed;
            public float PitchAngleDeadZone;

            [Header("Camera")]
            // The input which steers the object horizontal
            [SerializeField]
            public InputConfig CameraHorizontal;

            // The input which steers the object vertical
            [SerializeField]
            public InputConfig CameraVertical;

			/// <summary>
			/// While true, all inputs are blocked except for interact.
			/// </summary>
			public bool BlockInputs;

		}

        /// <summary>
        /// The possible states the player's camera can be in
        /// </summary>
        public enum CameraMode
        {
            FREE,
            LOCK_DOWN,
            LOCK_RIGHT,
            LOCK_LEFT,
        }

        /// <summary>
        /// Strictly input data to be processed by a <see cref="PlayerController"/>
        /// </summary>
        [Header("Input")]
        public Input InputData;

        /// <summary>
        /// The current state of the player's camera
        /// </summary>
        [Header("Misc")]
        [HideInInspector]
        public CameraMode ViewMode;

        [SerializeField]
        public StateMovement Movement;

        [SerializeField]
        public StateArtillery Artillery;

        /// <summary>
        /// Manages inventory and items.
        /// </summary>
        [HideInInspector]
        public Inventory Inventory;

        public void Init()
        {
            this.ViewMode = CameraMode.FREE;
			this.InputData.BlockInputs = false;

			// TODO: Implement reflection if we need to refactor due to the time it takes for the current non-DRY solution.
			Inventory.Clear(); // Needed in order to reset player data in editor.  Could remove from builds with a preprocessor macro if we wanted to.
        }

    }

}
