using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    }

}
