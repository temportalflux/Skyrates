using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Entity
{

    [RequireComponent(typeof(EntityAI))]
    public class DistanceCollider : MonoBehaviour
    {

        public float Radius;

        public LayerMask CollisionLayers;

        public float Frequency;

        public bool ScatterExecution;

        private float _delay;

        private EntityAI _owner;

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
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
            foreach (Collider other in Physics.OverlapSphere(this.transform.position, this.Radius))
            {
                EntityAI entity = other.gameObject.GetComponent<EntityAI>();
                entity.OnEnterEntityRadius(this._owner, this.Radius);
            }
        }

    }

}
