using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUnity : InputConfig
{

    public string inputDescriptor;

    public override string GetDescriptor()
    {
        return this.inputDescriptor;
    }

}
