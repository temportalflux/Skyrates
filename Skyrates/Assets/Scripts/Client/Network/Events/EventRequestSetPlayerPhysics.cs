using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Network;
using UnityEngine;

[NetworkEvent(Side.Client, Side.Server)]
public class EventRequestSetPlayerPhysics : NetworkEvent
{

    [BitSerialize(1)]
    public uint clientID;

    [BitSerialize(2)]
    public PhysicsData data;

    public EventRequestSetPlayerPhysics(PhysicsData dataIn) : base(NetworkEventID.RequestSetPlayerPhysics)
    {
        this.clientID = NetworkComponent.GetSession.ClientID;
        this.data = dataIn;
    }

}
