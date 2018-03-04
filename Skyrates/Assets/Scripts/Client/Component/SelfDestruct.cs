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

        public enum DelayMode
        {
            Time,
            Distance,
            First,
        }

        public DelayMode Mode;

        /// <summary>
        /// How long, in seconds, the object should exist in the world for.
        /// </summary>
        public float Delay;

        public float Distance;

        private Vector3 _start;

        void Start()
        {
            this._start = this.transform.position;
            StartCoroutine(this.DestroyAfter());
        }

        IEnumerator DestroyAfter()
        {
            if (this.Mode == DelayMode.Time)
            {
                // Destroy self after x-seconds
                Destroy(this.gameObject, this.Delay);
                yield break;
            }

            float timeElapsed = 0.0f;
            float timePrev = Time.time;
            while (true)
            {
                yield return null;

                timeElapsed += Time.time - timePrev;
                timePrev = Time.time;

                float distSq = (this.transform.position - this._start).sqrMagnitude;
                if (distSq >= this.Distance * this.Distance || (this.Mode == DelayMode.First && timeElapsed >= this.Delay))
                {
                    break;
                }
            }

            Destroy(this.gameObject);
        }

    }

}
