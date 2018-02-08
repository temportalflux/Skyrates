﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Ship/Stats")]
public class ShipStat : ScriptableObject
{

    [Tooltip("The amount of damage the ship can take")]
    public int Health;

    [Tooltip("The speed at which the ship moves")]
    public float MoveSpeed;

    [Tooltip("The modifier for how much damage the ship does")]
    public float DamageModifier;

    [Tooltip("The amount of loot dropped by the ship")]
    public int Loot;

}
