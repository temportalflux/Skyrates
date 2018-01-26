using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Input;

public class Controller : MonoBehaviour
{

    [Serializable]
    public struct InputKey
    {

        public Xbox.Enum key;

        public InputType type;

        public float multiplier;

        [HideInInspector]
        public Xbox Config { get; private set; }

        public void load()
        {
            this.Config = Xbox.find(this.key);
            Debug.Assert(this.Config.getInputType() == this.type);
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

        public bool down()
        {
            return this.Config.getInputType() == InputType.BUTTON &&
                Input.GetButtonDown(this.Config.getInputDescriptor());
        }

        public float getRaw()
        {
            return this.Config.getInputType() == InputType.AXIS
                ? Input.GetAxisRaw(this.Config.getInputDescriptor())
                : 0;
        }

    }

    [Header("Movement")]
    public InputKey movementHorizontal;
    public InputKey movementVertical;
    public InputKey movementUp;
    public InputKey movementDown;

    [Header("Camera")]
    public InputKey cameraHorizontal;
    public InputKey cameraVertical;

    public Transform camera;

    [Header("Actions")]
    public InputKey shoot;
    public Shooter shooter;
    public float bulletSpeed;

    private CharacterController controller;
    private float cameraRadius;

    // Use this for initialization
    void Start()
    {


        this.movementHorizontal.load();
        this.movementVertical.load();
        this.movementUp.load();
        this.movementDown.load();
        this.cameraHorizontal.load();
        this.cameraVertical.load();
        this.shoot.load();

        this.controller = this.GetComponent<CharacterController>();

        this.cameraRadius = this.camera.localPosition.sqrMagnitude;

    }

    // Update is called once per frame
    void Update()
    {

        Vector3 forwards = this.camera.forward;
        Vector3 strafe = this.camera.right;
        Vector3 vertical = this.transform.up;
        forwards.y = 0;
        strafe.y = 0;
        vertical.x = vertical.z = 0;


        // Move the player

        forwards *= this.movementVertical.get();
        strafe *= this.movementHorizontal.get();
        vertical *= (this.movementUp.get() - this.movementDown.get());

        this.controller.Move(forwards + vertical);

        float rotateScale = this.movementHorizontal.get();
        Vector3 rotate = this.transform.up * rotateScale;
        this.transform.Rotate(rotate);

        // Move the camera
        this.camera.RotateAround(this.transform.position, Vector3.up, this.cameraHorizontal.get() - rotateScale);
        this.camera.RotateAround(this.transform.position, this.camera.right, this.cameraVertical.get());
        
        if (this.shoot.getRaw() != 0)
        {
            this.shooter.fireProjectile().addForce(this.transform.forward * bulletSpeed);
        }

    }

}
