using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerDetector : MonoBehaviour
{
    
    [Serializable]
    public class EventTrigger : UnityEvent<Collider> { }

    [SerializeField]
    public EventTrigger EventOnTriggerEnter;
    [SerializeField]
    public EventTrigger EventOnTriggerExit;

    // Use this for initialization
    private void Start()
    {
        //Debug.Assert(this.GetComponent<Collider>().isTrigger, "TriggerDetector's collider must be a trigger");
    }

    private void OnTriggerEnter(Collider other)
    {
        this.EventOnTriggerEnter.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        this.EventOnTriggerExit.Invoke(other);
    }

}
