using UnityEngine;

namespace Skyrates.Respawn
{

    public class RespawnArea : MonoBehaviour
    {

        public Transform[] Locations;

#if UNITY_EDITOR
        public float SpawnScaleVisual = 5.0f;
#endif

        public Transform GetLocation(uint index)
        {
            return index < this.Locations.Length ? this.Locations[index] : null;
        }

        public Transform GetNextRespawnLocation()
        {
            return this.GetLocation((uint)Random.Range(0, this.Locations.Length));
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            foreach (Transform location in this.Locations)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(location.position, location.forward * this.SpawnScaleVisual);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(location.position, location.right * this.SpawnScaleVisual);
                Gizmos.color = Color.green;
                Gizmos.DrawRay(location.position, location.up * this.SpawnScaleVisual);
            }
        }
#endif

    }

}
