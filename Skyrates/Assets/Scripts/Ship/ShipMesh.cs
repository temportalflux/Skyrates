using Skyrates.Client.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Client.Ship
{

    [RequireComponent(typeof(Rigidbody))]
    public class ShipMesh : MonoBehaviour
    {

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
