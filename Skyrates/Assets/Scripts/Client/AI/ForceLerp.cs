using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ForceLerp : MonoBehaviour
{

    public enum State
    {
        Entering,
        StartToEnd,
        EndToStart,
    }

    public AITarget TargetA;

    public AITarget TargetB;

    public float UnitsPerSecond;

    private Rigidbody _physics;

    private State _state;

    private AITarget _currentTarget;

    void Start()
    {
        this._physics = this.GetComponent<Rigidbody>();
        this._state = State.Entering;
        this._currentTarget = this.TargetA;
    }

    void Update()
    {
        Vector3 direction = this._currentTarget.transform.position - this.transform.position;
        direction.Normalize();
        this._physics.position += direction * Time.deltaTime * this.UnitsPerSecond; // 5 units per second
    }

    void OnTriggerStay(Collider other)
    {
        AITarget target = other.GetComponent<AITarget>();
        if (target != null && target.Guid == this._currentTarget.Guid)
        {
            // Switch state
            switch (this._state)
            {
                case State.Entering:
                case State.EndToStart:
                    this._state = State.StartToEnd;
                    this._currentTarget = this.TargetB;
                    break;
                case State.StartToEnd:
                    this._state = State.EndToStart;
                    this._currentTarget = this.TargetA;
                    break;
            }
        }
    }

}
