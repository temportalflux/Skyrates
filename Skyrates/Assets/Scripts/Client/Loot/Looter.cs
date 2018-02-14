using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Loot;
using Skyrates.Common.Network;
using UnityEngine;

public class Looter : MonoBehaviour
{

    public EntityPlayerShip Owner;

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
        Loot lootObject = other.GetComponent<Loot>();
        if (lootObject != null)
        {
            // Must be passed off as game event, where networking called OnLootCollided
            GameManager.Events.Dispatch(new EventLootCollided(this.Owner, lootObject));
        }
    }

}
