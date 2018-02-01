using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputMovement : MonoBehaviour
{

    [Header("Movement")]
    public InputSet movementForward;
    public InputSet movementStrafe;
    public InputSet interact;
    public float speed = 5;

    public Transform forwardView;
    public Transform render;

    private Rigidbody physics;

    // Use this for initialization
    void Start()
    {
        this.physics = this.GetComponent<Rigidbody>();

        this.movementForward.Load();
        this.movementStrafe.Load();
        this.interact.Load();
    }

    // Update is called once per frame
    void Update()
    {
        this.Move();
    }

    private void Move()
    {
        Vector3 cameraForward = this.forwardView.forward.Flatten(Vector3.up);
        Vector3 cameraStrafe = this.forwardView.right.Flatten(Vector3.up);

        Vector3 movementForward = cameraForward * this.movementForward.GetValue();
        Vector3 movementStrafe = cameraStrafe * this.movementStrafe.GetValue();
        Vector3 movement = movementForward + movementStrafe;

        this.physics.velocity = movement.normalized * this.speed;

        if (movement.sqrMagnitude > 0)
        {
            Vector3 directionMovement = this.transform.position;
            directionMovement += movement.normalized;
            this.render.LookAt(directionMovement);

            // TODO: Fix animation rotation of mesh on flip
            //float step = 2.0f * Time.deltaTime;
            //Vector3 newDir = Vector3.RotateTowards(transform.forward, movement.normalized, step, 0.0F);
            //Debug.DrawRay(transform.position, newDir, Color.red);
            //this.mesh.transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

}
