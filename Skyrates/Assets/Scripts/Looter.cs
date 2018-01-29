﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looter : MonoBehaviour
{
    
    // meant to be used for raycast (hence mask). must convert bits to compare
    public LayerMask lootLayer;

    public int loot = 0;

    private void Start()
    {
        // TODO: Potentially expensive, and not required
        this.checkTriggers();
    }

    private void checkTriggers()
    {
        foreach (Collider collider in this.GetComponents<Collider>())
        {
            collider.isTrigger = true;
        }
    }

    // Called when some non-trigger collider with a rigidbody enters
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name + ":" + other.gameObject.layer + "==" + this.lootLayer.value);
        // layermask meant to be used for raycast (hence mask). must convert bits to compare
        if ((int)this.lootLayer == 1 << other.gameObject.layer)
        {
            // collider is a loot
            Destroy(other.gameObject);
            this.loot++;
        }
    }

}
