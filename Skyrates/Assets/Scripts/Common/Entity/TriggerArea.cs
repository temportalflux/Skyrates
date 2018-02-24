using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Skyrates.Common.Entity
{

    [RequireComponent(typeof(Collider))]
    public class TriggerArea : MonoBehaviour
    {

        [Serializable]
        public class EventTrigger : UnityEvent<TriggerArea, Collider> { }

        [SerializeField]
        public EventTrigger Enter;
        [SerializeField]
        public EventTrigger Stay;
        [SerializeField]
        public EventTrigger Exit;

        private void OnTriggerEnter(Collider other)
        {
            this.Enter.Invoke(this, other);
        }

        /*
        private void OnTriggerStay(Collider other)
        {
            this.Stay.Invoke(this, other);
        }
        */

        private void OnTriggerExit(Collider other)
        {
            this.Exit.Invoke(this, other);
        }

    }

}
