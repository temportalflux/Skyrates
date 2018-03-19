using System.Collections;
using Skyrates.Entity;
using UnityEngine;

namespace Skyrates.Mono
{
    /// <summary>
    /// Interface detected during overlap in <see cref="DistanceCollider"/>.
    /// </summary>
    public interface IDistanceCollidable
    {

        /// <summary>
        /// We enter source's area.
        /// Called from source on the colliable that has entered the area around source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="radius"></param>
        void OnEnterEntityRadius(EntityAI source, float radius);

        /// <summary>
        /// Other enters our area.
        /// When we move into an area such that other has entered our area
        /// </summary>
        /// <param name="other"></param>
        /// <param name="radius"></param>
        void OnOverlapWith(GameObject other, float radius);

    }

    [RequireComponent(typeof(EntityAI))]
    public class DistanceCollider : MonoBehaviour
    {

        public enum DistanceMode
        {
            Sphere,
            Box,
        }

        public DistanceMode Mode;

        public float RadiusSphere;

        public Vector3 RadiusBox;

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
            switch (this.Mode)
            {
                case DistanceMode.Sphere:
                    Gizmos.DrawWireSphere(this.transform.position, this.RadiusSphere);
                    break;
                case DistanceMode.Box:
                    Gizmos.DrawWireCube(this.transform.position, this.RadiusBox);
                    break;
                default:
                    break;
            }
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
            Collider[] colliders;
            switch (this.Mode)
            {
                case DistanceMode.Sphere:
                    colliders = UnityEngine.Physics.OverlapSphere(this.transform.position, this.RadiusSphere, this.CollisionLayers);
                    break;
                case DistanceMode.Box:
                    colliders = UnityEngine.Physics.OverlapBox(this.transform.position, this.RadiusBox, this.transform.rotation, this.CollisionLayers);
                    break;
                default:
                    return;
            }
            foreach (Collider other in colliders)
            {
                IDistanceCollidable[] collidables = other.gameObject.GetComponentsInParent<IDistanceCollidable>();
                foreach (IDistanceCollidable collidable in collidables)
                {
                    if (collidable != null)
                    {
                        collidable.OnEnterEntityRadius(this._owner, this.RadiusSphere);
                        this._owner.OnOverlapWith(other.gameObject, this.RadiusSphere);
                    }
                }
            }
        }

    }
}