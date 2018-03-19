using UnityEngine;

namespace Skyrates.Mono
{

    /// <summary>
    /// Destroys the game object after some time.
    /// </summary>
    public class SelfDestruct : MonoBehaviour
    {

        /// <summary>
        /// How long, in seconds, the object should exist in the world for.
        /// </summary>
        public float Delay;

        void Start()
        {
            // Destroy self after x-seconds
            Destroy(this.gameObject, this.Delay);
        }

    }

}
