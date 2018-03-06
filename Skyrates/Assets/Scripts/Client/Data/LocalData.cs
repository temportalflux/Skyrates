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
        public struct InputData
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

        public InputData input;

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
        /// The current state of the player's camera
        /// </summary>
        [Header("Controls")]
        [HideInInspector]
        public CameraMode ViewMode;

        /// <summary>
        /// Manages inventory and items.
        /// </summary>
        public Inventory Inventory;

        public float SpeedInitial;

        // TODO: Get from total speed of player
        public float SpeedMin
        {
            get { return 0; }
        }
        // TODO: Get from total speed of player
        public float SpeedMax
        {
            get { return 60; }
        }

        // TODO: This is set via UserControlled AI - this should be calculated on input update
        public float MovementSpeed;

        public void Awake()
        {
            this.MovementSpeed = 0.0f;
        }

        public void OnDestroy()
        {
            this.MovementSpeed = 0.0f;
        }

        public void Init()
        {
            this.ViewMode = CameraMode.FREE;
			// TODO: Implement reflection if we need to refactor due to the time it takes for the current non-DRY solution.
			Inventory.Clear(); // Needed in order to reset player data in editor.  Could remove from builds with a preprocessor macro if we wanted to.
        }

    }

}
