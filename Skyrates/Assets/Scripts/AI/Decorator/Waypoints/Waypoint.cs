using UnityEngine;

namespace Skyrates.AI.Target
{

    public class Waypoint : MonoBehaviour
    {

        public WaypointList Owner;

        public float Radius;

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (this.Owner == null) return;
            this.Owner.OnDrawGizmosSelected();
        }

        public void DrawGizmos(Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, this.Radius);
        }
#endif

    }

}
