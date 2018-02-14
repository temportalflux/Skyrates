using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player/Local")]
public class LocalData : ScriptableObject
{

    public uint LootCount;

    public void Init()
    {
        this.LootCount = 0;
    }

}
