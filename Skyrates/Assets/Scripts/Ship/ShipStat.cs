using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Loot;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Ship/Stats")]
public class ShipStat : ScriptableObject
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

	public float CalculateDamage(float attack, float defense, float protection)
	{
		//TODO: Add penetration (attack multiplier, the opposite of protection) if wanted.
		//TODO: Add critical hit if wanted.
		return Mathf.Max(
						(attack - defense) * //Subtract defense from attack and multiply the total by the protection formula.
						((100.0f - Mathf.Clamp(protection, 0.0f, 100.0f))  //Reverse the range from 0-100 to 100-0
						/ 100.0f) //Squish the range from 100-0 to 1-0
						, 1.0f); //Can't take less than 1 damage.
	}

	public float CaclulateRecoil(float damage)
	{
		float recoilMultiplier = 0.01f; //Putting this here for now.  Can move or tweak it later.
		return Mathf.Max(damage * recoilMultiplier, 1.0f);
	}

}
