using System;
using Skyrates.Loot;
using UnityEngine;

namespace Skyrates.Ship
{
    [CreateAssetMenu(menuName = "Data/Ship/Stats")]
    public class StatBlock : ScriptableObject
    {

        [Tooltip("The amount of damage the ship can take")]
        public int MaxHealth;

        [Tooltip("The speed at which the ship moves")]
        public float MoveSpeed;

        [Header("Loot")]

        [Tooltip("The amount of loot dropped by the ship")]
        public LootTable Loot;

        [Serializable]
        public class HealthFeedback
        {

            // TODO: https://docs.unity3d.com/ScriptReference/EditorGUILayout.CurveField.html

            // amount of damage taken at which the smoke starts at
            [SerializeField]
            public Vector2 SmokeDamage;

            [SerializeField]
            public Vector2 SmokeEmissionAmount;

            // amount of damage taken at which the fire starts at
            [SerializeField]
            public Vector2 FireDamage;

            [SerializeField]
            public Vector2 FireEmissionAmount;

        }
    
        [Header("Effects")]
        [SerializeField]
        public HealthFeedback HealthFeedbackData;

    }
}