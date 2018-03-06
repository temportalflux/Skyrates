﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Skyrates.Client.Data
{

    /// <summary>
    /// Encapsulates data for the player which does not need to be transferred over the network.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/Player/Local")]
    [Serializable]
    public class LocalData : ScriptableObject
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

            [Tooltip("Y Axis forward movement")]
            [SerializeField]
            public InputConfig MoveVertical;

            public float PitchAngle;

            [Header("Camera")]
            // The input which steers the object horizontal
            [SerializeField]
            public InputConfig CameraHorizontal;

            // The input which steers the object vertical
            [SerializeField]
            public InputConfig CameraVertical;

            public bool IsShooting;

            public float AimScale;

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

            // TODO: Condense this into an object keyed by ArtilleryComponent
            public float ShootingDataStarboardPercentReloaded;
            public bool ShootingDataStarboardCanReload;
            public float ShootingDataPortPercentReloaded;
            public bool ShootingDataPortCanReload;

            [Header("Active Reload")]
            [Range(0.0f, 1.0f)]
            public float ShootDelayActiveReloadStart = 0.2f;
            [Range(0.0f, 1.0f)]
            public float ShootDelayActiveReloadEnd = 0.3f;
            public float ShootDelay = 2.0f;

        }

        public Input InputData;

        public State StateData;

        /// <summary>
        /// Manages inventory and items.
        /// </summary>
        public Inventory Inventory;

        public void Awake()
        {
            this.StateData.MovementSpeed = 0.0f;

            this.StateData.ShootingDataStarboardPercentReloaded = 1.0f;
            this.StateData.ShootingDataStarboardCanReload = false;
            this.StateData.ShootingDataPortPercentReloaded = 1.0f;
            this.StateData.ShootingDataPortCanReload = false;
        }

        public void OnDestroy()
        {
            this.StateData.MovementSpeed = 0.0f;

            this.StateData.ShootingDataStarboardPercentReloaded = 1.0f;
            this.StateData.ShootingDataStarboardCanReload = false;
            this.StateData.ShootingDataPortPercentReloaded = 1.0f;
            this.StateData.ShootingDataPortCanReload = false;
        }

        public void Init()
        {
            this.StateData.ViewMode = CameraMode.FREE;

			// TODO: Implement reflection if we need to refactor due to the time it takes for the current non-DRY solution.
			Inventory.Clear(); // Needed in order to reset player data in editor.  Could remove from builds with a preprocessor macro if we wanted to.
        }

    }

}
