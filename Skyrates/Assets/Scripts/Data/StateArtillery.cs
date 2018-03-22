using System;
using UnityEngine;

namespace Skyrates.Data
{

    [Serializable]
    public class StateArtillery
    {

        [SerializeField]
        public StateArtilleryBroadside Starboard;

        [SerializeField]
        public StateArtilleryBroadside Port;

    }

}
