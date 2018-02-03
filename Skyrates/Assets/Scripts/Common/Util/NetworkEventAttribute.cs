using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class NetworkEventAttribute : Attribute
{

    private Side _sender;
    private Side _receiver;

    public NetworkEventAttribute(Side sender, Side receiver)
    {
        this._sender = sender;
        this._receiver = receiver;
    }

}
