using UnityEngine;

namespace Skyrates.AI.Target
{

    public class WaypointAgent : MonoBehaviour
    {
        
        [SerializeField]
        public Waypoint[] Targets;

        public Color WaypointStartColor = Colors.YellowPantone;
        public Color WaypointEndColor = Colors.BlueBolt;

#if UNITY_EDITOR
        public void OnDrawGizmosSelected()
        {
            if (this.Targets.Length > 0 && this.Targets[0] != null)
            {
                Gizmos.color = this.WaypointStartColor;
                Gizmos.DrawLine(this.transform.position, this.Targets[0].transform.position);
            }
            for (int iWaypoint = 0; iWaypoint < this.Targets.Length; iWaypoint++)
            {
                int iPrev = (iWaypoint) % this.Targets.Length;
                int iNext = (iPrev + 1) % this.Targets.Length;
                Waypoint previous = this.Targets[iPrev];
                Waypoint current = this.Targets[iNext];

                if (previous != null)
                {
                    previous.DrawGizmos(this.GetWaypointColor(iPrev));
                    if (current != null)
                    {
                        Gizmos.color = this.GetWaypointColor(iPrev + 1);
                        Gizmos.DrawLine(previous.transform.position, current.transform.position);
                    }
                }
            }
        }

        Color GetWaypointColor(int iWaypoint)
        {
            return Color.Lerp(this.WaypointStartColor, this.WaypointEndColor, (float) iWaypoint / this.Targets.Length);
        }
#endif

    }

}
