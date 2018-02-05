using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetworkEvent(Side.Client, Side.Server)]
public class EventHandshakeJoin : NetworkEvent
{

    public EventHandshakeJoin() : base(MessageMap.MessageID.HandshakeJoin) { }

}
