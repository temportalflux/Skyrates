using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;
using Skyrates.Server.Network;

namespace Skyrates.Client.Network
{

    public class DummyClient : Client
    {

        public override void Connect(Session session)
        {}

        public override void Create()
        {}

        public override void Destroy()
        {}

        public override void Shutdown()
        {}

        public override void StartAndConnect(Session session)
        {}

        public override void StartClient(Session session)
        {}

        public override void StartServer(Session session)
        {}

        public override void Dispatch(NetworkEvent evt, string address, int port, bool broadcast = false)
        {
        }

        public override void Update()
        {
        }

        public override void SubscribeEvents()
        {
            this.HasSubscribed = true;
        }

        public override void UnsubscribeEvents()
        {
            this.HasSubscribed = false;
        }

    }

}
