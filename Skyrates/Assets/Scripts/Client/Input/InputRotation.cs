using Skyrates.Client.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: This is very similar to InputMovement
public class InputRotation : MonoBehaviour
{
    
    public LocalData ControllerData;

    public Transform pivot;

    void Update()
    {
        this.Move();
    }

    private void Move()
    {

        // Grab the vertical axis (up)
        Vector3 dirVertical = this.transform.up;
        dirVertical.x = dirVertical.z = 0;

        // Rotate around the vertical axis
        this.transform.RotateAround(this.pivot.position, dirVertical, this.ControllerData.input.CameraHorizontal.Value);

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
        this.transform.RotateAround(this.pivot.position, dirHorizontal, this.ControllerData.input.CameraVertical.Value);

    }

}
