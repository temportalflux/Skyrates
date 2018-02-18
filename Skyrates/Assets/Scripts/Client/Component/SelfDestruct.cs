using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Client.Mono
{

    /// <summary>
    /// Destroys the game object after some time.
    /// </summary>
    public class SelfDestruct : UnityEngine.MonoBehaviour
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
