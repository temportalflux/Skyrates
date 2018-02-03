using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetworkEvent
{

    [BitSerialize]
    public byte eventID;

    public string sourceAddress;

    public float transmitTime;

    // For client creation
    public NetworkEvent(MessageMap.MessageID id)
    {
        this.eventID = (byte)id;
    }

    // For Deserialize
    public NetworkEvent() { }

    public virtual void Deserialize(byte[] data)
    {
        BitSerializeAttribute.Deserialize(this, data);
    }

    public virtual void Execute() { }

}
