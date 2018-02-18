using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player/Local")]
public class LocalData : ScriptableObject
{

    public uint LootCount;

    // TODO: Temporary
    public uint LootGoal;

    public void Init()
    {
        this.LootCount = 0;
        this.LootGoal = (uint)Random.Range(10, 60);
    }

}
