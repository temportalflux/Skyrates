using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Client.Network.Test
{
    public class NetworkTest : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            NetworkComponent.GetSession.Port = 425;
            NetworkComponent.Instance.StartClient();
        }

    }
}
