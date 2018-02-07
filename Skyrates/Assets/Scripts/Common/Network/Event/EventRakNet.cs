
namespace Skyrates.Common.Network.Event
{
    /// <summary>
    /// Message used when any RakNet message is received.
    /// These are NEVER sent by the game code.
    /// </summary>
    [NetworkEvent(Side.Client, Side.Server)]
    [NetworkEvent(Side.Server, Side.Client)]
    public class EventRakNet : NetworkEvent {}
}
