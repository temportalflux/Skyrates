using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetworkEvent(Side.Client, Side.Server)]
public class EventHandshakeAccept : NetworkEvent
{

    [BitSerialize(1)]
    public uint clientID;
    
    // TODO: Put on server
    [BitSerialize(2)]
    public Guid playerEntityGuid;

    public EventHandshakeAccept(uint clientID, Guid playerEntityGuid) : base(MessageMap.MessageID.HandshakeAccept)
    {
        this.clientID = clientID;
        this.playerEntityGuid = playerEntityGuid;
    }

    public override byte[] Serialize()
    {
        return base.Serialize();
    }

}
