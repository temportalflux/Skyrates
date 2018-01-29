using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Input;

public class Controller : MonoBehaviour
{

    [Serializable]
    public struct InputKey
    {

        public Xbox.Enum key;

        public float multiplier;

        [HideInInspector]
        public Xbox Config { get; private set; }

        public void load(InputType expectedType)
        {
            this.Config = Xbox.find(this.key);
            Debug.Assert(this.Config.getInputType() == expectedType);
        }

        public float get()
        {
            switch (this.Config.getInputType())
            {
                case InputType.AXIS:
                    return this.multiplier * Input.GetAxis(this.Config.getInputDescriptor());
                case InputType.BUTTON:
                    return Input.GetButton(this.Config.getInputDescriptor()) ? this.multiplier : 0;
                default: return 0;
            }
        }

    }

    public Transform camera;

    public InputKey movementHorizontal;
    public InputKey movementVertical;
    public InputKey cameraHorizontal;
    public InputKey cameraVertical;

    private CharacterController controller;
    private float cameraRadius;

    // Use this for initialization
    void Start()
    {


        this.movementHorizontal.load(InputType.AXIS);
        this.movementVertical.load(InputType.AXIS);
        this.cameraHorizontal.load(InputType.AXIS);
        this.cameraVertical.load(InputType.AXIS);

        this.controller = this.GetComponent<CharacterController>();

        this.cameraRadius = this.camera.localPosition.sqrMagnitude;

    }

    // Update is called once per frame
    void Update()
    {

        Vector3 forwards = this.camera.forward;
        Vector3 strafe = this.camera.right;
        forwards.y = 0;
        strafe.y = 0;


        // Move the player

        forwards *= this.movementVertical.get();
        strafe *= this.movementHorizontal.get();

        this.controller.Move(forwards + strafe);

        // Move the camera
        this.camera.RotateAround(this.transform.position, Vector3.up, this.cameraHorizontal.get());
        this.camera.RotateAround(this.transform.position, this.camera.right, this.cameraVertical.get());

    }

}
