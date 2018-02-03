using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: This is very similar to InputMovement
public class InputRotation : MonoBehaviour
{

    /// <summary>
    /// Structure to contain input values from controllers.
    /// </summary>
    [Serializable]
    public struct InputData
    {

        // The input which steers the object horizontal
        [HideInInspector]
        public float HorizontalInput;

        [Tooltip("The multiple of the input for Horizontal")]
        public float HorizontalSpeed;

        // The input which steers the object vertical
        [HideInInspector]
        public float VerticalInput;

        [Tooltip("The multiple of the input for Vertical")]
        public float VerticalSpeed;
        
        public float Horizontal
        {
            get { return this.HorizontalInput * this.HorizontalSpeed; }
        }

        public float Vertical
        {
            get { return this.VerticalInput * this.VerticalSpeed; }
        }

    }

    public InputData playerInput;

    public Transform pivot;

    void Update()
    {
        this.GetInput();
        this.Move();
    }

    private void GetInput()
    {

        // Vertical is right stick (up/down)
        this.playerInput.VerticalInput = Input.GetAxis("xbox_stick_r_vertical");

        // Horizontal is right stick (left/right)
        this.playerInput.HorizontalInput = Input.GetAxis("xbox_stick_r_horizontal");

    }

    private void Move()
    {

        // Grab the vertical axis (up)
        Vector3 dirVertical = this.transform.up;
        dirVertical.x = dirVertical.z = 0;

        // Rotate around the vertical axis
        this.transform.RotateAround(this.pivot.position, dirVertical, this.playerInput.Horizontal);

        // Grab the x axis (right)
        Vector3 dirHorizontal = this.transform.right;
        dirVertical.y = dirVertical.z = 0;

        
        /*
        float rotateDegrees = this.playerInput.Vertical;
        Vector3 currentVector = transform.position - this.pivot.position;
        currentVector.y = 0;
        Vector3 iVec = this.transform.forward.Flatten(Vector3.up);
        float angleBetween = Vector3.Angle(iVec, currentVector) * (Vector3.Cross(iVec, currentVector).y > 0 ? 1 : -1);
        float newAngle = Mathf.Clamp(angleBetween + rotateDegrees, -90, 40);
        rotateDegrees = newAngle - angleBetween;
        */

        // Pivot around the local left/right axis (right of the facing direction)
        this.transform.RotateAround(this.pivot.position, dirHorizontal, this.playerInput.Vertical);

    }

}
