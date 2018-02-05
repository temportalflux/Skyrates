using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Network;
using UnityEngine;

public abstract class NetworkEvent
{

    [BitSerialize]
    public byte eventID;

    public string sourceAddress;

    public float transmitTime;

    // For client creation
    public NetworkEvent(NetworkEventID id)
    {
        this.eventID = (byte)id;
    }

    // For Deserialize
    public NetworkEvent() { }

    public virtual byte[] Serialize()
    {
        return BitSerializeAttribute.Serialize(this);
    }

    public virtual void Deserialize(byte[] data)
    {
        BitSerializeAttribute.Deserialize(this, data);
    }

}
