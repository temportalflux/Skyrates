using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetworkEvent(Side.Client, Side.Server)]
public class EventUpdatePlayerPhysics : NetworkEvent
{

    [BitSerialize(1)]
    public PhysicsData data;

    public EventUpdatePlayerPhysics(PhysicsData dataIn) : base(MessageMap.MessageID.UpdatePlayerPhysics)
    {
        this.data = dataIn;
    }

}
