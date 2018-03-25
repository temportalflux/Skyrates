using Skyrates.Entity;
using Skyrates.Mono;
using UnityEngine;

namespace Skyrates.Ship
{

    [RequireComponent(typeof(Rigidbody))]
    public class ForwardCollisionsTo : MonoBehaviour, IDistanceCollidable
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
        
        public void OnEnterEntityRadius(EntityAI source, float radius)
        {
            this.Owner.OnEnterEntityRadius(source, radius);
        }

        public void OnOverlapWith(GameObject other, float radius)
        {
            this.Owner.OnOverlapWith(other, radius);
        }
    }

}
