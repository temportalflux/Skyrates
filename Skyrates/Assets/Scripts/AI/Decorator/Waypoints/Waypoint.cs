using UnityEngine;

namespace Skyrates.AI.Target
{

    public class Waypoint : MonoBehaviour
    {

        public WaypointAgent Agent;

        public float Radius;

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (this.Agent == null) return;
            this.Agent.OnDrawGizmosSelected();
        }

        public void DrawGizmos(Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, this.Radius);
        }
#endif

    }

}
