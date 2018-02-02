using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Side
{
    Server, Client
}

[AttributeUsage(
    AttributeTargets.Class |
    AttributeTargets.Struct |
    AttributeTargets.Field,
    Inherited = true)]
public class SideOnlyAttribute : Attribute
{

    private Side _side;

    public SideOnlyAttribute(Side side)
    {
        this._side = side;
    }

}
