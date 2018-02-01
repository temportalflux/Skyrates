using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputConfig
{

    public enum InputType
    {
        BUTTON, AXIS
    }

    public InputType inputType;
    public float multiplier;

    public InputType GetInputType()
    {
        return this.inputType;
    }

    public abstract string GetDescriptor();

    public float GetValue()
    {
        return this.inputType == InputType.BUTTON ? this.GetButtonFloat() : this.GetAxis();
    }

    public bool IsButton()
    {
        return this.GetInputType() == InputType.BUTTON;
    }

    public bool GetButton()
    {
        return Input.GetButton(this.GetDescriptor());
    }

    public bool GetButtonDown()
    {
        return Input.GetButtonDown(this.GetDescriptor());
    }
    
    public bool GetButtonUp()
    {
        return Input.GetButtonUp(this.GetDescriptor());
    }

    public float GetButtonFloat()
    {
        return this.GetButton() ? this.multiplier : 0;
    }

    public float GetAxis()
    {
        return Input.GetAxis(this.GetDescriptor()) * this.multiplier;
    }

    public float GetAxisRaw()
    {
        return Input.GetAxisRaw(this.GetDescriptor()) * this.multiplier;
    }

}
