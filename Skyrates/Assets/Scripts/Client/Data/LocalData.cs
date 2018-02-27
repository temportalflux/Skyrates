using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            public InputConfig Forward;

            [Tooltip("XZ Plan forward movement")]
            [SerializeField]
            public InputConfig Strafe;

            [Tooltip("Y Axis forward movement")]
            [SerializeField]
            public InputConfig Vertical;

            [Header("Camera")]
            // The input which steers the object horizontal
            [SerializeField]
            public InputConfig CameraHorizontal;

            // The input which steers the object vertical
            [SerializeField]
            public InputConfig CameraVertical;

            [Header("Shooting")]
            /// <summary>
            /// Input to trigger shooting
            /// </summary>
            [SerializeField]
            public InputConfig ShootRight;

            [SerializeField]
            public InputConfig ShootLeft;

            [SerializeField]
            public float ShootDelay;
            [SerializeField]
            public float ShootDelayMin;

            [Header("Menu")]
            [SerializeField]
            public bool MainMenu;

            [SerializeField]
            public bool Back;

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
        public CameraMode ViewMode;

        /// <summary>
        /// The amount of loot currently collected.
        /// Value is 0 during instantiation and destruction.
        /// </summary>
        [Header("Loot")]
        public uint LootCount;

        // TODO: Temporary
        /// <summary>
        /// The amount of loot to collect for the "winstate" to occur.
        /// Set to a random number between 10 and 60 on instantiation and destruction (the latter isn't necessary).
        /// </summary>
        public uint LootGoal;
        
        public void Init()
        {
            this.LootCount = 0;
            this.LootGoal = (uint) UnityEngine.Random.Range(10, 60);

            this.ViewMode = CameraMode.FREE;
        }

    }

}
