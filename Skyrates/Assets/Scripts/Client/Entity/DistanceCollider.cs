using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Entity
{

    public interface DistanceCollidable
    {

        /// <summary>
        /// Called from source on the colliable that has entered the area around source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="radius"></param>
        void OnEnterEntityRadius(EntityAI source, float radius);

        /// <summary>
        /// When we move into an area such that other has entered our area
        /// </summary>
        /// <param name="other"></param>
        /// <param name="radius"></param>
        void OnOverlapWith(GameObject other, float radius);

    }

    [RequireComponent(typeof(EntityAI))]
    public class DistanceCollider : MonoBehaviour
    {

        public float Radius;

        public LayerMask CollisionLayers;
        
        public Color Color;

        public float Frequency;

        public bool ScatterExecution;

        private float _delay;

        private EntityAI _owner;

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = this.Color;
            Gizmos.DrawWireSphere(transform.position, this.Radius);
        }
#endif

        void Start()
        {
            this._owner = this.GetComponent<EntityAI>();
            this._delay = this.ScatterExecution ? Random.Range(0, this.Frequency) : 0;
            StartCoroutine(this.AlertCollisions());
        }

        private IEnumerator AlertCollisions()
        {
            while (this.gameObject != null)
            {
                yield return new WaitForSeconds(this.Frequency + this._delay);
                this._delay = 0.0f;
                this.ExecuteAlertCollisions();
            }
        }

        private void ExecuteAlertCollisions()
        {
            foreach (Collider other in Physics.OverlapSphere(this.transform.position, this.Radius, this.CollisionLayers))
            {
                DistanceCollidable collidable = other.gameObject.GetComponent<DistanceCollidable>();
                if (collidable != null)
                {
                    collidable.OnEnterEntityRadius(this._owner, this.Radius);
                    this._owner.OnOverlapWith(other.gameObject, this.Radius);
                }
            }
        }

    }

}
