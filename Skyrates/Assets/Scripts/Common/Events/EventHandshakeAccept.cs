using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetworkEvent(Side.Client, Side.Server)]
public class EventHandshakeAccept : NetworkEvent
{

    [BitSerialize(1)]
    public uint clientID;

    public EventHandshakeAccept(uint clientID) : base(MessageMap.MessageID.HandshakeAccept)
    {
        this.clientID = clientID;
    }

}
