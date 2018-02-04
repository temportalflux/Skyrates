using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetworkEvent(Side.Server, Side.Client)]
public class EventConnection : NetworkEvent
{

    public override void Execute()
    {
        // could be accepted or rejected
        if (this.eventID == (byte)MessageMap.MessageID.ConnectionAccepted)
        {
            Debug.Log("Joining server");
            NetworkComponent.GetClient().JoinServer();
        }
    }

}
