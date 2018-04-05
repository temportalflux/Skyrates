using UnityEngine;

namespace Skyrates.Mono
{

    /// <summary>
    /// Destroys the game object after some time.
    /// </summary>
    public class SelfDestruct : MonoBehaviour
    {

        public enum DelayMode
        {
            Distance,
            Delay,
        }

        public DelayMode Mode;

        /// <summary>
        /// How long, in seconds, the object should exist in the world for.
        /// </summary>
        public float Delay;

        private float _maxDistSq;
        private Vector3 _start;

        void Start()
        {
            switch (this.Mode)
            {
                case DelayMode.Delay:
                    // Destroy self after x-seconds
                    Destroy(this.gameObject, this.Delay);
                    break;
                case DelayMode.Distance:
                    this._start = this.transform.position;
                    this._maxDistSq = this.Delay * this.Delay;
                    break;
                default:
                    break;
            }
        }

        void Update()
        {
            if (this.Mode == DelayMode.Distance && (this.transform.position - this._start).sqrMagnitude >= this._maxDistSq)
            {
                Destroy(this.gameObject);
            }
        }

    }

}
