using UnityEngine;

namespace Skyrates.Mono
{

    /// <summary>
    /// Kill box for the world.
    /// </summary>
    public class OutOfWorldBoundary : MonoBehaviour
    {

        void OnTriggerEnter(Collider col)
        {
            Destroy(col.gameObject);
        }

    }


}
