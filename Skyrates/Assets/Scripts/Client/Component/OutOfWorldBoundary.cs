using UnityEngine;

namespace Skyrates.Client.Mono
{

    /// <summary>
    /// Kill box for the world.
    /// </summary>
    public class OutOfWorldBoundary : UnityEngine.MonoBehaviour
    {

        void OnTriggerEnter(Collider col)
        {
            Destroy(col.gameObject);
        }

    }


}
