using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class InputMovement : MonoBehaviour
{

    /// <summary>
    /// Structure to contain input values from controllers.
    /// </summary>
    [Serializable]
    public struct InputData
    {

        // The input which steers the object forward
        [HideInInspector]
        public float ForwardInput;

        [Tooltip("The multiple of the input for ForwardInput")]
        public float ForwardSpeed;

        // The input which steers the object horizontal
        [HideInInspector]
        public float StrafeInput;

        [Tooltip("The multiple of the input for Strafe")]
        public float StrafeSpeed;

        // The input which steers the object vertical
        [HideInInspector]
        public float VerticalInput;

        [Tooltip("The multiple of the input for Vertical")]
        public float VerticalSpeed;

        public float Forward
        {
            get { return this.ForwardInput * this.ForwardSpeed; }
        }

        public float Strafe
        {
            get { return this.StrafeInput * this.StrafeSpeed; }
        }

        public float Vertical
        {
            get { return this.VerticalInput * this.VerticalSpeed; }
        }

    }

    public InputData playerInput;

    [Tooltip("The transform which points towards where the forward direction is.")]
    public Transform forwardView;

    [Tooltip("The root of the render object (must be a child/decendent of this root)")]
    public Transform render;

    private Rigidbody physics;
    
    void Start()
    {
        this.physics = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        this.GetInput();
        this.Move();
    }

    private void GetInput()
    {
        // ForwardInput is left stick (up/down)
        this.playerInput.ForwardInput = Input.GetAxis("xbox_stick_l_vertical");

        // Strafe is left stick (left/right)
        this.playerInput.StrafeInput = Input.GetAxis("xbox_stick_l_horizontal");

        // Vertical is left stick (up/down if A is down)
        this.playerInput.VerticalInput =
            Input.GetButton("xbox_bumper_l")
             ? Input.GetAxis("xbox_stick_r_vertical")
             : 0.0f;
    }

    private void Move()
    {
        // TODO: Optimize. This can be done via matricies/linear algebra
        Vector3 cameraForward = this.forwardView.forward.Flatten(Vector3.up).normalized;
        Vector3 cameraStrafe = this.forwardView.right.Flatten(Vector3.up).normalized;
        Vector3 vertical = this.transform.up.Flatten(Vector3.forward + Vector3.right).normalized;

        Vector3 movementForward = cameraForward * this.playerInput.Forward;
        Vector3 movementStrafe = cameraStrafe * this.playerInput.Strafe;
        Vector3 movementVertical = vertical * this.playerInput.Vertical;

        Vector3 movementXZ = movementForward + movementStrafe;
        Vector3 movementXYZ = movementXZ + movementVertical;

        this.physics.velocity = movementXYZ;

        if (movementXYZ.sqrMagnitude > 0)
        {
            Vector3 directionMovement = this.transform.position;
            // TODO: Bug. This is wonky when moving vertically
            directionMovement += movementXZ.normalized;
            this.render.LookAt(directionMovement);

            // TODO: Improve. Fix animation rotation of mesh on flip
            //float step = 2.0f * Time.deltaTime;
            //Vector3 newDir = Vector3.RotateTowards(transform.forward, movement.normalized, step, 0.0F);
            //Debug.DrawRay(transform.position, newDir, Color.red);
            //this.mesh.transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

}
