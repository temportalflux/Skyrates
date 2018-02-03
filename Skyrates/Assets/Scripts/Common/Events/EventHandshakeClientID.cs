using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetworkEvent(Side.Server, Side.Client)]
public class EventHandshakeClientID : NetworkEvent
{

    [BitSerialize(1)]
    public uint clientID;
    
    // for deserialize
    public EventHandshakeClientID()
    {
    }

    public override void Execute()
	{
        Debug.Log("Client got id " + this.clientID);

        // Set the client ID
        NetworkComponent.Session.SetClientID(this.clientID);
        // Mark the client as connected to the server (it can now process updates)
	    NetworkComponent.Session.Connected = true;

        // TODO: Decouple via events
        NetworkComponent.GetClient().Dispatch(new EventHandshakeAccept(this.clientID, Entity.NewGuid()));
        SceneLoader.Instance.ActivateNext();
	}

}
