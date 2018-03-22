using System;
using Skyrates.Scene;
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

            [Tooltip("XZ Plan forward movement")]
            [SerializeField]
            public InputConfig TurnY;

            public float YawAngle;

            public float YawAngleSpeed;
            public float YawAngleDeadZone;

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
        
        // Not serializable - does not save data
        public class BroadsideCannonSet
        {

            /// <summary>
            /// If the cannon can be actively reloaded.
            /// Can be triggered after starting reload to auto complete if within a specific range.
            /// </summary>
            private bool _canActiveReload = false;

            /// <summary>
            /// If the cannon is in the process of reloading.
            /// </summary>
            private bool _isLoading = false;

            /// <summary>
            /// How much reloaded the cannon is.
            /// </summary>
            [Range(0, 1)]
            private float _percentLoaded = 1.0f;

            /// <summary>
            /// If the cannon is fully loaded.
            /// </summary>
            public bool IsLoaded
            {
                get { return this._percentLoaded >= 1.0f; }
            }

            public float GetPercentLoaded()
            {
                return this._percentLoaded;
            }

            public void Empty()
            {
                this._percentLoaded = 0.0f;
                this._canActiveReload = true;
                this._isLoading = false;
            }

            public void LoadBy(float amount)
            {
                if (!this._isLoading) return;

                this._percentLoaded = Mathf.Min(1.0f, this._percentLoaded + amount);

                if (this._percentLoaded >= 1.0f)
                {
                    this.Load();
                }
            }

            private void Load()
            {
                this._canActiveReload = false;
                this._isLoading = false;
                this._percentLoaded = 1.0f;
            }

            public void TryReload(float start, float end)
            {
                if (!this._isLoading)
                {
                    this._isLoading = true;
                    this._percentLoaded = 0.0f;
                }
                else if (this._canActiveReload)
                {
                    if (this._percentLoaded >= start && this._percentLoaded <= end)
                    {
                        this.Load();
                    }
                    else
                    {
                        this._canActiveReload = false;
                    }
                }
            }

        }

        [Serializable]
        public class State
        {

            public float SpeedInitial;

            // TODO: Get from total speed of player
            public float SpeedMin = 0;

            // TODO: Get from total speed of player
            public float SpeedMax = 60;

            // TODO: This is set via UserControlled AI - this should be calculated on input update
            public float MovementSpeed;

            /// <summary>
            /// The current state of the player's camera
            /// </summary>
            [Header("Controls")]
            [HideInInspector]
            public CameraMode ViewMode;

            // TODO: Create a class which references all artillery for a specific set, and also handles if/how much loaded it is.
            public BroadsideCannonSet CannonSetStarboard;
            public BroadsideCannonSet CannonSetPort;

            [Header("Active Reload")]
            [Range(0.0f, 1.0f)]
            public float ShootDelayActiveReloadStart = 0.2f;
            [Range(0.0f, 1.0f)]
            public float ShootDelayActiveReloadEnd = 0.3f;
            public float ShootDelay = 2.0f;

        }

        /// <summary>
        /// Strictly input data to be processed by a <see cref="PlayerController"/>
        /// </summary>
        public Input InputData;

        public State StateData;

        /// <summary>
        /// Manages inventory and items.
        /// </summary>
        public Inventory Inventory;

        public void OnEnable()
        {
            this.StateData.MovementSpeed = 0.0f;
        }

        public void OnDisable()
        {
            this.StateData.MovementSpeed = 0.0f;
        }

        public void Init()
        {
            this.StateData.ViewMode = CameraMode.FREE;
			this.InputData.BlockInputs = false;

			// TODO: Implement reflection if we need to refactor due to the time it takes for the current non-DRY solution.
			Inventory.Clear(); // Needed in order to reset player data in editor.  Could remove from builds with a preprocessor macro if we wanted to.
        }

    }

}
