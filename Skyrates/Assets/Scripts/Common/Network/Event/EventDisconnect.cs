using Skyrates.Common.Network;

[NetworkEvent(Side.Client, Side.Server)]
[NetworkEvent(Side.Server, Side.Client)]
public class EventDisconnect : NetworkEvent
{

    [BitSerialize(1)]
    public uint clientID;

    // deserialize
    public EventDisconnect() : base(NetworkEventID.Disconnect) { }

    // dispatch
    public EventDisconnect(uint clientID) : this()
    {
        this.clientID = clientID;
    }

}
