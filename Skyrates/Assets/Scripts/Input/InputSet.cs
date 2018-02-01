using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InputSet
{

    public enum Controller
    {
        KEYBOARD,
        MOUSE,
        XBOX,
        PS4,
        JOYCON,
    }
    
    public static readonly Controller[] CONTROLLERS =
    {
        Controller.KEYBOARD, Controller.MOUSE,
        Controller.XBOX, Controller.PS4,
        Controller.JOYCON
    };

    [Serializable]
    public struct Configuration
    {
        public Controller inputType;
        public InputConfig descriptor;
    }

    public Configuration[] configs;

    private Dictionary<Controller, InputConfig> mappedConfigs;

    public void loadConfigs()
    {
        this.mappedConfigs.Clear();
        foreach (Configuration config in this.configs)
        {
            this.mappedConfigs.Add(config.inputType, config.descriptor);
        }
    }

    public float GetValue(Controller target)
    {
        InputConfig config;
        return this.mappedConfigs.TryGetValue(target, out config) ? config.GetValue() : 0;
    }

    public float GetValue()
    {
        foreach (Controller target in CONTROLLERS)
        {
            float value = this.GetValue(target);
            if (value != 0.0f) return value;
        }
        return 0.0f;
    }

    public bool GetButton(Controller target)
    {
        InputConfig config;
        return this.mappedConfigs.TryGetValue(target, out config) && config.GetButton();
    }
    
    public bool GetButton()
    {
        foreach (Controller target in CONTROLLERS)
        {
            if (this.GetButton(target)) return true;
        }
        return false;
    }

    public bool GetButtonDown(Controller target)
    {
        InputConfig config;
        return this.mappedConfigs.TryGetValue(target, out config) && config.GetButtonDown();
    }

    public bool GetButtonDown()
    {
        foreach (Controller target in CONTROLLERS)
        {
            if (this.GetButtonDown(target)) return true;
        }
        return false;
    }
    
    public bool GetButtonUp(Controller target)
    {
        InputConfig config;
        return this.mappedConfigs.TryGetValue(target, out config) && config.GetButtonUp();
    }

    public bool GetButtonUp()
    {
        foreach (Controller target in CONTROLLERS)
        {
            if (this.GetButtonUp(target)) return true;
        }
        return false;
    }

    public float GetAxis(Controller target)
    {
        InputConfig config;
        return this.mappedConfigs.TryGetValue(target, out config) ? config.GetAxis() : 0;
    }

    public float GetAxis()
    {
        foreach (Controller target in CONTROLLERS)
        {
            float value = this.GetAxis(target);
            if (value != 0.0f) return value;
        }
        return 0.0f;
    }

    public float GetAxisRaw(Controller target)
    {
        InputConfig config;
        return this.mappedConfigs.TryGetValue(target, out config) ? config.GetAxisRaw() : 0;
    }

    public float GetAxisRaw()
    {
        foreach (Controller target in CONTROLLERS)
        {
            float value = this.GetAxisRaw(target);
            if (value != 0.0f) return value;
        }
        return 0.0f;
    }

}
