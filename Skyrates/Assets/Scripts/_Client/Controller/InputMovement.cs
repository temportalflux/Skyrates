using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputMovement : MonoBehaviour
{

    /// <summary>
    /// Structure to contain input values from controllers.
    /// </summary>
    [Serializable]
    public struct InputData
    {
        // The input which steers the object forward
        //[HideInInspector]
        public float ForwardInput;

        [Tooltip("The multiple of the input for ForwardInput")]
        public float ForwardSpeed;

        // The input which steers the object horizontal
        //[HideInInspector]
        public float StrafeInput;

        [Tooltip("The multiple of the input for Strafe")]
        public float StrafeSpeed;

        // The input which steers the object vertical
        //[HideInInspector]
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

    public float constantVelocity;

    [Tooltip("The transform which points towards where the forward direction is.")]
    public Transform forwardView;

    [Tooltip("The root of the render object (must be a child/decendent of this root)")]
    public Transform render;

    public Rigidbody physics;

    public PhysicsData physicsData;

    void Start()
    {
        this.physicsData = new PhysicsData();
    }
    
    void Update()
    {
        this.GetInput(ref this.playerInput);
        this.Move(this.playerInput, ref this.physicsData);
        this.ApplyPhysics(ref this.physicsData);

    }

    private void GetInput(ref InputData input)
    {
        // ForwardInput is left stick (up/down)
        input.ForwardInput = Input.GetAxis("xbox_stick_l_vertical");

        // Strafe is left stick (left/right)
        input.StrafeInput = Input.GetAxis("xbox_stick_l_horizontal");

        // Vertical is left stick (up/down if A is down)
        input.VerticalInput =
            Input.GetButton("xbox_bumper_l")
             ? Input.GetAxis("xbox_stick_r_vertical")
             : 0.0f;
    }

    private void Move(InputData input, ref PhysicsData physicsData)
    {
        // TODO: Optimize. This can be done via matricies/linear algebra
        Vector3 cameraForward = this.forwardView.forward.Flatten(Vector3.up).normalized;
        Vector3 cameraStrafe = this.forwardView.right.Flatten(Vector3.up).normalized;
        Vector3 vertical = this.transform.up.Flatten(Vector3.forward + Vector3.right).normalized;

        // For character
        //Vector3 movementForward = cameraForward * this.playerInput.Forward;
        // for ship
        Vector3 movementForward = this.render.forward * (input.Forward + this.constantVelocity);
        
        Vector3 movementStrafe = cameraStrafe * input.Strafe;
        Vector3 movementVertical = vertical * input.Vertical;

        // for character
        // Vector3 movementXZ = movementForward + movementStrafe;
        // for ship
        Vector3 movementXZ = movementForward;

        Vector3 movementXYZ = movementXZ + movementVertical;

        physicsData.VelocityLinear = movementXYZ;

        // for ship movement
        physicsData.VelocityRotationEuler = new Vector3(0.0f, input.Strafe, 0.0f);

        if (movementXYZ.sqrMagnitude > 0)
        {
            Vector3 directionMovement = this.transform.position;
            directionMovement += movementXZ.normalized;
            // For character movement
            //this.render.LookAt(directionMovement);

            // TODO: Improve. Fix animation rotation of mesh on flip
            //float step = 2.0f * Time.deltaTime;
            //Vector3 newDir = Vector3.RotateTowards(transform.forward, movement.normalized, step, 0.0F);
            //Debug.DrawRay(transform.position, newDir, Color.red);
            //this.mesh.transform.rotation = Quaternion.LookRotation(newDir);
        }

    }

    public void ApplyPhysics(ref PhysicsData physicsData)
    {
        physicsData.PositionLinear = this.transform.position;
        physicsData.PositionRotational = this.render.rotation.eulerAngles;

        this.physics.velocity = physicsData.VelocityLinear;
        this.render.Rotate(physicsData.VelocityRotationEuler, Space.World);

        bool moved = false;
        moved = moved || physicsData.VelocityLinear.sqrMagnitude > 0;
        moved = moved || physicsData.VelocityRotationEuler.sqrMagnitude > 0;

        if (moved)
        {
            // TODO: Fire unity event
            NetworkComponent.GetClient().Dispatch(new EventUpdatePlayerPhysics(physicsData));
        }
    }

}
