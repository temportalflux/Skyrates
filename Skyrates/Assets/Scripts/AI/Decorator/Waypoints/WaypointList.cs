using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.AI.Target
{

    public class WaypointList : MonoBehaviour
    {

        [SerializeField]
        public Waypoint[] Targets;

        public WaypointAgent Agent;
        
#if UNITY_EDITOR
        public void OnDrawGizmosSelected()
        {
            if (this.Agent == null)
                return;
            this.Agent.OnDrawGizmosSelected();
        }
#endif

    }

}
