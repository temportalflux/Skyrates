
namespace Skyrates.Common.Network.Event
{
    [NetworkEvent(Side.Client, Side.Server)]
    [NetworkEvent(Side.Server, Side.Client)]
    public class EventRakNet : NetworkEvent {}
}
