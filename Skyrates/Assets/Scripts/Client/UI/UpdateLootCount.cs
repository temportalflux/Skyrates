using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateLootCount : MonoBehaviour
{

    public LocalData data;

    public Text LootText;

    private void Update()
    {
        this.LootText.text = this.data.LootCount.ToString();
    }

}