using UnityEngine;

namespace Skyrates.AI.Target
{

    public class Waypoint : MonoBehaviour
    {

        public float Radius;

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, this.Radius);
        }
#endif

    }

}
