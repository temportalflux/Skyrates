using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client;
using Skyrates.Client.Loot;
using Skyrates.Common.Network;
using UnityEngine;

public class Looter : MonoBehaviour
{

    public uint loot;

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
            // collider is a loot
            Destroy(lootObject.gameObject);
            // TODO: Create event
            this.loot++;
        }
    }

    public void OnGUI()
    {
        if (NetworkComponent.GetSession.IsNetworked)
        {
            if (NetworkComponent.GetSession.PlayerGuid != this.transform.parent.GetComponent<EntityPlayerShip>().Guid)
                return;
        }
        GUIStyle st = new GUIStyle();
        st.fontSize = 50;
        GUI.Label(new Rect(Screen.width - 50, 0, 50, 50), this.loot.ToString(), st);
    }

}
