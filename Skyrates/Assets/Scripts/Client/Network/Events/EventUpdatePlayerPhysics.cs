using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetworkEvent(Side.Client, Side.Server)]
public class EventUpdatePlayerPhysics : NetworkEvent
{

    [BitSerialize(1)]
    public uint clientID;

    [BitSerialize(2)]
    public PhysicsData data;

    public EventUpdatePlayerPhysics(PhysicsData dataIn) : base(MessageMap.MessageID.UpdatePlayerPhysics)
    {
        this.clientID = NetworkComponent.Session.ClientID;
        this.data = dataIn;
    }

}
