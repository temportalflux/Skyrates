using Skyrates.Entity;
using UnityEngine;

namespace Skyrates.Ship
{

    [RequireComponent(typeof(Rigidbody))]
    public class ForwardCollisionsTo : MonoBehaviour
    {

        // TODO: Change to GameObject
        public EntityShip Owner;

        private void OnTriggerEnter(Collider other)
        {
            Owner.OnTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            Owner.OnTriggerExit(other);
        }

    }

}
