using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class NetworkEventAttribute : Attribute
{

    private readonly Side _sender;
    private readonly Side _receiver;

    public NetworkEventAttribute(Side sender, Side receiver)
    {
        this._sender = sender;
        this._receiver = receiver;
    }

    public Side GetSender()
    {
        return this._sender;
    }

    public Side GetReceiver()
    {
        return this._receiver;
    }

}
