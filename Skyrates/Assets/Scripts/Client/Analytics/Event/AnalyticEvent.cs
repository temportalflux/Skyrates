using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticEvent
{

    private EnumAnalyticEvent evtName;
    private Dictionary<string, object> data;

    private Guid sessionID
    {
        get { return (Guid)this[EnumAnalyticParam.SessionID]; }
        set { this[EnumAnalyticParam.SessionID] = value; }
    }

    public AnalyticEvent(EnumAnalyticEvent name)
    {
        this.evtName = name;
        this.data = new Dictionary<string, object>();

        this.sessionID = Analytics.SessionID;
    }

    public object this[EnumAnalyticParam key]
    {
        get { return this.data[key.ToString()]; }
        set { this.data[key.ToString()] = value; }
    }

    public void Dispatch()
    {
        Analytics.Dispatch(this.evtName, this.data);
    }

}
