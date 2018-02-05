using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetworkEvent(Side.Client, Side.Server)]
[NetworkEvent(Side.Server, Side.Client)]
public class EventDisconnect : NetworkEvent
{

    [BitSerialize(1)]
    public uint clientID;

    // deserialize
    public EventDisconnect() : base(MessageMap.MessageID.Disconnect) { }

    // dispatch
    public EventDisconnect(uint clientID) : this()
    {
        this.clientID = clientID;
    }

}
